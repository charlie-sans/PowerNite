using System;
using FrooxEngine.UIX;
using FrooxEngine;
using Elements.Core;
namespace PowerNite.PowerNite.UI;
internal class BaseUI {

	public BaseUI(string name, float2 size) {
	
	}
	// Static error UI creator, attaches to parent
	public static Slot CreateErrorUI(Slot parent, string msg, float font_size) {
		var RootSlot = parent.AddSlot("ErrorCanvas");
		var builder = RadiantUI_Panel.SetupPanel(RootSlot, "ErrorCanvas", new float2(800, 600));
		RadiantUI_Constants.SetupEditorStyle(builder, true);
	
		builder.Style.TextAlignment = Alignment.MiddleLeft;
		builder.Style.ForceExpandHeight = false;
		builder.Style.TextLineHeight = 1f;
		builder.VerticalLayout();
		var text = builder.Text(msg);
		text.Size.Value = font_size;
		builder.Text("This is an error message, please report this to the developer.");
		RootSlot.GlobalScale = new float3(0.001f, 0.001f, 0.001f);
		RootSlot.PositionInFrontOfUser(float3.Backward);
		return builder.Root;
	}
}
