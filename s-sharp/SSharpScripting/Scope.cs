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
/// Represents a script scope. Anything that's not in this when the script
/// executes, the script isn't going to have access to. The exceptions are
/// the globally-set system (and MOO) assembly allowances.
/// </summary>
public class Scope {
	public Scope() {
		_context = new ContextWithBaggage();
		RuntimeHost.InitializeScript(_context);
	}

	internal Scope(IScriptContext context) {
		if (!(context is ContextWithBaggage))
			throw new ArgumentException("Context is not one of ours");
		_context = (ContextWithBaggage)context;
	}

	public void set(string name, object value) {
		_context.SetItem(name, value);
	}

	public void remove(string name) {
		// Not sure if there's a better way or not...
		_context.SetItem(name, null);
	}

	public object get(string name) {
		return _context.GetItem(name, false);
	}

	public void baggageSet(string name, object value) {
		_context.baggageSet(name, value);
	}

	public object baggageGet(string name) {
		return _context.baggageGet(name);
	}

	public void baggageDel(string name) {
		_context.baggageDel(name);
	}

	/// <summary>
	/// Called whenever we want to query for a scoped item, to give a
	/// callback the first right of refusal on the name lookup.
	/// </summary>
	public GetItemDelegate queryForItem {
		get {
			return _context.queryForItem;
		}
		set {
			_context.queryForItem = value;
		}
	}

	public delegate object GetItemDelegate(string id);

	internal ContextWithBaggage context {
		get { return _context; }
	}

	ContextWithBaggage _context;
}

}
