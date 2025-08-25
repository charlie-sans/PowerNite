using System;
using FrooxEngine.UIX;
using FrooxEngine;
using Elements.Core;
namespace PowerNite.PowerNite.UI;
public class BaseUI {
	public static readonly string Logo_Error = "https://raw.githubusercontent.com/charlie-sans/PowerNite/refs/heads/master/PowerNite/PowerNite.Resources/Logo_error.png";
	public BaseUI(string name, float2 size) {
	
	}
	// Static error UI creator, attaches to parent
	public static Slot CreateErrorUI( string msg, float font_size = 6f) {
		var RootSlot = Engine.Current.WorldManager.FocusedWorld.RootSlot.AddSlot("ErrorCanvas");

		Console.WriteLine(msg);
		var builder = RadiantUI_Panel.SetupPanel(RootSlot, "Woops, something Broke :{", new float2(800, 1280));
		RadiantUI_Constants.SetupEditorStyle(builder, true);
	
		builder.Style.TextAlignment = Alignment.MiddleLeft;
		builder.Style.ForceExpandHeight = false;
		builder.Style.TextLineHeight = 1f;
		builder.VerticalLayout();
		var img = builder.Image(new Uri(Logo_Error));
		img.Slot.GetComponent<LayoutElement>().FlexibleHeight.Value = 30f;
		var scroll = builder.ScrollArea();
		var scrlayout = scroll.Slot.Parent.GetComponent<LayoutElement>();
		scroll.Slot.AttachComponent<OverlappingLayout>();
		scroll.Slot.AttachComponent<ContentSizeFitter>().VerticalFit.Value = SizeFit.MinSize;
		if (scrlayout == null) Console.WriteLine("scrlayout is null");
		scrlayout.FlexibleHeight.Value = 30f; // default should be 30f
		var text = builder.Text(msg);
		text.Size.Value = font_size; // default should be 6f from params
		text.VerticalAutoSize.Value = false; // never do that for a scrill area
		builder.NestOut(); // get out of the scroll area back to the main layout

		// stack trace area
		builder.Text("Stack Trace:").Size.Value = 20f;
		var secscroll = builder.ScrollArea(); // add another scroll area for the stack trace

		var textbox = builder.Text(System.Environment.StackTrace);
		// proform the bs that was layout adding
		secscroll.Slot.Parent.GetComponent<LayoutElement>().FlexibleHeight.Value = 30f; // default should be 30f
		secscroll.Slot.AttachComponent<OverlappingLayout>();
		secscroll.Slot.AttachComponent<ContentSizeFitter>().VerticalFit.Value = SizeFit.MinSize;
		builder.NestOut(); // get out of the scroll area back to the main layout
		textbox.VerticalAutoSize.Value = false;
		textbox.Size.Value = 13f;
		RootSlot.GlobalScale = new float3(0.001f, 0.001f, 0.001f);

		RootSlot.GlobalPosition += new float3(0, 0, 10f);
		RootSlot.PositionInFrontOfUser(float3.Backward);
		return builder.Root;
	}
}
