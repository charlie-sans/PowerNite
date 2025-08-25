using System;
using System.Buffers;
using System.Collections.Generic;

using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Elements.Core;

using FrooxEngine;
using FrooxEngine.UIX;

using HarmonyLib;
using HarmonyLib;

using PowerNite.PowerNite.UI;
using PowerNite.PowerNite.UI.Renderer;
using PowerNite.PowerShell.Extentions;




using ResoniteHotReloadLib;

using ResoniteModLoader;
using System.IO.Compression;
using System.IO;
using System.Reflection;
using System.Linq;
namespace PowerNite;
// More info on creating mods can be found https://github.com/resonite-modding-group/ResoniteModLoader/wiki/Creating-Mods
public class PowerNiteMod : ResoniteMod
{
    internal const string VERSION_CONSTANT = "1.0.0";
    public override string Name => "PowerNite";
    public override string Author => "Finite";
    public override string Version => VERSION_CONSTANT;
    public override string Link => "https://git.finite.ovh/PowerNite";

	public static string Domain = "";
	public static PowerNiteMod INSTANCE { get; private set; }
	public static void Msg(string msg)
	{
		Console.WriteLine($"[{INSTANCE.Name}] {msg}");
	}
	public static void Debug(string msg) {
		Console.WriteLine($"[{INSTANCE.Name} Debug] {msg}");
	}

    public static void BeforeHotReload()
    {
        Harmony harm = new Harmony("ovh.finite.PowerNite");
        harm.UnpatchAll("ovh.finite.PowerNite");
		HotReloader.RemoveMenuOption("PowerNite", "Create PowerShell");
		HotReloader.RemoveMenuOption("PowerNite", "Create MASMShell");

	}


public static void OnHotReload(ResoniteMod modInstance)
    {
        //Harmony harm = new Harmony(Domain);
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
		Debug($"{Name} OnEngineInit Complete");
	}

	static void Setup() {
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
            Debug($"Added {INSTANCE.Name}'s option {name} for path {path}");
            
        }
    }
}
