namespace Kayateia.Climoo.Scripting.Ruby {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Hosting;
using System.Security.Policy;
using System.Reflection;
using System.Security.Permissions;
using System.Security;

/// <summary>
/// Main entry point for accessing the Ruby scripting plug-in for Climoo.
/// </summary>
public class RubyScripting {
	/// <summary>
	/// Creates a new scripting context.
	/// </summary>
	/// <param name="trusted">A list of assemblies that are to be fully trusted when called.</param>
	/// <returns>The new scripting context</returns>
	static public Context CreateContext(Assembly[] trusted) {
		// Grant only execute permissions.
		PermissionSet pset = new PermissionSet(PermissionState.None);
		pset.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

		// Create a list of strong names for the trusted assemblies.
		StrongName[] names = new StrongName[trusted.Length + 1];
		for (int i=0; i<trusted.Length; ++i)
			names[i] = CreateStrongName(trusted[i]);
		names[trusted.Length] = CreateStrongName(Assembly.GetExecutingAssembly());

		// Create a new AppDomain with restricted execution abilities.
		AppDomain domain = AppDomain.CreateDomain("ScriptingDomain",
			AppDomain.CurrentDomain.Evidence,
			new AppDomainSetup() { ApplicationBase = Environment.CurrentDirectory },
			pset,
			names);

		// Initialize the IronRuby runtime engine.
		ScriptRuntimeSetup setup = new ScriptRuntimeSetup() {
			DebugMode = false
		};
		setup.LanguageSetups.Add(
			new LanguageSetup("IronRuby.Runtime.RubyContext, IronRuby, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
				"IronRuby v1.1",
				new string[] { "IronRuby", "Ruby", "rb" },
				new string[] { ".rb" })
		);
		var runtime = ScriptRuntime.CreateRemote(domain, setup);
		ScriptEngine engine = runtime.GetEngine("Ruby");

		// Add us to the app domain.
		var myName = Assembly.GetExecutingAssembly().FullName;
		domain.Load(myName);

		return new Context(domain, engine);
	}

	#region Utils
	/// <summary>
	/// Create a StrongName that matches a specific assembly
	/// </summary>
	/// <exception cref="ArgumentNullException">
	/// if <paramref name="assembly"/> is null
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// if <paramref name="assembly"/> does not represent a strongly named assembly
	/// </exception>
	/// <param name="assembly">Assembly to create a StrongName for</param>
	/// <returns>A StrongName that matches the given assembly</returns>
	static StrongName CreateStrongName(Assembly assembly) {
		if (assembly == null)
			throw new ArgumentNullException("assembly");

		AssemblyName assemblyName = assembly.GetName();
		System.Diagnostics.Debug.Assert(assemblyName != null, "Could not get assembly name");

		// get the public key blob
		byte[] publicKey = assemblyName.GetPublicKey();
		if (publicKey == null || publicKey.Length == 0)
			throw new InvalidOperationException("Assembly is not strongly named");

		StrongNamePublicKeyBlob keyBlob = new StrongNamePublicKeyBlob(publicKey);

		// and create the StrongName
		return new StrongName(keyBlob, assemblyName.Name, assemblyName.Version);
	}
	#endregion
}

}
