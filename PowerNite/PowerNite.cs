using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Resources;
using HarmonyLib;
using Elements.Core;

using FrooxEngine;
using FrooxEngine.UIX;

using HarmonyLib;

using PowerNite.PowerShell.Extentions;

using ResoniteHotReloadLib;

using ResoniteModLoader;
using PowerNite.PowerNite.UI;
using System.Text.RegularExpressions;
using System.Text;
namespace PowerNite;
// More info on creating mods can be found https://github.com/resonite-modding-group/ResoniteModLoader/wiki/Creating-Mods
public class PowerNiteMod : ResoniteMod
{
    internal const string VERSION_CONSTANT = "1.0.0";
    public override string Name => "PowerNite";
    public override string Author => "Finite";
    public override string Version => VERSION_CONSTANT;
    public override string Link => "https://git.finite.ovh/PowerNite";


	// Add a static instance field to resolve the CS0103 error
	public static PowerNiteMod INSTANCE { get; private set; }

    public static void BeforeHotReload()
    {
        Harmony harm = new Harmony("ovh.finite.PowerNite");
        harm.UnpatchAll("ovh.finite.PowerNite");
		HotReloader.RemoveMenuOption("PowerNite", "Create PowerShell");
		//HotReloader.RemoveMenuOption("PowerNite", "Get Cmdlets");

		//GC.Collect();

	}

    public static void OnHotReload(ResoniteMod modInstance)
    {
        //Harmony harm = new Harmony("ovh.finite.PowerNite");
        //harm.PatchAll();
        Setup();
		Msg("PowerNite Hot Reloaded");
	}

    public override void OnEngineInit()
    {
        // Assign the static instance field
        INSTANCE = this;

        HotReloader.RegisterForHotReload(this);
        Setup();
        Msg("meowMoew");
		Debug("PowerNite OnEngineInit Complete");
	}

