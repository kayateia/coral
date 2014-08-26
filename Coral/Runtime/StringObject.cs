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
/// Handles various OOP aspects of strings in Coral.
/// </summary>
static public class StringObject
{
	/// <summary>
	/// Handles various method calls on string objects.
	/// </summary>
	static public FValue Method( State state, string str, string name )
	{
		if( name == "length" )
		{
			return new FValue( (st2, args) =>
				{
					st2.pushResult( str.Length );
				}
			);
		}
		else if( name == "format" )
		{
			return new FValue( (st2, args) =>
				{
					string result = str.FormatI( args );
					st2.pushResult( result );
				}
			);
		}
		else if( name == "split" )
		{
			return new FValue( (st2, args) =>
				{
					// We should have 1 or 2 args.
					if( args.Length < 1 )
						throw new ArgumentException( "Not enough args to string.split" );

					// Convert the split-by array if necessary.
					string[] splitBy;
					if( args[0] is string )
						splitBy = new string[] { Util.CoerceString( args[0] ) };
					else
						splitBy = Util.CoerceStringArray( args[0] );

					if( args.Length == 1 )
						st2.pushResult( Util.CoerceStringListObject( str.Split( splitBy, StringSplitOptions.None ) ) );
					else
						st2.pushResult( Util.CoerceStringListObject( str.Split( splitBy, Util.CoerceNumber( args[1] ), StringSplitOptions.None ) ) );
				}
			);
		}
		else if( name == "replace" )
		{
			return new FValue( (st2, args) =>
				{
					if( args.Length != 2 )
						throw new ArgumentException( "Must pass replace() two args" );

					string from = Util.CoerceString( args[0] );
					string to = Util.CoerceString( args[1] );

					st2.pushResult( str.Replace( from, to ) );
				}
			);
		}
		else
			throw new ArgumentException( "String object has no method '{0}'".FormatI( name ) );
	}

	/// <summary>
	/// Array access on a string. Yes, this is a bit of overkill, but we may want
	/// to hook into this process later.
	/// </summary>
	static public string ArrayAccess( string str, int index )
	{
		return str[index] + "";
	}

	/// <summary>
	/// Array slices on a string.
	/// </summary>
	static public string ArraySlice( string str, int? start, int? end )
	{
		int astart, aend;
		Util.ArraySlice( str.Length, start, end, out astart, out aend );

		return str.Substring( astart, aend - astart );
	}

	/// <summary>
	/// Implements the string.join method.
	/// </summary>
	static void MethodJoin( State state, object[] args )
	{
		// Args should be a separator string and an array of stuff to join.
		if( args.Length != 2 )
			throw new ArgumentException( "string.join must be called with a separator string and an array of strings" );

		string sep = Util.CoerceString( args[0] );
		string[] arr = Util.CoerceStringArray( args[1] );
		state.pushResult( String.Join( sep, arr ) );
	}

	/// <summary>
	/// Registers the string object in the const scope.
	/// </summary>
	/// <remarks>
	/// This could probably be factored out into a helper class.
	/// </remarks>
	static public void RegisterObject( ConstScope scope )
	{
		scope.setConstant( "string",
			new MetalObject()
			{
				indexLookup = (state, idx) =>
				{
					throw new InvalidOperationException( "Can't index the string object" );
				},
				memberLookup = (state, name) =>
				{
					state.pushResult( new LValue()
					{
						read = st => {
							if( name == "join" )
							{
								return new FValue( (st2,args) => MethodJoin( st2, args ) );
							}
							else
							{
								throw new ArgumentException( "Unknown method on string object" );
							}
						},
						write = (st,val) => { throw new InvalidOperationException( "Can't write to the string object" ); }
					} );
				}
			}
		);
	}
}

}
