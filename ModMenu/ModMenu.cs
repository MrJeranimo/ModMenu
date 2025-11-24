using Brutal.ImGuiApi;
using HarmonyLib;
using KSA;
using StarMap.API;

namespace ModMenu
{
    [StarMapMod]
    public class ModMenu
    {
        private readonly Harmony MHarmony = new Harmony("ModMenu");
        private static bool ShowWindow = true;
        public static bool MenuInjectionFailed = false;
        

        [StarMapAllModsLoaded]
        public void OnAllModsLoaded()
        {
            // Patches ModMenu
            MHarmony.PatchAll(typeof(ModMenu).Assembly);
        }

        [StarMapUnload]
        public void OnUnload()
        {
            // Unpatches only ModMenu
            MHarmony.UnpatchAll(nameof(ModMenu));
        }

        [StarMapAfterGui]
        public void DrawBackUpWindow()
        {
            // If the Mod Menu Injection works, don't create a window.
            if (!MenuInjectionFailed) return;

            if (!ShowWindow) return;

            // Create the backup window
            if(ImGui.Begin("Mod Menu Backup", ref ShowWindow))
            {
                foreach(var mod in ModMenuPatcher.Mods)
                {
                    if(ImGui.BeginMenu(mod.Name))
                    {
                        mod.Callback();
                        ImGui.EndMenu();
                    }
                }
                ImGui.End();
            }
        }
    }
}
