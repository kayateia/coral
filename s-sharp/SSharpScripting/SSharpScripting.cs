namespace Kayateia.Climoo.Scripting.SSharp {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptNET;
using ScriptNET.Runtime;

public class SSharpScripting {
	/// <summary>
	/// Initialize the S# scripting runtime.
	/// </summary>
	/// <remarks>This only needs to be done once per process.</remarks>
	static public void Init() {
		// Do basic init.
		RuntimeHost.Initialize();

		// Replace the assembly manager with our own, which provides nearly nothing.
		RuntimeHost.AssemblyManager = new SandboxAssemblyManager(RuntimeHost.AssemblyManager);

		// Replace the binder with our own, which allows using MOO objects as 'dynamic'
		// style objects, and so forth.
		RuntimeHost.Binder = new SandboxBinder(RuntimeHost.Binder);

		Script s = Script.Compile("1+1");
		System.Diagnostics.Debug.WriteLine(s.Execute());
	}
}

}
