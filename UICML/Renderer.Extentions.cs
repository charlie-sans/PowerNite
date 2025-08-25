using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FrooxEngine;

namespace PowerNite.PowerNite.UI.Renderer;
public class Extentions {
	public static  Slot GetSlotFromName(Slot RootSlot, string name) {
		return RootSlot.GetAllChildren().FirstOrDefault(s => s.Name == name);
	}
	public static Component GetComponentFromSlotName<T>(Slot RootSlot, string name) where T : Component {
		var slot = GetSlotFromName(RootSlot, name);
		if (slot == null)
			Console.WriteLine($"[GetComponentFromSlotName] Slot with name '{name}' not found.");
		return slot.GetComponent<T>();
	}
}
