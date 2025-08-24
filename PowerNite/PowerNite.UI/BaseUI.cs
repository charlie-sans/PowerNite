using System;
using FrooxEngine.UIX;
using FrooxEngine;
using Elements.Core;
namespace PowerNite.PowerNite.UI;
internal class BaseUI {
	public static readonly string Logo_Error = "https://raw.githubusercontent.com/PowerNite/PowerNite/main/Assets/PowerNite.UI/Textures/ErrorLogo.png";
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
		builder.Image(new Uri(Logo_Error));
		builder.Text("");
		var text = builder.Text(msg);
		RootSlot.GlobalScale = new float3(0.001f, 0.001f, 0.001f);
		RootSlot.PositionInFrontOfUser(float3.Backward);
		return builder.Root;
	}
}
