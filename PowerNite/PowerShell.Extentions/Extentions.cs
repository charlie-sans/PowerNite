using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerNite.PowerShell.Extentions;
public class Extentions {
	public string GetResourceTextFile(string filename) {
		string result = string.Empty;

		using (Stream stream = this.GetType().Assembly.
				   GetManifestResourceStream("PowerNite.Resources" + filename)) {
			using (StreamReader sr = new StreamReader(stream)) {
				result = sr.ReadToEnd();
			}
		}
		if (result == null || result.Length == 0) {
			return "Resource not found: " + filename;
		}
		return result;
	}
}
