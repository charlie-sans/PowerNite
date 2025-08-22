using Elements.Core;
using FrooxEngine;
using FrooxEngine.UIX;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Slots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
namespace PowerNite.PowerShell.Extentions
{
    public class UIXMLParser
    {
        public static Slot rootSlot = Engine.Current.WorldManager.FocusedWorld.RootSlot;
        // slot cache for the canvas slot
        public Slot CanvasSlot { get; private set; }
        public Slot Parse(string xml)
        {
            var slot = rootSlot.AddSlot("UIXMLParserRoot");
            if (CanvasSlot != null)
            {
                // clear the previous canvas slot if it exists
                CanvasSlot = null;
            }
            var doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (XmlException ex)
            {
                var errorSlot = slot.AddSlot("xmlError").AttachComponent<TextRenderer>();
                errorSlot.Text.Value = $"XML Parsing Error: {ex.Message}";
            }

            var rootElement = doc.DocumentElement; // root element should be <canvas> or similar
            if (rootElement == null || rootElement.Name != "canvas")
            {
                var errorSlot = slot.AddSlot("invalidRoot").AttachComponent<TextRenderer>();
                errorSlot.Text.Value = "Invalid root element. Expected <canvas>.";

            }

            // Process the XML elements and create corresponding Slots based on the XML Schema

            XDocument xDocument = XDocument.Parse(xml);
            XElement canvas = xDocument.Root;

            // Process canvas
            ProcessElement(canvas);

            slot.Name = "ParsedSlot";

            return slot;
        }
        private void Msg(string message)
        {
            Console.WriteLine(message);
        }
        private void ProcessElement(XElement element)
        {
            Msg($"Element: <{element.Name}>");

            // Collect all attributes into a dictionary
            var attributes = element.Attributes()
                .ToDictionary(a => a.Name.LocalName.ToLower(), a => a.Value);

            // Print attributes
            foreach (var attr in attributes)
            {
                Msg($"Attribute: {attr.Key} = {attr.Value}");
            }

            // Pass all attributes to the appropriate handler
            switch (element.Name.LocalName.ToLower())
            {
                case "canvas":
                    HandleCanvasAttributes(attributes);
                    Msg("Canvas element found.");
                    break;


                default:
                    Msg($"Unknown element: <{element.Name}>");
                    break;
            }

            // Process content if it's text content
            if (!string.IsNullOrEmpty(element.Value.Trim()) && !element.Elements().Any())
            {
                Msg($"Content: {element.Value}");
            }

            // Process child elements
            foreach (XElement child in element.Elements())
            {
                ProcessElement(child);
            }
        }

        private void HandleCanvasAttributes(Dictionary<string, string> attrs)
        {
            var CanvasSlot = rootSlot.AddSlot(attrs.TryGetValue("name", out var name) ? name : "Canvas");
            if (CanvasSlot != null)
            {
                // set up the canvas with basic slots.
               
                var backing = CanvasSlot.AddSlot("Backing");
                backing.AttachComponent<StaticTexture2D>().URL.Value = new Uri("resdb:///cb7ba11c8a391d6c8b4b5c5122684888a6a719179996e88c954a49b6b031a845.png");
                backing.AttachComponent<RawImage>().Texture.Value = backing.GetComponent<StaticTexture2D>().ReferenceID; // this is funkyyyyy, i love IT!!!!!
            }
            // get the width and height
            if (attrs.TryGetValue("width", out var widthStr) && int.TryParse(widthStr, out var width) && attrs.TryGetValue("height", out var heightStr) && int.TryParse(heightStr, out var height))
            {
                Msg($"Canvas Width: {width}");
                Msg($"Canvas Height: {height}");
                CanvasSlot.AttachComponent<Canvas>().Size.Value = new float2(width, height);
            }

        }
    }
    public class UIXML
    {
        public static Slot rootSlot = Engine.Current.WorldManager.FocusedWorld.RootSlot;
        public static Slot Render(string xml)
        {
            var slott = rootSlot.AddSlot("UIXMLParserRoot");
            if (string.IsNullOrEmpty(xml))
            {
                var errorSlot = slott.AddSlot("xmlError").AttachComponent<TextRenderer>();
                errorSlot.Text.Value = $"XML Parsing failed beacause no text was provided";
            }
            var parser = new UIXMLParser();
            var slot = parser.Parse(xml);
            if (slot == null)
            {
                throw new InvalidOperationException("Failed to parse XML into a Slot.");
            }
            slot.Parent = rootSlot;
            return slot ?? slott;
        }
    }
}
