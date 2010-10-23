namespace Kayateia.Climoo.Scripting.Ruby {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Hosting;

/// <summary>
/// Represents a fully isolated scripting context. Each logged-in user will get
/// one of these to themselves, as a last-resort effort to prevent
/// cross-sandbox contamination.
/// </summary>
public class Context : IDisposable {
	internal Context(AppDomain domain, ScriptEngine engine) {
		_domain = domain;
		_engine = engine;
	}

	public void Dispose() {
		if (_domain != null) {
			AppDomain.Unload(_domain);
			_domain = null;
		}
	}

	public ScriptSource loadScriptFromFile(string filename) {
		return _engine.CreateScriptSourceFromFile(filename);
	}

	public ScriptSource loadScriptFromString(string code) {
		return _engine.CreateScriptSourceFromFile(code);
	}

	public AppDomain appDomain { get { return _domain; } }

	AppDomain _domain;
	ScriptEngine _engine;
}

}
