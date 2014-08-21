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

/// <summary>
/// A scope is a container for program state information like active variable values.
/// </summary>
public class Scope
{
	public Scope()
	{
	}

	/// <summary>
	/// Gets a variable value by name.
	/// </summary>
	public object get( string name )
	{
		object v;
		if( _values.TryGetValue( name, out v ) )
			return v;
		else
			throw new ArgumentException( "Undefined variable " + name );
	}

	/// <summary>
	/// Sets a variable value by name.
	/// </summary>
	public void set( string name, object value )
	{
		_values[name] = value;
	}

	/// <summary>
	/// Deletes a variable from the scope by name.
	/// </summary>
	public void delete( string name )
	{
		_values.Remove( name );
	}

	Dictionary<string, object> _values = new Dictionary<string,object>();
}

}
