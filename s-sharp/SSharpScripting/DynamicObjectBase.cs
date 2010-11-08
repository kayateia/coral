namespace Kayateia.Climoo.Scripting.SSharp {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

/// <summary>
/// Base object that implements the dynamic object interface and
/// simplifies its use in a number of ways.
/// </summary>
public abstract class DynamicObjectBase : IDynamicObject {
	public DynamicObjectBase() {
	}

	public class PassthroughAttribute : System.Attribute { }

	public virtual bool isMemberPassthrough(string name) {
		if (_memberPassthrough == null) {
			_memberPassthrough = new List<string>();

			Type t = this.GetType();
			var flags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public;

			FieldInfo[] fields = t.GetFields(flags);
			foreach (var f in fields)
				if (f.GetCustomAttributes(typeof(PassthroughAttribute), true).Length > 0)
					_memberPassthrough.Add(f.Name);

			PropertyInfo[] props = t.GetProperties(flags);
			foreach (var p in props)
				if (p.GetCustomAttributes(typeof(PassthroughAttribute), true).Length > 0)
					_memberPassthrough.Add(p.Name);
		}

		return _memberPassthrough.Contains(name);
	}
	List<string> _memberPassthrough = null;

	public virtual object getMember(string name) { return null; }
	public virtual string getMimeType(string name) { return null; }
	public virtual bool hasMember(string name) { return false; }

	// This can be overridden to add more (dynamic) member names.
	public virtual IEnumerable<string> getMemberNames() {
		return _memberPassthrough;
	}

	public virtual void setMember(string name, object val) { throw new DynamicObjectFailure("No member 'set' capability available."); }
	public virtual void setMimeType(string name, string type) { throw new DynamicObjectFailure("No member 'set' capability available."); }

	public virtual bool hasMethod(string name) { return false; }
	public virtual object callMethod(Scope scope, string name, object[] args) {
		throw new DynamicObjectFailure("No matching methods are available.");
	}

	public virtual bool isMethodPassthrough(string name) {
		if (_methodPassthrough == null) {
			_methodPassthrough = new List<string>();

			Type t = this.GetType();
			var flags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public;

			MethodInfo[] meths = t.GetMethods(flags);
			foreach (var m in meths)
				if (m.GetCustomAttributes(typeof(PassthroughAttribute), true).Length > 0)
					_methodPassthrough.Add(m.Name);
		}

		return _methodPassthrough.Contains(name);
	}
	List<string> _methodPassthrough = null;
}

}
