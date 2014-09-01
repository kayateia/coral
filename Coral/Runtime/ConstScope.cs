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
/// Implementation of IScope that implements a constant scope. This is for read-only
/// compiler/runtime constants that scripts can use, but which can't be modified. One
/// of these should be at the root.
/// </summary>
public class ConstScope : IScope
{
	public ConstScope( IScope parent )
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

		// Check parents, if any.
		if( _parent != null )
		{
			v = _parent.get( name );
			if( v != null )
				return v;
		}

		// Invalid variable. Since we'll be at the bottom, we need to handle this.
		return null;
	}

	public bool has( string name )
	{
		return _values.ContainsKey( name ) || (_parent != null && _parent.has( name ));
	}

	public void set( string name, object value )
	{
		if( _parent == null || _values.ContainsKey( name ) )
			throw CoralException.GetInvOp( "Can't set a Coral constant" );
		else if( _parent != null )
			_parent.set( name, value );
	}

	public void delete( string name )
	{
		if( _parent == null || _values.ContainsKey( name ) )
			throw CoralException.GetInvOp( "Can't delete a Coral constant" );
		else if( _parent != null )
			_parent.delete( name );
	}

	public void setConstant( string name, object value )
	{
		_values[name] = value;
	}

	public string[] getNames()
	{
		string[] parentNames = _parent != null ? _parent.getNames() : new string[0];
		return _values.Keys.Union( parentNames ).ToArray();
	}

	Dictionary<string, object> _values = new Dictionary<string,object>();
	IScope _parent;
}

}
