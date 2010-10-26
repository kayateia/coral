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
		_context = new ScriptContext();
		RuntimeHost.InitializeScript(_context);
	}

	internal Scope(IScriptContext context) {
		_context = context;
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

	internal IScriptContext context {
		get { return _context; }
	}

	IScriptContext _context;
}

}
