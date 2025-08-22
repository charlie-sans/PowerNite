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
        public static Uri BackgroundLogo = new Uri("resdb:///6c145eed7431fc20c8d33c3fccc873f97e4a8ca21c1a4199447942cf79be6ffa.png");    
        public static Uri NineSlice = new Uri("resdb:///cb7ba11c8a391d6c8b4b5c5122684888a6a719179996e88c954a49b6b031a845.png");
        public static UI_UnlitMaterial UIMat { get; private set; }
        // slot cache for the canvas slot
        public Slot CanvasSlot { get; private set; } 
        public UIXMLParser()
        {
           
            // Initialize the CanvasSlot
            CanvasSlot = rootSlot.AddSlot("Canvas");
            CanvasSlot.AttachComponent<Canvas>().Size.Value = new float2(32, 16); // Default size if the canvas can't be created
        }
        public Slot Parse(string xml)
        {
            var slot = CanvasSlot.AddSlot("UIXMLParserRoot");
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
        private static void Msg(string message)
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
            CanvasSlot.Name = attrs.TryGetValue("name", out var name) ? name : "Canvas";
            if (CanvasSlot != null)
            {
                // set up the canvas with basic slots.
                InitCanvas(CanvasSlot);

            }
            // get the width and height
            if (attrs.TryGetValue("width", out var widthStr) && int.TryParse(widthStr, out var width) && attrs.TryGetValue("height", out var heightStr) && int.TryParse(heightStr, out var height))
            {
                Msg($"Canvas Width: {width}");
                Msg($"Canvas Height: {height}");
                CanvasSlot.GetComponent<Canvas>().Size.Value = new float2(width, height);
            }
        }
        /// <summary>
        /// Generatess the bare minium for a canvas slot.
        /// </summary>  
       
        public static void InitCanvas(Slot canvasSlot)
        {
            // god this method looks terrible, but it works, so whatever.
            canvasSlot.GlobalScale = new float3(0.1f, 0.1f, 0.1f); // set the global scale to 0.1f as FrooxEngine uses 1 unit = 1 meter, and we want the canvas to be smaller.
            var backing = canvasSlot.AddSlot("Backing");
            canvasSlot.AddSlot("content");
            Msg("initializing canvas slot wih backing and contents");
            backing.AttachComponent<StaticTexture2D>().URL.Value = NineSlice;
            backing.AttachComponent<RawImage>().Texture.Value = backing.GetComponent<StaticTexture2D>().ReferenceID; // this is funkyyyyy, i love IT!!!!!
            backing.AddSlot("Quad").AttachComponent<MeshRenderer>().Material.Value = UIMat.ReferenceID;
            var quad = backing.FindChild("Quad");
            var mesh = quad.AttachComponent<QuadMesh>();
            quad.GetComponent<MeshRenderer>().Mesh.Value = mesh.ReferenceID;
            mesh.Size.Value = new float2(32, 16); 
            quad.LocalPosition = new float3(0, 0, -0.01f); // slightly behind the canvas so that it doesn't occlude the canvas
        }
    }

    public class UIXML
    {
        public static Slot rootSlot = Engine.Current.WorldManager.FocusedWorld.RootSlot;
        public static Slot Render(string xml)
        {
            var slott = Engine.Current.WorldManager.FocusedWorld.RootSlot.AddSlot("UIXMLParserRoot");
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
            slot.Parent = slott;
            return slot;
        }
    }
}
