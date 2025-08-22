using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Elements.Core;

using FrooxEngine;
using FrooxEngine.UIX;
namespace PowerNite.PowerShell.Extentions
{
    public class UIXMLParser
    {
    
        public static Uri BackgroundLogo = new Uri("resdb:///6c145eed7431fc20c8d33c3fccc873f97e4a8ca21c1a4199447942cf79be6ffa.png");    
        public static Uri NineSlice = new Uri("resdb:///cb7ba11c8a391d6c8b4b5c5122684888a6a719179996e88c954a49b6b031a845.png");
        public static UI_UnlitMaterial UIMat { get; private set; }
		public bool AlreadyAdded { get; private set; } = false;
		public Slot CanvasSlot { get; private set; }
        public Slot LastAddedSlot { get; private set; } // make this instance, not static

        public UIXMLParser()
        {
            Msg("UIXMLParser constructor called.");
            if (CanvasSlot != null && !CanvasSlot.IsDestroyed)
            {
                Msg("UIXMLParser already has a CanvasSlot initialized.");
                Console.WriteLine("UIXMLParser already has a CanvasSlot initialized.");
                return;
            }
            Msg("Initializing CanvasSlot.");
            CanvasSlot = Engine.Current.WorldManager.FocusedWorld.RootSlot.AddSlot("ovh.Finite.TemplateUIXCanvasNam");
            CanvasSlot.GlobalScale = new float3(0.3f, 0.3f, 0.3f);
            CanvasSlot.PositionInFrontOfUser(float3.Backward);
            CanvasSlot.AttachComponent<Canvas>().Size.Value = new float2(32, 16);
            Msg("UIXMLParser initialized with CanvasSlot.");
            Console.WriteLine("UIXMLParser initialized with CanvasSlot.");
            AlreadyAdded = true;
            LastAddedSlot = CanvasSlot;
        }
        public Slot Parse(string xml)
        {
            Msg("Parse called.");
            if (string.IsNullOrEmpty(xml))
            {
                Msg("XML input is null or empty.");
                var errorSlot = CanvasSlot.AddSlot("xmlError").AttachComponent<TextRenderer>();
                errorSlot.Text.Value = $"XML Parsing failed because no text was provided";
                errorSlot.Slot.GlobalPosition += new float3(0, -1f, 0);
                LastAddedSlot = errorSlot.Slot;
                Msg("Returning null from Parse due to empty XML.");
                return null;
            }
            Slot slot = null;
            if (CanvasSlot == null || CanvasSlot.IsDestroyed)
            {
                Msg("CanvasSlot is null or destroyed, creating new CanvasSlot.");
                Console.WriteLine("CanvasSlot is null or invalid, creating a new one.");
                CanvasSlot = Engine.Current.WorldManager.FocusedWorld.RootSlot.AddSlot("ovh.Finite.TemplateUIXCanvasName");
                CanvasSlot.GlobalScale = new float3(0.3f, 0.3f, 0.3f);
                CanvasSlot.PositionInFrontOfUser(float3.Backward);
                CanvasSlot.AttachComponent<Canvas>().Size.Value = new float2(32, 16);
                AlreadyAdded = true;
            }
            else
            {
                Msg("Using existing CanvasSlot.");
                Console.WriteLine("Using existing CanvasSlot.");
                // slot = CanvasSlot.AddSlot("UIXMLParserRootSlot");
            }
            Msg("Adding UIXMLParserRootSlot to CanvasSlot.");
            slot = CanvasSlot.AddSlot("UIXMLParserRootSlot");
            LastAddedSlot = slot;

            var doc = new XmlDocument();
            try
            {
                Msg("Attempting to load XML.");
                doc.LoadXml(xml);
                Msg("XML loaded successfully.");
            }
            catch (XmlException ex)
            {
                Msg($"XMLException caught: {ex.Message}");
                var errorSlot = slot.AddSlot("xmlError").AttachComponent<TextRenderer>();
                errorSlot.Text.Value = $"XML Parsing Error: {ex.Message}";
                errorSlot.Slot.GlobalPosition += new float3(0, -1f, 0);
                LastAddedSlot = errorSlot.Slot;
                Msg("Returning null from Parse due to XMLException.");
                return null;
            }

            var rootElement = doc.DocumentElement;
            if (rootElement == null || rootElement.Name != "canvas")
            {
                Msg("Root element is invalid or not <canvas>.");
                var errorSlot = slot.AddSlot("invalidRoot").AttachComponent<TextRenderer>();
                errorSlot.Text.Value = "Invalid root element. Expected <canvas>.";
                errorSlot.Slot.GlobalPosition += new float3(0, -1f, 0);
                Console.WriteLine("Invalid root element. Expected <canvas>.");
                Msg("Invalid root element. Expected <canvas>.");
                LastAddedSlot = errorSlot.Slot;
                Msg("Returning null from Parse due to invalid root element.");
                return null;
            }

            Msg("Processing XML elements.");
            XDocument xDocument = XDocument.Parse(xml);
            XElement canvas = xDocument.Root;
            ProcessElement(canvas);

            slot.Name = "ParsedSlot";
            LastAddedSlot = slot;
            Msg("Parse completed successfully.");
            return slot;
        }
        private static void Msg(string message)
        {
            Console.WriteLine($"[UIXMLParser] {message}");
        }
        private void ProcessElement(XElement element)
        {
            Msg($"ProcessElement called for <{element.Name}>.");

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
                    Msg("Handling canvas attributes.");
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
                Msg($"Processing child element <{child.Name}>.");
                ProcessElement(child);
            }
        }