	static void Setup() {
		////// Patch Harmony
		//Harmony harmony = new Harmony("ovh.finite.PowerNite");
		//harmony.PatchAll();
		//HotReloader.RegisterForHotReload(INSTANCE);
		Msg("PowerNite Setup Complete");

		AddNewMenuOption("PowerNite", "Create PowerShell", () =>
        {
			Debug("Creating PowerShell UI");
			UIXMLParser parser = new UIXMLParser();
			var root = Engine.Current.WorldManager.FocusedWorld.RootSlot;
			var newPanel = root.AddSlot("PowerNitePanel");


			newPanel.GlobalScale = new float3(0.001f, 0.001f, 0.001f);
			newPanel.PositionInFrontOfUser(float3.Backward);
			var builder = RadiantUI_Panel.SetupPanel(newPanel, "PowerNiteCanvas", new float2(1200, 800));
			builder.Root.GlobalScale = new float3(0.001f, 0.001f, 0.001f);
			RadiantUI_Constants.SetupEditorStyle(builder, true);
			builder.Style.TextLineHeight = 1f;
			handleBuilderUI(newPanel, builder,  parser);

			//text.Editor.Target.LocalEditingFinished += (x) =>
			//{
			//	if (string.IsNullOrEmpty(text.Text.Content))
			//	{
			//		Msg("Text field is empty, cannot render UI.");
			//		BaseUI.CreateErrorUI(builder.Root, "Text field is empty", 20f);
			//		return;
			//	}
			//	try
			//	{
			//		var ui = parser.Render(text.Text.Content);
			//		ui.PositionInFrontOfUser(float3.Backward);
			//		ui.GlobalScale = new float3(0.001f, 0.001f, 0.001f);
			//	}
			//	catch (Exception e)
			//	{
			//		Msg($"Error rendering UI: {e.Message}");
			//		BaseUI.CreateErrorUI(builder.Root, $"Error rendering UI: {e.Message}", 20f);
			//	}
			//};
		});
    }
	public static void handleBuilderUI(Slot newPanel, UIBuilder builder, UIXMLParser parser) {
		Debug("handleBuilderUI called");
		if (newPanel == null) {
			Msg("New panel is null, cannot handle UI builder.");
			var eui = BaseUI.CreateErrorUI(
				Engine.Current.WorldManager.FocusedWorld.RootSlot, 
				"New panel is null, cannot handle UI builder.", 
				20f);
			eui.GlobalScale = new float3(0.0001f, 0.0001f, 0.0001f);
			eui.PositionInFrontOfUser(float3.Backward);
			eui.Name = "PowerNiteErrorUI";
			eui.GlobalPosition += new float3(0, 1, 0);
			Debug("Error UI created for null panel");
			return;
		}

		// Horizontal layout section
		Msg("Setting up horizontal layout...");
		var hz = builder.HorizontalLayout();

		// Scroll area section
		Msg("Setting up scroll area...");
		var rect = builder.ScrollArea();

		// Text field section
		Msg("Setting up text field...");
		var text = builder.TextField("code, put here..");
		text.Slot.Name = "PowerNiteTextField";
		text.Slot.AttachComponent<OverlappingLayout>();
		rect.Slot.AttachComponent<OverlappingLayout>();
		rect.Slot.AttachComponent<ContentSizeFitter>().VerticalFit.Value = SizeFit.PreferredSize;

		// Layout configuration section
		Msg("Configuring layout elements...");
		rect.Slot.Parent.AttachComponent<LayoutElement>().FlexibleWidth.Value = 100f;
		builder.NestOut();

		// Vertical layout and slider section
		var v = builder.VerticalLayout();
		var SliderElement = builder.Slider(30f, 100f, 50f);
		Msg("Configuring slider element...");
		v.Slot.GetComponent<LayoutElement>().FlexibleWidth.Value = 10f;

		// Buttons section
		Msg("Adding Run and Reset buttons...");
		var run = builder.Button("Run");
		run.LocalPressed += (btn, data) =>
		{
			var butn = run;
			if (butn.Slot.GetComponent<Button>() == null)
			{
				Console.WriteLine("Run button component is null, cannot attach event.");
				return;
			}
		
			if (string.IsNullOrEmpty(text.Text.Content.Value))
			{
				Console.WriteLine("Text field is empty, cannot run PowerShell script.");
			
				return;
			}
			//Console.WriteLine($"Running PowerShell script: {text.Text.Content.Value}");
			PowerNite.ASM.Runtime.Runtime.Reset();
			PowerNite.ASM.Runtime.Runtime.Run(text.Text.Content.Value.Split('\n'));
		};
		var reset = builder.Button("UICompile");
		reset.LocalPressed += (btn, data) =>
		{
			UIXMLParser parser = new UIXMLParser();
			if (string.IsNullOrEmpty(text.Text.Content.Value))
			{
				Console.WriteLine("Text field is empty, cannot render UI.");
				//BaseUI.CreateErrorUI(builder.Root, "Text field is empty", 20f);
				return;
			}
			var Builder = parser.Render(text.Text.Content.Value);
			var slot = parser.RootSlot;
			if (slot == null)
			{
				Console.WriteLine("Builder returned null slot, cannot render UI.");
				BaseUI.CreateErrorUI(builder.Root, "Builder returned null slot", 20f);
				return;
			}
			slot.PositionInFrontOfUser(float3.Backward);
			slot.GlobalScale = new float3(0.001f, 0.001f, 0.001f);
			slot.Name = "PowerNiteUIXPanelDemo";

		};
		Msg("Configuring Run and Reset buttons...");
		builder.NestOut();

		// Text display section
		builder.NestInto(text.Slot);
		Msg("Adding Regtext and ColorText...");
		var TextToColor = builder.Text("");
		Msg("Setting up Regtext and ColorText...");
		TextToColor.Slot.Name = "PowerNiteText";
		Msg("Configuring Regtext properties...");
		var SourceText = text.Editor.Slot.GetComponentInChildren<Text>();
		Msg("Configuring ColorText properties...");
		SourceText.Size.Value = 20f;
		TextToColor.Size.Value = 20f;
		SourceText.Slot.Name = "PowerNiteColorText";

		// Slider event for text size
		SliderElement.Value.OnValueChange += (x) => {
			Debug($"Slider value changed: {x}");
			SourceText.Size.Value = x;
			TextToColor.Size.Value = x;
			Debug($"Text size set to: {x}");
		};

		// Alignment and offset section
		Msg("Setting up alignment and offsets...");
		TextToColor.Slot.GetComponent<Text>().AutoSize = false;
		SourceText.Slot.GetComponent<Text>().AutoSize = false;
		TextToColor.HorizontalAlign.Value = Elements.Assets.TextHorizontalAlignment.Left;
		TextToColor.VerticalAlign.Value = Elements.Assets.TextVerticalAlignment.Top;
		SourceText.VerticalAlign.Value = Elements.Assets.TextVerticalAlignment.Top;
		SourceText.HorizontalAlign.Value = Elements.Assets.TextHorizontalAlignment.Left;
		SourceText.Slot.GetComponent<RectTransform>().OffsetMin.Value = new float2(0, 0);
		SourceText.Slot.GetComponent<RectTransform>().OffsetMax.Value = new float2(0, 0);
		TextToColor.Slot.GetComponent<RectTransform>().OffsetMin.Value = new float2(0, 0);
		TextToColor.Slot.GetComponent<RectTransform>().OffsetMax.Value = new float2(0, 0);

		// Font section
		Msg("Attaching font...");
		var fontset = TextToColor.Slot.AttachComponent<StaticFont>();
		fontset.URL.Value = new Uri("resdb:///5ae30f1aad434d97a3b30556e0584d372dc1574cd4cec3beb60efd57467cb705");
		TextToColor.Font.Value = fontset.ReferenceID;
		SourceText.Font.Value = fontset.ReferenceID;

		// Syntax highlighter event section
		Msg("Setting up syntax highlighter event...");
		text.Editor.Target.LocalEditingChanged += (x) => {
			Debug("LocalEditingChanged event triggered");
			string highlighted = SimpleSyntaxHighlighter.HighlightSyntax(SourceText.Content, TextToColor);
			TextToColor.Content.Value = highlighted;
			Debug($"Highlighted text updated: {highlighted.Length} chars");
		};


		// Fix for CS1593: Delegate 'ButtonEventHandler' does not take 1 arguments
		if (run.Slot.GetComponent<Button>() == null)
		{
			Debug("Run button component is null, cannot attach event.");
			return;
		}

		// Fix for CS1660: Cannot convert lambda expression to type 'WorldDelegate' because it is not a delegate type




		Msg("handleBuilderUI completed.");
	}
	public static void AddNewMenuOption(string path, string name, Action reloadAction)
    {
        Debug("Begin AddReloadMenuOption");
        if (!Engine.Current.IsInitialized)
        {
            Engine.Current.RunPostInit(AddActionDelegate);
        }
        else
        {
            AddActionDelegate();
        }
        void AddActionDelegate()
        {
            DevCreateNewForm.AddAction(path, name, (x) =>
            {
				x.Destroy();
				reloadAction();
            });
            Debug($"Added PowerNite's option {name} for path {path}");
            
        }
    }
}
public class SimpleSyntaxHighlighter {
	private static readonly Dictionary<string, string> PwshcolorMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
        // Keywords
        { "function", "blue" },
		{ "filter", "blue" },
		{ "if", "blue" },
		{ "else", "blue" },
		{ "elseif", "blue" },
		{ "switch", "blue" },
		{ "foreach", "blue" },
		{ "for", "blue" },
		{ "do", "blue" },
		{ "while", "blue" },
		{ "until", "blue" },
		{ "break", "blue" },
		{ "continue", "blue" },
		{ "return", "blue" },
		{ "exit", "blue" },
		{ "throw", "blue" },
		{ "try", "blue" },
		{ "catch", "blue" },
		{ "finally", "blue" },
		{ "trap", "blue" },
		{ "param", "blue" },
		{ "begin", "blue" },
		{ "process", "blue" },
		{ "end", "blue" },
		{ "dynamicparam", "blue" },
		{ "in", "blue" },
		{ "where", "blue" },
		{ "class", "blue" },
		{ "enum", "blue" },
		{ "namespace", "blue" },
		{ "using", "blue" },
		{ "module", "blue" },
		{ "assembly", "blue" },
		{ "static", "blue" },
		{ "hidden", "blue" },
		{ "base", "blue" },
		{ "this", "blue" },
        
