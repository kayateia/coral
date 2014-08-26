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
/// Extensible objects are used by the Passthrough class to handle get/set
/// on an object in regards to "undeclared" properties.
/// </summary>
public interface IExtensible
{
	/// <summary>
	/// Gets a property value by name.
	/// </summary>
	object getProperty( State state, string name );

	/// <summary>
	/// Checks to see if a property is available.
	/// </summary>
	bool hasProperty( State state, string name );

	/// <summary>
	/// Sets a property value by name.
	/// </summary>
	void setProperty( State state, string name, object value );

	/// <summary>
	/// Gets a property value by name.
	/// </summary>
	object callMethod( State state, string name, object[] args );

	/// <summary>
	/// Checks to see if a property is available.
	/// </summary>
	bool hasMethod( State state, string name );
}

}
