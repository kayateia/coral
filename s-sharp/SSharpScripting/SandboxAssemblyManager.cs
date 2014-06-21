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
/// This custom assembly manager will allow us to have much finer grain control
/// over both assembly loading and object finding (for static methods). Many
/// calls are just passed down to the original assembly manager.
/// </summary>
public class SandboxAssemblyManager : IAssemblyManager {
	public SandboxAssemblyManager(IAssemblyManager orig) {
		_orig = orig;
		_orig.BeforeAddAssembly += beforeAddAssembly;
		_orig.BeforeAddType += beforeAddType;
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
		// Currently we only support System (and only some of those).
		return name == "System";

		//return _orig.HasNamespace(name);
	}

	public void AddType(string alias, Type type) {
		_orig.AddType(alias, type);
	}

	// These are just here for show.
	public event EventHandler<AssemblyHandlerEventArgs> BeforeAddAssembly;
	public event EventHandler<AssemblyHandlerEventArgs> BeforeAddType;

	void beforeAddAssembly(object sender, AssemblyHandlerEventArgs evt) {
		if (this.BeforeAddAssembly != null) {
			this.BeforeAddAssembly(sender, evt);
			if (evt.Cancel)
				return;
		}

		// Presently we disallow all assembly adds.
		evt.Cancel = true;
	}

	void beforeAddType(object sender, AssemblyHandlerEventArgs evt) {
		if (this.BeforeAddType != null) {
			this.BeforeAddType(sender, evt);
			if (evt.Cancel)
				return;
		}

		System.Diagnostics.Debug.WriteLine("Added type: {0}", evt.Type.FullName);
	}

	public void Dispose() {
		if (_orig != null)
			_orig.Dispose();
		_orig = null;
	}
}

}
