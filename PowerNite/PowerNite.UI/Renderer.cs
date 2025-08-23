using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Elements.Core;

using FrooxEngine;
using FrooxEngine.UIX;

using PowerNite.PowerNite.UI;
namespace PowerNite.PowerShell.Extentions
{
    public class UIXMLParser
    {

    
        public UIXMLParser()
		{
			// Constructor logic if needed
		}
		public Slot Render(string XML) {
			// parse the XML file
			XDocument doc = XDocument.Parse(XML);
			// Create a new UIBuilder instance
			var newCanvas = Engine.Current.WorldManager.FocusedWorld.RootSlot.AddSlot("PowerNiteUIXPanel");
			newCanvas.AttachComponent<Canvas>();
			newCanvas.GetComponent<Canvas>().Size.Value = new float2(1280, 800);
			newCanvas.GlobalScale = new float3(0.001f, 0.001f, 0.001f);
			var builder = RadiantUI_Panel.SetupPanel(newCanvas, "ErrorCanvas", new float2(800, 600));
			RadiantUI_Constants.SetupEditorStyle(builder, true);

			// check to see if the root element is <canvas>
			if (doc.Root.Name.LocalName != "canvas")
			{
				Console.WriteLine("Root element is not <canvas>");
				builder.Root.Destroy();
				var root = BaseUI.CreateErrorUI(
					Engine.Current.WorldManager.FocusedWorld.RootSlot, 
					"Root element is not <canvas>", 
					20f);
				root.GlobalScale = new float3(0.001f, 0.001f, 0.01f);
				root.PositionInFrontOfUser(float3.Backward);
				return root;

			}

			foreach (var element in doc.Root.Elements())
			{
				switch (element.Name.LocalName)
				{
					case "canvas":
						// get the element attributes
						var width = element.Attribute("width")?.Value;
						var height = element.Attribute("height")?.Value;
						if (width != null)
						{
							builder.Canvas.Size.Value = new float2(float.Parse(width), 
								float.Parse(height));
						}
						break;
					case "text":
						builder.Text(element.Value);
						break;
				
					default:
						Console.WriteLine($"Unknown element: {element.Name}");
						break;
				}
			}

			return builder.Root;
		
		}
	}


}
