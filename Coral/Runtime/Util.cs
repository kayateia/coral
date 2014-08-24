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
			return ((string)o).IsNullOrEmpty();
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

		throw new ArgumentException( "Can't coerce value into a number" );
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
			throw new ArgumentException( "Can't coerce value into an array" );

		return list.Select( s => CoerceString( s ) ).ToArray();
	}

	/// <summary>
	/// Coerce a string array into a Coral string array.
	/// </summary>
	static public object CoerceStringListObject( string[] strs )
	{
		return new List<object>( strs );
	}
}

}