        private void HandleCanvasAttributes(Dictionary<string, string> attrs)
        {
            Msg("HandleCanvasAttributes called.");
            CanvasSlot.Name = attrs.TryGetValue("name", out var name) ? name : "ovh.Finite.TemplateUIXCanvasName";
            if (CanvasSlot != null)
            {
                Msg("Initializing canvas slot.");
                InitCanvas(CanvasSlot);
            }
            if (attrs.TryGetValue("width", out var widthStr) && int.TryParse(widthStr, out var width) &&
                attrs.TryGetValue("height", out var heightStr) && int.TryParse(heightStr, out var height))
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
            Msg("InitCanvas called.");
            canvasSlot.GlobalScale = new float3(0.1f, 0.1f, 0.1f);
            var backing = canvasSlot.AddSlot("Backing");
            canvasSlot.AddSlot("content");
            Msg("initializing canvas slot wih backing and contents");
            backing.AttachComponent<StaticTexture2D>().URL.Value = NineSlice;
            backing.AttachComponent<RawImage>().Texture.Value = backing.GetComponent<StaticTexture2D>().ReferenceID;
            backing.AddSlot("Quad").AttachComponent<MeshRenderer>().Material.Value = UIMat.ReferenceID;
            var quad = backing.FindChild("Quad");
            var mesh = quad.AttachComponent<QuadMesh>();
            quad.GetComponent<MeshRenderer>().Mesh.Value = mesh.ReferenceID;
            mesh.Size.Value = new float2(32, 16); 
            quad.LocalPosition = new float3(0, 0, -0.01f);
            Msg("InitCanvas completed.");
        }
        public  Slot Render(string xml)
        {
            Msg("Render called.");
            if (string.IsNullOrEmpty(xml))
            {
                Msg("XML input is null or empty in Render.");
                var errorSlot = CanvasSlot.AddSlot("xmlError").AttachComponent<TextRenderer>();
                errorSlot.Text.Value = $"XML Parsing failed because no text was provided";
                errorSlot.Slot.GlobalPosition += new float3(0, -1f, 0);
                errorSlot.Slot.GlobalScale = new float3(0.1f, 0.1f, 0.1f);
                LastAddedSlot = errorSlot.Slot;
                Msg("Returning null from Render due to empty XML.");
                return null;
            }
            var slot = Parse(xml);
            if (slot == null)
            {
                Msg("Parse returned null in Render, throwing exception.");
                throw new InvalidOperationException("Failed to parse XML into a Slot.");
            }
            Msg("Render completed successfully.");
            return slot;
        }
    }


}
