namespace Kayateia.Climoo.Scripting.SSharp {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ScriptNET;
using ScriptNET.Runtime;

/// <summary>
/// Sandbox binder for S#.
/// </summary>
/// <remarks>
/// This is a custom binder built for our sandbox. The primary thing it allows
/// us to do over the original one is to allow us to implement some 'dynamic'
/// style objects whose member accesses translate directly into MOO manipulations
/// outside the sandbox immediately.
/// 
/// Anything that's an IDynamicObject, we take over. Everything else we pass on.
/// Presumably the assembly sandbox already caught references we don't want.
/// </remarks>
public class SandboxBinder : IObjectBinder {
	public SandboxBinder(IObjectBinder orig) {
		_orig = orig;
	}
	IObjectBinder _orig;

	public IObjectBind BindToConstructor(Type target, object[] arguments) {
		return _orig.BindToConstructor(target, arguments);
	}

	public IObjectBind BindToMethod(object target, string methodName, Type[] genericParameters, object[] arguments) {
		if (target is IDynamicObject) {
			if (genericParameters != null && genericParameters.Length > 0)
				return null;
			var dyn = target as IDynamicObject;
			if (!dyn.hasMethod(methodName, arguments))
				return null;

			return new DynamicMethodBinding(dyn, methodName);
		}
		return _orig.BindToMethod(target, methodName, genericParameters, arguments);
	}

	public IObjectBind BindToMethod(object target, MethodInfo method, object[] arguments) {
		if (target is IDynamicObject)
			return null;
		return _orig.BindToMethod(target, method, arguments);
	}

	public IMemberBind BindToMember(object target, string memberName, bool throwNotFound) {
		if (target is IDynamicObject) {
			var dyn = target as IDynamicObject;
			if (!dyn.hasMember(memberName)) {
				if (throwNotFound)
					throw new DynamicObjectFailure("Can't find member", memberName);
				else
					return null;
			}
			return new DynamicMemberBinding(dyn, memberName);
		} else
			return _orig.BindToMember(target, memberName, throwNotFound);
	}

	public IObjectBind BindToIndex(object target, object[] arguments, bool setter) {
		if (target is IDynamicObject)
			return null;
		else
			return _orig.BindToIndex(target, arguments, setter);
	}

	public object ConvertTo(object value, Type targetType) {
		if (value is IDynamicObject)
			return null;
		else
			return _orig.ConvertTo(value, targetType);
	}

	public bool CanBind(MemberInfo member) {
		return _orig.CanBind(member);
	}
}

}
