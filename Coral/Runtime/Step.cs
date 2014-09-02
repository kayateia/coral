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
/// Lambda type for a continuation that will be called when the step is reached.
/// </summary>
public delegate void ActionCallback( State s );

/// <summary>
/// Represents a step pushed onto the continuation stack.
/// </summary>
public class Step
{
	public Step( AstNode n, ActionCallback a, string d = null )
	{
		this.node = n;
		this.action = a;
		this.description = d;
		if( this.description == null )
			this.description = n.ToString();
	}

	/// <summary>
	/// The AstNode associated with this step, if any. This will be used for
	/// line number and other context info.
	/// </summary>
	public AstNode node { get; set; }

	/// <summary>
	/// The actual callback lambda to be called when this step is reached.
	/// </summary>
	public ActionCallback action { get; set; }

	/// <summary>
	/// Description of the step. This will be filled from AstNode if not passed in.
	/// </summary>
	public string description { get; set; }

	/// <summary>
	/// If this is non-null, then any steps pushed on the action stack below this
	/// one should make use of the scope here rather than the global one. This allows
	/// us to specify nested scopes speculatively in the same way we do steps to execute.
	/// </summary>
	public IScope scope { get; set; }

	/// <summary>
	/// If this is non-null, then any steps pushed on the action stack below this
	/// one should make use of the security context here rather than any previous one.
	/// This allows us to specify nested security contexts speculatively in the same way
	/// we do steps to execute.
	/// </summary>
	public ISecurityContext securityContext { get; set; }

	public override string ToString()
	{
		return this.description;
	}
}

}
