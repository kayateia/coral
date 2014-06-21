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
/// Thin subclass of ScriptContext that lets us associate non-S#-accessible
/// items from a later method call.
/// </summary>
public class ContextWithBaggage : ScriptContext {
	public ContextWithBaggage() { }

	public void baggageSet(string item, object val) {
		_baggage[item] = val;
	}

	public object baggageGet(string item) {
		return _baggage[item];
	}

	public void baggageDel(string item) {
		_baggage.Remove(item);
	}

	// Give a chance to our own code to make algorithmically named scope items.
	public override object GetItem(string id, bool throwException) {
		if (this.queryForItem != null) {
			var rv = this.queryForItem(id);
			if (rv != null)
				return rv;
		}
		return base.GetItem(id, throwException);
	}

	public Scope.GetItemDelegate queryForItem = null;

	Dictionary<string, object> _baggage = new Dictionary<string,object>();
}

}
