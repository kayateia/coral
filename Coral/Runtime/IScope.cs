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

/// <summary>
/// A scope is a container for program state information like active variable values.
/// These can act hierarchically; they can be chained, and the available values are
/// pulled from the highest up in the chain.
/// </summary>
public interface IScope
{
	/// <summary>
	/// Gets a variable value by name.
	/// </summary>
	object get( string name );

	/// <summary>
	/// Checks to see if a variable is set.
	/// </summary>
	bool has( string name );

	/// <summary>
	/// Sets a variable value by name.
	/// </summary>
	void set( string name, object value );

	/// <summary>
	/// Deletes a variable from the scope by name.
	/// </summary>
	void delete( string name );
}

}
