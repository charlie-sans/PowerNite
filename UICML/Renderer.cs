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

using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Audio;
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
		public List<string> Errors = new List<string>();
		public List<string> AllowedXMLElements = new List<string>() {
			"canvas","vertical","horizontal","overlapping","scroll",
			"text","button","input","checkbox","image","slider","box","textarea"
		};
		public static int LevenshteinDistance(string s, string t) {
			if (string.IsNullOrEmpty(s)) {
				return string.IsNullOrEmpty(t) ? 0 : t.Length;
			}
			if (string.IsNullOrEmpty(t)) {
				return s.Length;
			}
			int n = s.Length;
			int m = t.Length;
			int[,] d = new int[n + 1, m + 1];
			for (int i = 0; i <= n; d[i, 0] = i++) { }
			for (int j = 0; j <= m; d[0, j] = j++) { }
			for (int i = 1; i <= n; i++) {
				for (int j = 1; j <= m; j++) {
					int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost);
				}
			}
			return d[n, m];
		}
		public UIBuilder Render(string XML)
        {
            XDocument doc = XDocument.Parse(XML);

            // Create the panel and builder here
            var newCanvas = Engine.Current.WorldManager.FocusedWorld.RootSlot.AddSlot("PowerNiteUIXPanel");
            //newCanvas.AttachComponent<Canvas>();
            //newCanvas.GetComponent<Canvas>().Size.Value = new float2(1280, 800);
            newCanvas.GlobalScale = new float3(0.00025f, 0.00025f, 0.00025f);

            builder = RadiantUI_Panel.SetupPanel(newCanvas, "PowerNiteUIXPanelRoot", new float2(1280, 800));
            RootSlot = newCanvas;
			var audioslot = RootSlot.AddSlot("audioholder");
		
			RadiantUI_Constants.SetupEditorStyle(builder, true);

            // Check root element
            if (doc.Root.Name.LocalName != "canvas")
            {
                Console.WriteLine("Root element is not <canvas>");
                builder.Root.Destroy();
                var root = BaseUI.CreateErrorUI(
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
			if (Errors.Count > 0) {
				Console.WriteLine("Errors found in UI XML:");
				newCanvas.Destroy(); // Remove the faulty UI
				builder.Root.Destroy();
				var UI = BaseUI.CreateErrorUI(
			
					"Errors in UI XML:\n" + string.Join("\n", Errors),
					20f);
				return null;
			}

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
					var name = element.Attribute("name")?.Value ?? "PowerNiteUIXPanel";
					if (width != null && height != null) {
                        RootSlot.GetComponent<Canvas>().Size.Value = new float2(float.Parse(width), float.Parse(height));
                        Console.WriteLine("set tcanvas size to: " + width + "x" + height);
					}
					RootSlot.Name = name;
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

                case "text": {
						var color = element.Attribute("color")?.Value;
						var fontsize = element.Attribute("fontsize")?.Value;
						var minwidth = element.Attribute("minwidth")?.Value;
						var minheight = element.Attribute("minheight")?.Value;
						var text = element.Value;
						var txt = builder.Text(text);
						if (!string.IsNullOrWhiteSpace(color)) {
							try {
								txt.Color.Value = colorX.Parse(color);
							} catch {
								// Optionally log or fallback to default color
							}
						}
						if (fontsize != null)
							txt.Size.Value = int.Parse(fontsize);
						if (minwidth != null && minheight != null) {
							var comp = txt.Slot.AttachComponent<LayoutElement>();
							comp.FlexibleHeight.Value = int.Parse(minheight);
							comp.FlexibleWidth.Value = int.Parse(minwidth);
						}
					}
                    break;

                case "button":
                    {
                        var color = element.Attribute("color")?.Value;
                        var fontsize = element.Attribute("fontsize")?.Value;
						var btnname = element.Attribute("name")?.Value;

						var text = element.Value;
                        var btn = builder.Button(text);
						if (btnname != null)
							btn.Slot.Name = btnname;
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
                    var slot = builder.Empty("Box");
					var minwidt = element.Attribute("minwidth")?.Value;
					var minheigh = element.Attribute("minheight")?.Value;
					if (minheigh != null || minwidt != null) {
						var comp = slot.GetComponentOrAttach<LayoutElement>(); // should replace the if statements with GetComponentOrAttach
						if (minheigh != null)
							comp.FlexibleHeight.Value = int.Parse(minheigh);
						if (minwidt != null)
							comp.FlexibleWidth.Value = int.Parse(minwidt);
					}
					break;

                case "textarea":
                    {
                        var color = element.Attribute("color")?.Value;
                        var fontsize = element.Attribute("fontsize")?.Value;
                        var placeholder = element.Attribute("placeholder")?.Value;
						var minwidth = element.Attribute("minwidth")?.Value;
						var minheight = element.Attribute("minheight")?.Value;
						Console.WriteLine("Creating textarea with min size: " + minwidth + "x" + minheight);
						var tf = builder.TextField(element.Value);
						var audo = tf.Slot.AttachComponent<StaticAudioClip>();
						var areaname = element.Attribute("name")?.Value;
						if (areaname != null)
							tf.Slot.Name = areaname;
						audo.Slot.GetComponent<StaticAudioClip>().URL.Value = new Uri("https://cdn.freesound.org/previews/561/561661_7107243-hq.ogg");
						tf.Editor.Target.LocalEditingChanged += (E) => {
							var rs = E.Slot.PlayOneShot(audo, 1);
							rs.MaxDistance.Value = 5f;
							rs.MaxScale.Value = 10f;
							rs.MinScale.Value = 0f;	
							//E.Slot.Parent.Parent.PlayOneShot(audo,1);
							Console.WriteLine("Played sound on textarea edit");
						};
						if (minheight != null) {
							Console.WriteLine("Attached LayoutElement to textarea");
							tf.Slot.GetComponent<LayoutElement>().FlexibleHeight.Value = int.Parse(minheight);
						}
						if (minwidth != null) {
							tf.Slot.GetComponent<LayoutElement>().FlexibleWidth.Value = int.Parse(minwidth);
						}
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
					Errors.Add($"Unknown element: {element.Name}");
					// Suggest closest match
					int closestDistance = int.MaxValue;
					string closestMatch = null;
					foreach (var allowed in AllowedXMLElements) {
						int dist = LevenshteinDistance(element.Name.LocalName, allowed);
						if (dist < closestDistance) {
							closestDistance = dist;
							closestMatch = allowed;
						}
					}
					if (closestMatch != null && closestDistance <= 3) {
						Errors.Add($" Did you mean <{closestMatch}>?");
						Console.WriteLine($" Did you mean <{closestMatch}>?");
					}

					break;
            }
        }
    }
}
