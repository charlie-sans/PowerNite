using System;
using System.Management.Automation;

namespace PowerNite.PowerNite.PowerShell;
internal class PowerShell {
    public static void RunWriteHost() {
        using (var ps = System.Management.Automation.PowerShell.Create()) {
            ps.AddScript("Write-Host 'Hello from PowerShell'");
            ps.Invoke();
        }
    }
}
