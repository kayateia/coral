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

/// <summary>
/// Implementation of IScope that implements a fixed set of variables,
/// suitable for parameters and for loop variables and such.
/// </summary>
public class ParameterScope : IScope
{
	public ParameterScope( IScope parent, string[] parameters )
	{
		_parent = parent;
		_contents = new StandardScope();
		foreach( string p in parameters )
			_contents.set( p, null );
	}

	/// <summary>
	/// Gets a variable value by name.
	/// </summary>
	public object get( string name )
	{
		if( _contents.has( name ) )
			return _contents.get( name );
		else
			return _parent.get( name );
	}

	/// <summary>
	/// Checks to see if a variable is set.
	/// </summary>
	public bool has( string name )
	{
		if( _contents.has( name ) )
			return true;
		else
			return _parent.has( name );
	}

	/// <summary>
	/// Sets a variable value by name.
	/// </summary>
	public void set( string name, object value )
	{
		if( _contents.has( name ) )
			_contents.set( name, value );
		else
			_parent.set( name, value );
	}

	/// <summary>
	/// Deletes a variable from the scope by name.
	/// </summary>
	public void delete( string name )
	{
		_parent.delete( name );
	}

	IScope _parent;
	StandardScope _contents;
}

}
