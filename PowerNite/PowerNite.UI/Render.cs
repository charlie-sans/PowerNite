using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using FrooxEngine;
using Microsoft.VisualBasic;
using FrooxEngine.UIX;
using ResoniteModLoader;
using ResoniteHotReloadLib;
using System.Reflection;
using System.IO;
namespace PowerNite.PowerNite.UI;
public class Render {
	
	public void RenderUI() {
		//TODO: Implement UI rendering logic here
	}
}

public class PowerShellReflector {
	public static Type GetTypeFromDLL(string dllPath, string className) {
		try {
			var assembly = Assembly.LoadFrom(dllPath);
			var type = assembly.GetType(className);
			if (type == null) {
				Console.WriteLine($"Class {className} not found in {dllPath}");
				return null;
			}
			return type;
		} catch (Exception ex) {
			Console.WriteLine($"Error loading assembly or finding class: {ex.Message}");
			return null;
		}
	}
	public static MethodInfo GetMethodFromDLL(string dllPath, string className, string methodName) {
		try {
			var assembly = Assembly.LoadFrom(dllPath);
			var type = assembly.GetType(className);
			if (type == null) {
				Console.WriteLine($"Class {className} not found in {dllPath}");
				return null;
			}
			var method = type.GetMethod(methodName);
			if (method == null) {
				Console.WriteLine($"Method {methodName} not found in class {className}");
				return null;
			}
			return method;
		} catch (Exception ex) {
			Console.WriteLine($"Error loading assembly or finding method: {ex.Message}");
			return null;
		}
	}

	public static string FindAssemblyByName(string assemblyName) {
		var searchDirs = new List<string> {
			"rml_libs",
			
		};

		foreach (var dir in searchDirs) {
			Console.WriteLine($"Searching in: {dir}");
			if (!Directory.Exists(dir)) continue;
			foreach (var file in Directory.EnumerateFiles(dir, "*.dll", SearchOption.AllDirectories)) {
				if (Path.GetFileNameWithoutExtension(file).Equals(assemblyName, StringComparison.OrdinalIgnoreCase)) {
					// Check if this is a reference assembly
					if (IsReferenceAssembly(file)) {
						Console.WriteLine($"Found reference assembly (not usable for execution): {file}");
						continue;
					}
					Console.WriteLine($"Found assembly: {file}");
					return file;
				}
			}
		}
		return null;
	}

	private static bool IsReferenceAssembly(string filePath) {
		try {
			var assembly = Assembly.LoadFrom(filePath);
			var attrs = assembly.GetCustomAttributes(false);
			foreach (var attr in attrs) {
				if (attr.GetType().FullName == "System.Runtime.CompilerServices.ReferenceAssemblyAttribute")
					return true;
			}
		} catch {
			// If it fails to load, assume it's not usable
			return true;
		}
		return false;
	}
}
public class CmdletReflector {
	public static void ListCmdletsFromSessionState() {
		string assemblyName = "System.Management.Automation";
		string initialSessionStateTypeName = "System.Management.Automation.InitialSessionState";
		string runspaceFactoryTypeName = "System.Management.Automation.Runspaces.RunspaceFactory";
		string runspaceTypeName = "System.Management.Automation.Runspaces.Runspace";
		string powerShellTypeName = "System.Management.Automation.PowerShell";

		// Find assembly by name
		string dllPath = PowerShellReflector.FindAssemblyByName(assemblyName);
		if (dllPath == null) {
			Console.WriteLine("Could not find System.Management.Automation.dll on the system.");
			return;
		}

		var initialSessionStateType = PowerShellReflector.GetTypeFromDLL(dllPath, initialSessionStateTypeName);
		var runspaceFactoryType = PowerShellReflector.GetTypeFromDLL(dllPath, runspaceFactoryTypeName);
		var runspaceType = PowerShellReflector.GetTypeFromDLL(dllPath, runspaceTypeName);
		var powerShellType = PowerShellReflector.GetTypeFromDLL(dllPath, powerShellTypeName);

		if (initialSessionStateType == null || runspaceFactoryType == null || runspaceType == null || powerShellType == null) {
			Console.WriteLine("Failed to load required types from System.Management.Automation.dll.");
			return;
		}

		// Create InitialSessionState instance via reflection
		var initialSessionState = initialSessionStateType.GetMethod("CreateDefault", BindingFlags.Public | BindingFlags.Static)
			.Invoke(null, null);

		// Create Runspace via RunspaceFactory.CreateRunspace(initialSessionState)
		var createRunspaceMethod = runspaceFactoryType.GetMethod("CreateRunspace", new[] { initialSessionStateType });
		var runspace = createRunspaceMethod.Invoke(null, new[] { initialSessionState });

		// Open the runspace
		runspaceType.GetMethod("Open").Invoke(runspace, null);

		// Create PowerShell instance
		var powerShell = powerShellType.GetMethod("Create", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);

		// Set Runspace property
		var runspaceProp = powerShellType.GetProperty("Runspace");
		runspaceProp.SetValue(powerShell, runspace);

		// AddScript("Get-Command")
		var addScriptMethod = powerShellType.GetMethod("AddScript", new[] { typeof(string) });
		addScriptMethod.Invoke(powerShell, new object[] { "Get-Command" });

		// Invoke()
		var invokeMethod = powerShellType.GetMethod("Invoke", Type.EmptyTypes);
		var cmdlets = invokeMethod.Invoke(powerShell, null) as System.Collections.ICollection;

		if (cmdlets == null || cmdlets.Count == 0) {
			Console.WriteLine("No cmdlets found in the current session.");
			return;
		}

		// Print cmdlet names using reflection
		foreach (var cmdlet in cmdlets) {
			var propertiesProp = cmdlet.GetType().GetProperty("Properties");
			var properties = propertiesProp.GetValue(cmdlet, null);
			var itemIndexer = properties.GetType().GetProperty("Item");
			var nameProp = itemIndexer.GetValue(properties, new object[] { "Name" });
			var valueProp = nameProp.GetType().GetProperty("Value");
			var nameValue = valueProp.GetValue(nameProp, null);
			Console.WriteLine(nameValue);
		}

		// Dispose PowerShell
		var disposeMethod = powerShellType.GetMethod("Dispose");
		disposeMethod.Invoke(powerShell, null);

		// Close and dispose runspace
		runspaceType.GetMethod("Close").Invoke(runspace, null);
		runspaceType.GetMethod("Dispose").Invoke(runspace, null);
	}
}
