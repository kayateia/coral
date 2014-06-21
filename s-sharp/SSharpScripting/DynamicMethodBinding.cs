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
