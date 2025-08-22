using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Resources;

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
    }

    public static void OnHotReload(ResoniteMod modInstance)
    {
        Harmony harm = new Harmony("ovh.finite.PowerNite");
        harm.PatchAll();

        Setup();
    }

    public override void OnEngineInit()
    {
        // Assign the static instance field
        INSTANCE = this;

        HotReloader.RegisterForHotReload(this);
        Setup();
        Msg("meowMoew");
    }

    static void Setup()
    {
        // Patch Harmony
        Harmony harmony = new Harmony("ovh.finite.PowerNite");
        harmony.PatchAll();
        HotReloader.RegisterForHotReload(INSTANCE);
        Msg("PowerNite Setup Complete");
        Debug("PowerNite Setup Complete");
        // Add the reload menu option
        AddNewMenuOption("PowerNite", "Create PowerShell", () =>
        {
            Msg("Spawn UI");
			//var UIResource = EXT.GetResourceTextFile("BaseEditor.XML");
			Msg("meow");
			var slot = UIXML.Render("""
				<canvas width="800" height="600" background="#5e294e50">
				    <horizontal minheight="50" spacing="10" background="#222">
				        <vertical spacing="10" padding="10" minwidth="650">
				            <horizontal minheight="1" spacing="10" background="#222">
				                <text color="#fff" halign="center" fontsize="20">main.ps1</text>
				            </horizontal>
				            <textarea minheight="300" color="#fff" background="#f0f0f0" fontsize="30" placeholder="Enter your code here..." >codeasd</textarea>
				            <vertical spacing="10">
				                <textarea minheight="100" color="#fff" background="#111" fontsize="30" placeholder="Console output..." >consoleasd</textarea>
				                <input placeholder="Type here..." minheight="5" fontsize="30"></input>
				            </vertical>
				        </vertical>
				        <vertical spacing="2" padtop="10" padleft="10" padright="10" padbottom="10">
				            <button text="Run" fontsize="14" minwidth="2">Run</button>
				            <button text="Reset" fontsize="14" minwidth="2">Reset</button>
				            <button text="Save" fontsize="14" minwidth="2">Export</button>
				            <button text="Help"  fontsize="14" minwidth="2">Help</button>
				        </vertical>
				    </horizontal>
				</canvas>
				""");
			Msg("created slot");
			slot.Name = "PowerNite";
			slot.Parent = Engine.Current.WorldManager.FocusedWorld.RootSlot;
			slot.PositionInFrontOfUser(float3.Backward);
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
            Debug($"Added PowerNite");
            Console.WriteLine($"Added PowerNite");
        }
    }
}
