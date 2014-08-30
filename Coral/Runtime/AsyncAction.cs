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
using System.Text;

/// <summary>
/// Return from a native method to cause further actions to be taken inside
/// the Coral interpreter.
/// </summary>
public class AsyncAction
{
	public enum Action
	{
		/// <summary>
		/// Exit the interpreter immediately, leaving everything suspended in place.
		/// </summary>
		Exit,

		/// <summary>
		/// Make a function call with the specified parameters, passing its return
		/// value back to whatever called this method.
		/// </summary>
		Call,

		/// <summary>
		/// Set a variable in the current scope.
		/// </summary>
		Variable,

		/// <summary>
		/// Execute a CodeFragment in the current scope.
		/// </summary>
		Code,

		/// <summary>
		/// Execute a callback.
		/// </summary>
		Callback
	}

	/// <summary>
	/// The action to be taken.
	/// </summary>
	public Action action { get; set; }

	/// <summary>
	/// Function or variable name, for Call or Variable.
	/// </summary>
	public string name { get; set; }

	/// <summary>
	/// Function call parameters, for Call.
	/// </summary>
	public object[] args { get; set; }

	/// <summary>
	/// Variable value, for Variable.
	/// </summary>
	public object value { get; set; }

	/// <summary>
	/// Code fragment, for Code.
	/// </summary>
	public CodeFragment code { get; set; }

	/// <summary>
	/// Callback for Callback.
	/// </summary>
	public Action<State> callback { get; set; }

	/// <summary>
	/// A function, for Call.
	/// </summary>
	public FValue function { get; set; }
}

}
