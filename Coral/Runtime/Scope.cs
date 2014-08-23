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
/// These act hierarchically; they can be chained, and the available values are
/// pulled from the highest up in the chain. Deletion only happens locally, however,
/// so deleting a variable will uncover any previous value.
/// </summary>
public class Scope
{
	public Scope()
	{
		_parent = null;
		this.fixedSet = false;
	}

	public Scope( Scope parent )
	{
		_parent = parent;
		this.fixedSet = false;
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
		if( _values.ContainsKey( name ) )
		{
			_values[name] = value;
		}
		else if( ( this.fixedSet && _parent != null ) || ( _parent != null && _parent.has( name ) ) )
		{
			_parent.set( name, value );
		}
		else
			_values[name] = value;
	}

	/// <summary>
	/// Deletes a variable from the scope by name.
	/// </summary>
	public void delete( string name )
	{
		if( !_values.ContainsKey( name ) && this.fixedSet && _parent != null )
			_parent.delete( name );
		else
			_values.Remove( name );
	}

	/// <summary>
	/// If true, new variables set on this scope will be pushed down to any parent
	/// scope if one is available.
	/// </summary>
	/// <remarks>
	/// This use of this is in very limited-scope scopes, for things like the loop
	/// variables in a for loop.
	/// </remarks>
	public bool fixedSet { get; set; }

	Scope _parent;
	Dictionary<string, object> _values = new Dictionary<string,object>();
}

}
