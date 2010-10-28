namespace Kayateia.Climoo.Scripting.SSharp {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ScriptNET;
using ScriptNET.Runtime;

/// <summary>
/// This custom assembly manager will allow us to have much finer grain control
/// over both assembly loading and object finding (for static methods). Many
/// calls are just passed down to the original assembly manager.
/// </summary>
public class SandboxAssemblyManager : IAssemblyManager {
	public SandboxAssemblyManager(IAssemblyManager orig) {
		_orig = orig;
		_orig.BeforeAddAssembly += (sender, args) => this.BeforeAddAssembly(sender, args);
		_orig.BeforeAddType += (sender, args) => this.BeforeAddType(sender, args);
	}
	IAssemblyManager _orig;

	public void Initialize(ScriptNET.Runtime.Configuration.ScriptConfiguration configuration) {
		_orig.Initialize(configuration);
	}

	public void AddAssembly(Assembly assembly) {
		_orig.AddAssembly(assembly);
	}

	public void RemoveAssembly(Assembly assembly) {
		_orig.RemoveAssembly(assembly);
	}

	public Type GetType(string name) {
		return _orig.GetType(name);
	}

	public bool HasType(string name) {
		return _orig.HasType(name);
	}

	public bool HasNamespace(string name) {
		return _orig.HasNamespace(name);
	}

	public void AddType(string alias, Type type) {
		_orig.AddType(alias, type);
	}

	public event EventHandler<AssemblyHandlerEventArgs> BeforeAddAssembly;

	public event EventHandler<AssemblyHandlerEventArgs> BeforeAddType;

	public void Dispose() {
		if (_orig != null)
			_orig.Dispose();
		_orig = null;
	}
}

}
