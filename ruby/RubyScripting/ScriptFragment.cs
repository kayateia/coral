namespace Kayateia.Climoo.Scripting.Ruby {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Hosting;

public class ScriptFragment {
	internal ScriptFragment(AppDomain domain, ScriptSource script) {
		_domain = domain;
		_script = script;
	}

	public AppDomain appDomain { get { return _domain; } }

	AppDomain _domain;
	ScriptSource _script;
}

}
