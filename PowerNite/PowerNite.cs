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
		//AddNewMenuOption("PowerNite", "Get Cmdlets", () => {
		//	Msg("Getting Cmdlets...");
		//	CmdletReflector.ListCmdletsFromSessionState();
		//	Msg("Cmdlets Retrieved");
		//});
		// Add the reload menu option
		AddNewMenuOption("PowerNite", "Create PowerShell", () =>
        {
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
		if (newPanel == null) {
			var eui = BaseUI.CreateErrorUI(
				Engine.Current.WorldManager.FocusedWorld.RootSlot, 
				"New panel is null, cannot handle UI builder.", 
				20f);
			eui.GlobalScale = new float3(0.0001f, 0.0001f, 0.0001f);
			eui.PositionInFrontOfUser(float3.Backward);
			eui.Name = "PowerNiteErrorUI";
			eui.GlobalPosition += new float3(0, 1, 0);
			Msg("New panel is null, cannot handle UI builder.");
			return;
		}
		var hz = builder.HorizontalLayout();
		var rect = builder.ScrollArea();
		var text = builder.TextField("Contents to render");
		text.Slot.AttachComponent<OverlappingLayout>();
		rect.Slot.AttachComponent<OverlappingLayout>();
		rect.Slot.AttachComponent<ContentSizeFitter>().VerticalFit.Value = SizeFit.MinSize;


		rect.Slot.Parent.AttachComponent<LayoutElement>().FlexibleWidth.Value = 100f; // apparently a Scrollarea gives you it's child from .slot?
		builder.NestOut();
		var v = builder.VerticalLayout();
		v.Slot.GetComponent<LayoutElement>().FlexibleWidth.Value = 10f;
		var run = builder.Button("Run");
		var reset = builder.Button("Reset");

		builder.NestOut();
		text.Editor.Slot.GetComponent<Text>().Size.Value = 10f;
		text.Editor.Slot.GetComponent<Text>().AutoSize = false;
		rect.Slot.GetComponent<Text>().Size.Value = 10f;
		rect.Slot.GetComponent<Text>().AutoSize = false;
		builder.NestOut();
		builder.NestInto(text.Slot);
		var Regtext = builder.Text("");
		Regtext.Slot.Name = "PowerNiteText";
		var ColorText = text.Editor.Slot.GetComponentInChildren<Text>();
		ColorText.Slot.Name = "PowerNiteColorText";
		Regtext.AlignmentMode.Value = Elements.Assets.AlignmentMode.LineBased;
		Regtext.HorizontalAlign.Value = Elements.Assets.TextHorizontalAlignment.Left;
		Regtext.VerticalAlign.Value = Elements.Assets.TextVerticalAlignment.Top;
		ColorText.VerticalAlign.Value = Elements.Assets.TextVerticalAlignment.Top;
		ColorText.HorizontalAlign.Value = Elements.Assets.TextHorizontalAlignment.Left;
		ColorText.AlignmentMode.Value = Elements.Assets.AlignmentMode.LineBased;
		ColorText.Slot.GetComponent<RectTransform>().OffsetMin.Value = new float2(0, 0);
		ColorText.Slot.GetComponent<RectTransform>().OffsetMax.Value = new float2(0, 0);
		Regtext.Slot.GetComponent<RectTransform>().OffsetMin.Value = new float2(0, 0);
		Regtext.Slot.GetComponent<RectTransform>().OffsetMax.Value = new float2(0, 0);

		

		//text.MarkChangeDirty();
		var fontset = Regtext.Slot.AttachComponent<StaticFont>();
		fontset.URL.Value = new Uri("resdb:///5ae30f1aad434d97a3b30556e0584d372dc1574cd4cec3beb60efd57467cb705");
		Regtext.Font.Value = fontset.ReferenceID;
		ColorText.Font.Value = fontset.ReferenceID;
		//builder.Root.GetComponent<Canvas>().MarkChangeDirty();
		text.Editor.Target.LocalEditingChanged += (x) => {
			string highlighted = SimpleSyntaxHighlighter.HighlightSyntax(ColorText.Content);
			Regtext.Content.Value = highlighted;
			//Console.WriteLine($"Highlighted text: {highlighted}");
			//Regtext.MarkChangeDirty();
			//ColorText.MarkChangeDirty();
		};
		
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
	private static readonly Dictionary<string, string> colorMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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

	public static string HighlightSyntax(string input) {
		if (string.IsNullOrEmpty(input))
			return input;

		StringBuilder result = new StringBuilder();
		string[] lines = input.Split('\n');

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
		return Regex.IsMatch(word, @"^[A-Za-z]+-[A-Za-z]+\w*$");
	}

	private static bool IsStringLiteral(string word) {
		// Check if word is a string literal (quoted)
		return (word.StartsWith("\"") && word.EndsWith("\"")) ||
			   (word.StartsWith("'") && word.EndsWith("'"));
	}

	// Optional: Method to add custom keywords
	public static void AddKeyword(string keyword, string color) {
		colorMap[keyword] = color;
	}
}
