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
using ScriptNET.Runtime;

/// <summary>
/// Represents a binding of a method on an object in script code.
/// </summary>
internal class DynamicMethodBinding : IObjectBind {
	public DynamicMethodBinding(IDynamicObject target, string name) {
		_target = target;
		_name = name;
	}

	public bool CanInvoke() {
		return _target.hasMethod(_name);
	}

	public object Invoke(IScriptContext context, object[] args) {
		return _target.callMethod(new Scope(context), _name, args);
	}

	IDynamicObject _target;
	string _name;
}

}
