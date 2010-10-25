namespace Kayateia.Climoo.Scripting.SSharp {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptNET.Runtime;

internal class DynamicMethodBinding : IObjectBind {
	public DynamicMethodBinding(IDynamicObject target, string name) {
	}

	public bool CanInvoke() {
		throw new NotImplementedException();
	}

	public object Invoke(IScriptContext context, object[] args) {
		throw new NotImplementedException();
	}
}

}
