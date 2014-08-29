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
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Misc utility methods used thoroughout the code.
/// </summary>
static public class Util
{
	/// <summary>
	/// Handles array slice logic.
	/// </summary>
	static public void ArraySlice( int length, int? begin, int? end, out int beginOut, out int endOut )
	{
		int abegin = begin ?? 0;
		if( abegin < 0 )
			abegin = 0;

		int aend = end ?? length;
		if( aend < 0 )
			aend = length + aend;
		if( aend > length )
			aend = length;

		beginOut = abegin;
		endOut = aend;
	}

	/// <summary>
	/// Coerce a value into a boolean.
	/// </summary>
	static public bool CoerceBool( object o )
	{
		if( o is int )
			return ((int)o) != 0;
		if( o is bool )
			return (bool)o;
		if( o is string )
			return !((string)o).IsNullOrEmpty();
		return o != null;
	}

	/// <summary>
	/// Coerce a value into a number.
	/// </summary>
	static public int CoerceNumber( object o )
	{
		if( o is int )
			return (int)o;
		if( o is bool )
			return ((bool)o) ? 1 : 0;
		if( o is string )
			return int.Parse( (string)o, CultureFree.Culture );

		throw CoralException.GetArg( "Can't coerce value into a number" );
	}

	/// <summary>
	/// Coerce a value into a string.
	/// </summary>
	static public string CoerceString( object o )
	{
		if( o is string )
			return (string)o;
		else
			return o.ToStringI();
	}

	/// <summary>
	/// Coerce a value into a string array.
	/// </summary>
	static public string[] CoerceStringArray( object o )
	{
		var list = o as List<object>;
		if( list == null )
			throw CoralException.GetArg( "Can't coerce value into an array" );

		return list.Select( s => CoerceString( s ) ).ToArray();
	}

	/// <summary>
	/// Coerce a string array into a Coral string array.
	/// </summary>
	static public object CoerceStringListObject( string[] strs )
	{
		return new List<object>( strs );
	}

	/// <summary>
	/// Converts a Coral type to a .NET type.
	/// </summary>
	/// <param name="netType">The target .NET type</param>
	/// <param name="value">The value</param>
	/// <returns>The coerced value</returns>
	static public object CoerceToDotNet( Type netType, object value )
	{
		object rv = null;
		if( netType == typeof( int ) )
			rv = Util.CoerceNumber( value );
		else if( netType == typeof( string ) )
			rv = Util.CoerceString( value );
		else if( netType == typeof( string[] ) )
			rv = Util.CoerceStringArray( value );
		else if( netType == typeof( bool ) )
			rv = Util.CoerceBool( value );
		else
		{
			// Some custom type. All we can really do here is see if the arg is another
			// passthrough object, and pull its innards out if it's the right type.
			var pmo = value as Passthrough.PassthroughMetalObject;
			if( pmo != null )
			{
				if( pmo.innerObject.GetType() == netType )
					rv = pmo.innerObject;
			}
			else if( netType == typeof( object ) )
				rv = value;
		}

		if( rv == null && value != null )
			throw CoralException.GetArg( "Argument '{0}' can't be passed to this method/property".FormatI( value ) );
		else
			return rv;
	}

	/// <summary>
	/// Converts a .NET type to a Coral type.
	/// </summary>
	static public object CoerceFromDotNet( object value )
	{
		if( value == null )
			return null;
		else
		{
			Type rvt = value.GetType();
			if( rvt == typeof( int ) || rvt == typeof( string ) || rvt == typeof( bool ) )
				return value;
			else if( value is Array )
			{
				var rv = new List<object>();
				foreach( var v in (Array)value )
					rv.Add( CoerceFromDotNet( v ) );
				return rv;
			}
			else
			{
				Passthrough pt = new Passthrough( value );
				return pt.getObject( "<anon>" );
			}
		}
	}
}

}
