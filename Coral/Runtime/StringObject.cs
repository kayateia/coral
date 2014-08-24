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
					for( int i=0; i<args.Length; ++i )
						args[i] = LValue.Deref( st2, args[i] );
					string result = str.FormatI( args );
					st2.pushResult( result );
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
}

}
