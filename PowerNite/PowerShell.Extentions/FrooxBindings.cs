using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using FrooxEngine;
namespace PowerNite.PowerShell.Extentions.FrooxBindings;
[Cmdlet(VerbsCommon.Get, "GetChildByName")]
public class GetChildByName: Cmdlet {
	[Parameter(Mandatory = true)]
	public string Name {
		get { return name; }
		set { name = value; }
	}
	private string name;
	protected override void BeginProcessing() {
	
	}
}

/* simple way to get the badges on someone's nametag
 * and disable them for now.
 * 
 * $root = Get-GameObjectByPath "/charlie-san/nametag"
 * Write-Host "Root: $($root.Name)" // Output: Root: nametag
 * Write-Host "Children: $($root.Children.Count)" // Output: Children: 2
 * foreach ($child in $root.Children) {
 *	if ($child.Name -eq "badges") {
 *		$child.IsActive = $false
 * }
 * 
 * Write-Host "Child: $($child.Name) - Active: $($child.IsActive)
 * 
 * 
 */
