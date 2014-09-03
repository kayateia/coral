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
/// Implementation of IScope that calls out to something else to do its lookups.
/// This goes in the middle of a stack.
/// </summary>
public class LookupScope : IScope
{
	public LookupScope( IScope parent )
	{
		_parent = parent;
	}

	public delegate object LookupDelegate( string name );

	public LookupDelegate lookup { get; set; }

	public object get( string name )
	{
		if( this.lookup == null )
			return _parent.get( name );

		object v = this.lookup( name );
		if( v != null )
			return Util.CoerceFromDotNet( v );

		return _parent.get( name );
	}

	public bool has( string name )
	{
		if( this.lookup == null )
			return _parent.has( name );

		object v = this.lookup( name );
		if( v != null )
			return true;

		return _parent.has( name );
	}

	public void set( string name, object value )
	{
		_parent.set( name, value );
	}

	public void delete( string name )
	{
		_parent.delete( name );
	}

	public string[] getNames()
	{
		return _parent.getNames();
	}

	IScope _parent;
}

}
