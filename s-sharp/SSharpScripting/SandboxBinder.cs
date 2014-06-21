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
		var dyn = target as IDynamicObject;
		if (dyn != null) {
			if (!dyn.isMethodPassthrough(methodName)) {
				if (genericParameters != null && genericParameters.Length > 0)
					return null;
				if (!dyn.hasMethod(methodName))
					return null;

				return new DynamicMethodBinding(dyn, methodName);
			}
		}

		// Make sure it's on our whitelist before we allow access at all.
		if (!allowedToBind(target, methodName))
			return null;

		// Pass it down to the original reflection binder.
		return _orig.BindToMethod(target, methodName, genericParameters, arguments);
	}

	public IObjectBind BindToMethod(object target, MethodInfo method, object[] arguments) {
		var dyn = target as IDynamicObject;
		if (dyn != null && !dyn.isMethodPassthrough(method.Name))
			return null;

		// Make sure it's on our whitelist before we allow access at all.
		if (!allowedToBind(target, method.Name))
			return null;

		// Pass it down to the original reflection binder.
		return _orig.BindToMethod(target, method, arguments);
	}

	public IMemberBind BindToMember(object target, string memberName, bool throwNotFound) {
		var dyn = target as IDynamicObject;
		if (dyn != null && !dyn.isMemberPassthrough(memberName))
			return new DynamicMemberBinding(dyn, memberName);

		// Make sure it's on our whitelist before we allow access at all.
		if (!allowedToBind(target, memberName))
			return null;

		// Pass it down to the original reflection binder.
		return _orig.BindToMember(target, memberName, throwNotFound);
	}

	public IObjectBind BindToIndex(object target, object[] arguments, bool setter) {
		if (target is IDynamicObject)
			return null;
		else {
			// Make sure it's on our whitelist before we allow access at all.
			if (!allowedToBind(target, null))
				return null;

			return _orig.BindToIndex(target, arguments, setter);
		}
	}

	public object ConvertTo(object value, Type targetType) {
		if (value is IDynamicObject)
			return null;
		else {
			// Make sure it's on our whitelist before we allow access at all.
			if (!allowedToBind(value, null))
				return null;

			return _orig.ConvertTo(value, targetType);
		}
	}

	bool allowedToBind(object target, string name) {
		// System.Type is always bindable because the ScriptNET runtime
		// default binder pulls these out as static method calls anyway.
		if (target is System.Type)
			return true;

		// IDynamicObject always gets a pass too.
		if (target is IDynamicObject)
			return true;

		// As do arrays -- whatever is in them, it will go through the binder itself.
		if (target is System.Array)
			return true;

		// Find the type name.
		string typename = target.GetType().FullName;

		return RuntimeHost.AssemblyManager.HasType(typename);
	}

	public bool CanBind(MemberInfo member) {
		return _orig.CanBind(member);
	}
}

}
