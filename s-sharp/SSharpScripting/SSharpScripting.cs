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
using System.Text;
using ScriptNET;
using ScriptNET.Runtime;

public class SSharpScripting {
	/// <summary>
	/// Initialize the S# scripting runtime.
	/// </summary>
	/// <remarks>This only needs to be done once per process.</remarks>
	static public void Init() {
		// Nuke out the default assembly manager that loads all of the appdomain's assemblies.
		RuntimeHost.AssemblyManager = new BaseAssemblyManager();

		// Do basic init.
		RuntimeHost.Initialize();

		// Replace the assembly manager with our own, which provides nearly nothing.
		RuntimeHost.AssemblyManager = new SandboxAssemblyManager(RuntimeHost.AssemblyManager);

		// Replace the binder with our own, which allows using MOO objects as 'dynamic'
		// style objects, and so forth.
		RuntimeHost.Binder = new SandboxBinder(RuntimeHost.Binder);
	}

	// We pretty much never want this.
	/* static public void AllowAssembly(System.Reflection.Assembly assembly) {
		RuntimeHost.AssemblyManager.AddAssembly(assembly);
	} */

	static public void AllowType(System.Type type, string alias = null) {
		if (alias == null)
			alias = type.Name;
		RuntimeHost.AssemblyManager.AddType(alias, type);
	}
}

}
