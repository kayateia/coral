#region License
/*
	CliMOO - Multi-User Dungeon, Object Oriented for the web
	Copyright (C) 2010-2014 Kayateia

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion
namespace Kayateia.Climoo.Scripting.Coral
{
using System;
using System.Reflection;

	/// <summary>
/// Apply to methods and properties you wish to "pass through" to Coral scripts.
/// </summary>
public class CoralPassthroughAttribute : System.Attribute { }

/// <summary>
/// Wraps a .NET object and uses reflection to pass method and member accesses
/// back and forth to Coral.
/// </summary>
/// <remarks>
/// To use, simply tag methods or properties in your class with CoralPassthrough,
/// and ensure that they use Coral-compatible data types (int, bool, string,
/// string[], and other Passthrough-wrappable .NET objects). You can then
/// register them into a scope. Everything else operates under an IJW (It
/// Just Works) principle.
/// 
/// This is the preferred way for clients of the library to make metal objects.
/// </remarks>
public class Passthrough
{
	/// <summary>
	/// Construct a passthrough.
	/// </summary>
	/// <param name="o">The object we're wrapping.</param>
	public Passthrough( object o )
	{
		_obj = o;
		_t = _obj.GetType();
	}

	/// <summary>
	/// Register us on a const scope.
	/// </summary>
	public void registerConst( ConstScope scope, string name )
	{
		scope.setConstant( name, getObject( name ) );
	}

	/// <summary>
	/// Register us on a regular scope.
	/// </summary>
	public void registerScope( IScope scope, string name )
	{
		scope.set( name, getObject( name ) );
	}

	// We tag onto this to keep a reference to the type for stuff below.
	public class PassthroughMetalObject : MetalObject {
		public PassthroughMetalObject() { }

		public object innerObject { get; set; }
	}

	/// <summary>
	/// Gets the metal object we would put into the scope.
	/// </summary>
	public MetalObject getObject( string name )
	{
		return new PassthroughMetalObject()
		{
			indexLookup = (state, idx) => 
			{
				if( !hasIndex( state, idx ) )
					throw new ArgumentException( "{0} has no index '{1}'".FormatI( name, idx ) );
				doIndexLookup( state, idx );
			},
			memberLookup = (state, memname) =>
			{
				if( !hasMember( state, memname ) )
					throw new InvalidOperationException( "{0} has no member '{1}'".FormatI( name, memname ) );
				state.pushResult( new LValue()
				{
					read = st => doMemberRead( state, memname ),
					write = (st,val) => doMemberWrite( state, memname, val )
				} );
			},
			innerObject = _obj
		};
	}

	bool hasIndex( State state, object idx )
	{
		// We don't support this yet.
		return false;
	}

	// Returns a method if it exists and is properly tagged; otherwise null.
	MethodInfo getTaggedMethod( string name )
	{
		var method = _t.GetMethod( name );
		if( method != null && method.GetCustomAttributes( typeof( CoralPassthroughAttribute ), true ).Length != 0 )
			return method;
		else
			return null;
	}

	// Returns a property if it exists and is properly tagged; otherwise null.
	PropertyInfo getTaggedProperty( string name )
	{
		var prop = _t.GetProperty( name );
		if( prop != null && prop.GetCustomAttributes( typeof( CoralPassthroughAttribute ), true ).Length != 0 )
			return prop;
		else
			return null;
	}

	bool hasMember( State state, string name )
	{
		if( getTaggedMethod( name ) != null )
			return true;

		if( getTaggedProperty( name ) != null )
			return true;

		return false;
	}

	void doIndexLookup( State state, object idx )
	{
		// Unreachable, presently.
	}

	object doMemberRead( State state, string name )
	{
		// Are we dealing with a method, a property, or nothing?
		MethodInfo m = getTaggedMethod( name );
		if( m != null )
		{
			return new FValue( (st, args) =>
				{
					// Make sure our arg count matches, or the method takes an object[].
					ParameterInfo[] ps = m.GetParameters();
					if( ps.Length == 1 && ps[0].ParameterType == typeof( object[] ) )
					{
						st.pushResult( m.Invoke( _obj, new object[] { args } ) );
						return;
					}

					if( ps.Length != args.Length )
						throw new ArgumentException( "Wrong number of arguments to call '{0}'".FormatI( name ) );

					object[] coerced = new object[args.Length];
					for( int i=0; i<args.Length; ++i )
						coerced[i] = Util.CoerceToDotNet( ps[i].ParameterType, args[i] );

					// If we're still trucking here, then we've got everything we need.
					// TODO: We'll eventually need to do exception filtering here too.
					object rv = m.Invoke( _obj, coerced );

					// Now we have to go the other way.
					st.pushResult( Util.CoerceFromDotNet( rv ) );
				}
			);
		}

		// Properties are considerably simpler.
		PropertyInfo p = getTaggedProperty( name );
		if( p != null )
		{
			object rv = p.GetValue( _obj, null );
			return Util.CoerceFromDotNet( rv );
		}

		// This really shouldn't happen.
		throw new InvalidOperationException( "Member doesn't exist (invalid)" );
	}

	void doMemberWrite( State state, string name, object val )
	{
		PropertyInfo p = getTaggedProperty( name );
		if( p != null )
		{
			object cv = Util.CoerceToDotNet( p.PropertyType, val );
			p.SetValue( _obj, cv, null );
		}
		else
		{
			// This really shouldn't happen.
			throw new InvalidOperationException( "Member doesn't exist (invalid)" );
		}
	}

	object _obj;
	Type _t;
}

}