        // Operators
        { "and", "purple" },
		{ "or", "purple" },
		{ "not", "purple" },
		{ "xor", "purple" },
		{ "band", "purple" },
		{ "bor", "purple" },
		{ "bnot", "purple" },
		{ "bxor", "purple" },
		{ "as", "purple" },
		{ "is", "purple" },
		{ "like", "purple" },
		{ "notlike", "purple" },
		{ "match", "purple" },
		{ "notmatch", "purple" },
		{ "contains", "purple" },
		{ "notcontains", "purple" },
		{ "replace", "purple" },
        
        // Automatic variables
        { "$_", "green" },
		{ "$args", "green" },
		{ "$Error", "green" },
		{ "$false", "green" },
		{ "$foreach", "green" },
		{ "$input", "green" },
		{ "$null", "green" },
		{ "$PID", "green" },
		{ "$true", "green" },
		{ "$PSItem", "green" },
        
        // Common cmdlet prefixes
        { "Get-", "orange" },
		{ "Set-", "orange" },
		{ "New-", "orange" },
		{ "Remove-", "orange" },
		{ "Import-", "orange" },
		{ "Export-", "orange" },
		{ "Write-", "orange" },
        
        // Comments
        { "#", "gray" }
	};

	public static Dictionary<string, string> ParseExtention(Text Element) {
		// Return a copy of the color map to avoid external modifications
		switch (Element.Content.Value.ToLowerInvariant()) {
			case "pwsh":
				return PwshcolorMap; // PowerShell colors
			default:
				Console.WriteLine($"Unknown extension: {Element.Content}");
				return new Dictionary<string, string>(); // Default to nothing
		}
	}

	public static string HighlightSyntax(string input, Text Element) {
		if (string.IsNullOrEmpty(input))
			return input;

		StringBuilder result = new StringBuilder();
		string[] lines = input.Split('\n');
		var colorMap = PwshcolorMap; // Use the PowerShell color map for now
		// var colorMap = ParseExtention(Element);
		foreach (string line in lines) {
			if (line.TrimStart().StartsWith("#")) {
				// Entire line comment
				result.AppendLine($"<color=gray>{line}</color>");
				continue;
			}

			string[] words = Regex.Split(line, @"(\s+|\(|\)|\[|\]|\{|\}|;|,|\.|\"")");

			foreach (string word in words) {
				if (string.IsNullOrEmpty(word))
					continue;

				string trimmedWord = word.Trim();

				if (colorMap.ContainsKey(trimmedWord)) {
					result.Append($"<color={colorMap[trimmedWord]}>{word}</color>");
				} else if (trimmedWord.StartsWith("#") && trimmedWord.Length > 1) {
					// Inline comment
					result.Append($"<color=gray>{word}</color>");
				} else if (trimmedWord.StartsWith("$") && trimmedWord.Length > 1) {
					// Variables (starts with $ but not in predefined list)
					result.Append($"<color=green>{word}</color>");
				} else if (IsCmdlet(word)) {
					result.Append($"<color=orange>{word}</color>");
				} else if (IsStringLiteral(word)) {
					result.Append($"<color=#cf000000>{word}</color>");
				} else {
					result.Append(word);
				}
			}

			result.AppendLine(); // Preserve line breaks
		}

		return result.ToString().TrimEnd(); // Remove trailing newline
	}

	private static bool IsCmdlet(string word) {
		// Check if word looks like a cmdlet (Verb-Noun pattern)
		return Regex.IsMatch(word, @"^[A-Za-z]+-[A-ZaZ]+\w*$");
	}

	private static bool IsStringLiteral(string word) {
		// Check if word is a string literal (quoted)
		return (word.StartsWith("\"") && word.EndsWith("\"")) ||
			   (word.StartsWith("'") && word.EndsWith("'"));
	}

	
}
