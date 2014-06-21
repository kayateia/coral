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

namespace Kayateia.Climoo.Scripting.SSharp {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Thrown whenever a dynamic object operation fails.
/// </summary>
public class DynamicObjectFailure : System.Exception {
	public DynamicObjectFailure(string err)
		: base(err)
	{
	}

	public DynamicObjectFailure(string err, string name)
		: base(err)
	{
		this.name = name;
	}

	public string name { get; private set; }
}

/// <summary>
/// Objects can implement this to get dynamic binding in scripts. This means
/// that instead of attempting to bind through reflection, these methods will
/// be called for the matching functionality.
/// </summary>
public interface IDynamicObject {
	/// <summary>
	/// Returns true if requests for this member should be passed on to the
	/// default scripting binder (i.e. accessed as a .NET member).
	/// </summary>
	bool isMemberPassthrough(string name);

	/// <summary>
	/// Returns the member variable with the given name.
	/// </summary>
	/// <exception cref="DynamicObjectFailure">If the member can't be found.</exception>
	object getMember(string name);

	/// <summary>
	/// Returns the MIME type of the variable with the given name.
	/// </summary>
	/// <exception cref="DynamicObjectFailure">If the member can't be found.</exception>
	string getMimeType(string name);

	/// <summary>
	/// Returns true if getMember(name) would succeed.
	/// </summary>
	bool hasMember(string name);

	/// <summary>
	/// Returns a list of existing member names.
	/// </summary>
	/// <exception cref="DynamicObjectFailure">If the member names can't be enumerated.</exception>
	IEnumerable<string> getMemberNames();

	/// <summary>
	/// Sets a member value.
	/// </summary>
	/// <param name="name">Member name</param>
	/// <param name="val">New value</param>
	/// <exception cref="DynamicObjectFailure">If the member can't be set.</exception>
	void setMember(string name, object val);

	/// <summary>
	/// Sets a MIME type.
	/// </summary>
	/// <param name="name">Member name</param>
	/// <param name="type">New value</param>
	/// <exception cref="DynamicObjectFailure">If the member can't be set.</exception>
	void setMimeType(string name, string type);

	/// <summary>
	/// Returns true if callMethod(name) would succeed.
	/// </summary>
	bool hasMethod(string name);

	/// <summary>
	/// Returns true if requests for this method should be passed on to the
	/// default scripting binder (i.e. accessed as a .NET method).
	/// </summary>
	bool isMethodPassthrough(string name);

	/// <summary>
	/// Calls a method.
	/// </summary>
	/// <param name="scope">Scripting scope</param>
	/// <param name="name">Method name</param>
	/// <param name="args">Arguments to the method</param>
	/// <returns>The return value of the method</returns>
	/// <exception cref="DynamicObjectFailure">If the method can't be called.</exception>
	object callMethod(Scope scope, string name, object[] args);
}

}
