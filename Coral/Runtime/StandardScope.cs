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
/// Implementation of IScope that implements a simple nesting scope. This is
/// suitable for the majority of needs.
/// </summary>
public class StandardScope : IScope
{
	public StandardScope()
	{
		_parent = null;
	}

	public StandardScope( IScope parent )
	{
		_parent = parent;
	}

	/// <summary>
	/// Gets a variable value by name.
	/// </summary>
	public object get( string name )
	{
		object v;

		// Try our local store first.
		if( _values.TryGetValue( name, out v ) )
			return v;

		// If that fails, try any parent store.
		if( _parent != null )
		{
			v = _parent.get( name );
			if( v != null )
				return v;
		}

		// Invalid variable.
		throw new ArgumentException( "Undefined variable " + name );
	}

	/// <summary>
	/// Checks to see if a variable is set.
	/// </summary>
	public bool has( string name )
	{
		if( _values.ContainsKey( name ) )
			return true;
		else if( _parent != null )
			return _parent.has( name );
		else
			return false;
	}

	/// <summary>
	/// Sets a variable value by name.
	/// </summary>
	public void set( string name, object value )
	{
		if( _parent == null || !_parent.has( name ) )
			_values[name] = value;
		else
			_parent.set( name, value );
	}

	/// <summary>
	/// Deletes a variable from the scope by name.
	/// </summary>
	public void delete( string name )
	{
		if( _values.ContainsKey( name ) )
			_values.Remove( name );
		else
		{
			if( _parent != null )
				_parent.delete( name );
		}
	}

	public string[] getNames()
	{
		string[] parentNames = _parent != null ? _parent.getNames() : new string[0];
		return _values.Keys.Union( parentNames ).ToArray();
	}

	IScope _parent;
	Dictionary<string, object> _values = new Dictionary<string,object>();
}

}
