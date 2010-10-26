namespace Kayateia.Climoo.Scripting.SSharp {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptNET;

/// <summary>
/// Represents a scripting fragment that runs within the MOO sandbox context.
/// </summary>
public class ScriptFragment {
	public ScriptFragment() { }
	public ScriptFragment(string code) {
		this.code = code;
	}

	public string code {
		get { return _code; }
		set {
			_code = value;
			_compiled = Script.Compile(_code);
		}
	}
	string _code;

	public object execute(Scope scope) {
		Script recontexted =_compiled.DuplicateWithNewContext(scope.context);
		return recontexted.Execute();
	}

	Script _compiled;
}

}
