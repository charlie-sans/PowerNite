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
namespace PowerNite;
// More info on creating mods can be found https://github.com/resonite-modding-group/ResoniteModLoader/wiki/Creating-Mods
public class PowerNiteMod : ResoniteMod
{
    internal const string VERSION_CONSTANT = "1.0.0";
    public override string Name => "PowerNite";
    public override string Author => "Finite";
    public override string Version => VERSION_CONSTANT;
    public override string Link => "https://git.finite.ovh/PowerNite";
	public static Extentions EXT { get; private set; } = new Extentions();

	// Add a static instance field to resolve the CS0103 error
	public static PowerNiteMod INSTANCE { get; private set; }

    public static void BeforeHotReload()
    {
        Harmony harm = new Harmony("ovh.finite.PowerNite");
        harm.UnpatchAll("ovh.finite.PowerNite");
		HotReloader.RemoveMenuOption("PowerNite", "Create PowerShell");
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

    static void Setup()
    {
		////// Patch Harmony
		//Harmony harmony = new Harmony("ovh.finite.PowerNite");
		//harmony.PatchAll();
		//HotReloader.RegisterForHotReload(INSTANCE);
		Msg("PowerNite Setup Complete");

        // Add the reload menu option
        AddNewMenuOption("PowerNite", "Create PowerShell", () =>
        {
            Msg("Spawn UI");
			//var UIResource = EXT.GetResourceTextFile("BaseEditor.XML");
			Msg("meow");
			Debug("meow");
			Msg("either we hot reloaded or started the game");
			var UIXParser = new UIXMLParser();
			var slot = UIXParser.Render("""
				canvas width="800" height="600" background="#5e294e50">
				   
				</canvas>
				""");
			Msg("created slot");
			slot.Name = "PowerNite";
			slot.PositionInFrontOfUser(float3.Backward);
			slot.GlobalScale = new float3(0.1f, 0.1f, 0.1f); // set the global scale to 0.1f as FrooxEngine uses 1 unit = 1 meter, and we want the canvas to be smaller.
			Debug("Spawned PowerNite UI");
			Console.WriteLine("Spawned PowerNite UI");
		});
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
                reloadAction();
            });
            Debug($"Added PowerNite's option {name} for path {path}");
            
        }
    }
}
