using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Elements.Core;

using FrooxEngine;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Slots;
using FrooxEngine.UIX;

using PowerNite.PowerNite.UI;
namespace PowerNite.PowerShell.Extentions
{
    public class UIXMLParser
    {
        public Slot RootSlot;
        public UIBuilder builder;

        public UIXMLParser()
        {
            // No panel/builder creation here
        }

        public UIBuilder Render(string XML)
        {
            XDocument doc = XDocument.Parse(XML);

            // Create the panel and builder here
            var newCanvas = Engine.Current.WorldManager.FocusedWorld.RootSlot.AddSlot("PowerNiteUIXPanel");
            //newCanvas.AttachComponent<Canvas>();
            //newCanvas.GetComponent<Canvas>().Size.Value = new float2(1280, 800);
            newCanvas.GlobalScale = new float3(0.00025f, 0.00025f, 0.00025f);

            builder = RadiantUI_Panel.SetupPanel(newCanvas, "PowerNiteUIXPanel", new float2(1280, 800));
            RootSlot = newCanvas;

            RadiantUI_Constants.SetupEditorStyle(builder, true);

            // Check root element
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
				newCanvas.Destroy();
				RootSlot = null;
				builder = null;
				return null;
            }

            // Recursively process XML elements
            ProcessElement(doc.Root, builder);

            return builder;
        }

        private void ProcessElement(XElement element, UIBuilder builder)
        {
            // Map XML tag to UIBuilder method
            switch (element.Name.LocalName)
            {
                case "canvas":
                    // Canvas attributes (width, height, etc.)
                    var width = element.Attribute("width")?.Value;
                    var height = element.Attribute("height")?.Value;
                    if (width != null && height != null) {
                        RootSlot.GetComponent<Canvas>().Size.Value = new float2(float.Parse(width), float.Parse(height));
                        Console.WriteLine("set tcanvas size to: " + width + "x" + height);
						
		
					}
                    // Process children
                    foreach (var child in element.Elements())
                        ProcessElement(child, builder);
                    break;

                case "vertical":
                    builder.VerticalLayout();
                    foreach (var child in element.Elements())
                        ProcessElement(child, builder);
                    builder.NestOut();
                    break;

                case "horizontal":
                    builder.HorizontalLayout();
                    foreach (var child in element.Elements())
                        ProcessElement(child, builder);
                    builder.NestOut();
                    break;

                case "overlapping":
                    builder.OverlappingLayout();
                    foreach (var child in element.Elements())
                        ProcessElement(child, builder);
                    builder.NestOut();
                    break;

                case "scroll":
                    builder.ScrollArea();
                    foreach (var child in element.Elements())
                        ProcessElement(child, builder);
                    builder.NestOut();
                    break;

                case "text":
                    {
                        var color = element.Attribute("color")?.Value;
                        var fontsize = element.Attribute("fontsize")?.Value;
                        var text = element.Value;
                        var txt = builder.Text(text);
                        if (!string.IsNullOrWhiteSpace(color))
                        {
                            try
                            {
                                txt.Color.Value = colorX.Parse(color);
                            }
                            catch
                            {
                                // Optionally log or fallback to default color
                            }
                        }
                        if (fontsize != null)
                            txt.Size.Value = int.Parse(fontsize);
                    }
                    break;

                case "button":
                    {
                        var color = element.Attribute("color")?.Value;
                        var fontsize = element.Attribute("fontsize")?.Value;
                        var text = element.Value;
                        var btn = builder.Button(text);
                        if (!string.IsNullOrWhiteSpace(color))
                        {
                            try
                            {
                                btn.ColorDrivers[0].NormalColor.Value = colorX.Parse(color);
                            }
                            catch
                            {
                                // Optionally log or fallback to default color
                            }
                        }
                        if (fontsize != null)
                            btn.Slot.GetComponentInChildren<Text>().Size.Value = int.Parse(fontsize);
                    }
                    break;

                case "input":
                    {
                        var color = element.Attribute("color")?.Value;
                        var fontsize = element.Attribute("fontsize")?.Value;
                        var placeholder = element.Attribute("placeholder")?.Value;
                        var tf = builder.TextField(element.Value);
                        if (!string.IsNullOrWhiteSpace(color))
                        {
                            try
                            {
                                tf.Text.Color.Value = colorX.Parse(color);
                            }
                            catch
                            {
                                // Optionally log or fallback to default color
                            }
                        }
                        if (fontsize != null)
                            tf.Text.Size.Value = int.Parse(fontsize);
                        if (placeholder != null)
                            tf.Text.NullContent.Value = placeholder;
                    }
                    break;

                case "checkbox":
                    builder.Checkbox();
                    break;

                case "image":
                    {
                        var uri = element.Attribute("uri")?.Value;
                        if (uri != null)
                            builder.Image(new Uri(uri));
                    }
                    break;

                case "slider":
                    builder.Slider(32f); // Default height
                    break;

                case "box":
                    builder.Empty("Box");
                    break;

                case "textarea":
                    {
                        var color = element.Attribute("color")?.Value;
                        var fontsize = element.Attribute("fontsize")?.Value;
                        var placeholder = element.Attribute("placeholder")?.Value;
                        var tf = builder.TextField(element.Value);
                        if (!string.IsNullOrWhiteSpace(color))
                        {
                            try
                            {
                                tf.Text.Color.Value = colorX.Parse(color);
                            }
                            catch
                            {
                                // Optionally log or fallback to default color
                            }
                        }
                        if (fontsize != null)
                            tf.Text.Size.Value = int.Parse(fontsize);
                        if (placeholder != null)
                            tf.Text.NullContent.Value = placeholder;
                    }
                    break;

         
                default:
                    Console.WriteLine($"Unknown element: {element.Name}");
                    break;
            }
        }
    }
}
