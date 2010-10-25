namespace Kayateia.Climoo.Scripting.SSharp {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptNET.Runtime;

/// <summary>
/// Represents a successful binding to a member in an IDynamicObject.
/// </summary>
internal class DynamicMemberBinding : IMemberBind {
	public DynamicMemberBinding(IDynamicObject target, string name) {
	}

	public object Target {
		get { throw new NotImplementedException(); }
	}

	public Type TargetType {
		get { throw new NotImplementedException(); }
	}

	public System.Reflection.MemberInfo Member {
		get { throw new NotImplementedException(); }
	}

	public void SetValue(object value) {
		throw new NotImplementedException();
	}

	public object GetValue() {
		throw new NotImplementedException();
	}

	public void AddHandler(object value) {
		throw new NotImplementedException();
	}

	public void RemoveHandler(object value) {
		throw new NotImplementedException();
	}

	public bool CanInvoke() {
		throw new NotImplementedException();
	}

	public object Invoke(IScriptContext context, object[] args) {
		throw new NotImplementedException();
	}
}

}
