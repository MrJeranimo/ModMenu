using Brutal.ImGuiApi;
using Brutal.Logging;
using HarmonyLib;
using KSA;
using System.Reflection;
using System.Reflection.Emit;


namespace ModMenu
{
    [HarmonyPatch(typeof(Program))]
    public static class ModMenuPatcher
    {
        public static readonly List<ModEntry> Mods = new List<ModEntry>();

        // Struct to get the name of the Mod and code to be injected
        public readonly struct ModEntry
        {
            public string Name { get; }
            public Action Callback { get; }

            public ModEntry(string name, Action callback)
            {
                Name = name;
                Callback = callback;
            }
        }

        /// <summary>
        /// Finds where to inject the Mod Menu item into the KSA Program
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>
        [HarmonyPatch("DrawMenuBar")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            MethodInfo endMenuMethod = AccessTools.Method(typeof(ImGui), nameof(ImGui.EndMenu));
            MethodInfo injectMethod = AccessTools.Method(typeof(ModMenuPatcher), nameof(InjectModMenu));

            bool injected = false;
            int endMenuCount = 0;

            for (int i = codes.Count - 1; i >= 0; i--)
            {
                if ((codes[i].opcode == OpCodes.Call || codes[i].opcode == OpCodes.Callvirt)
                    && codes[i].operand is MethodInfo method
                    && method == endMenuMethod)
                {
                    endMenuCount++;

                    if (endMenuCount == 1)
                    {
                        codes.Insert(i + 4, new CodeInstruction(OpCodes.Call, injectMethod));
                        DefaultCategory.Log.Info($"ModMenu - Successfully injected after View menu's EndMenu at index {i + 3}");
                        injected = true;
                        break;
                    }
                }
            }

            if (!injected)
            {
                DefaultCategory.Log.Error("ModMenu - Could not find injection point");
                DefaultCategory.Log.Info("ModMenu - Starting Backup Window");
                ModMenu.MenuInjectionFailed = true;
            }

            return codes;
        }

        /// <summary>
        /// Adds your ImGui code to the Mod Menu. Your code will start in your Mod's SubMenu
        /// </summary>
        /// <param name="callback"></param>
        public static void AddToModMenu(string name, Action callback)
        {
            Mods.Add(new ModEntry(name, callback));

            // Sorts alphabetically by Mod Name
            Mods.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Creates the ImGui Menu "Mods" and each of the added Mod's SubMenus
        /// </summary>
        private static void InjectModMenu()
        {
            try
            {
                // Begins the Menu Mods
                if (ImGui.BeginMenu("Mods", true))
                {
                    // Loops through and adds each of the Mods ImGui code to the menu
                    foreach (var mod in Mods)
                    {
                        if (ImGui.BeginMenu(mod.Name)) {
                            try
                            {
                                mod.Callback();
                            }
                            catch (Exception ex)
                            {
                                DefaultCategory.Log.Error($"ModMenu - Mod Callback Error: {ex}");
                            }
                            ImGui.EndMenu();
                        }
                        else
                        {
                            DefaultCategory.Log.Error($"ModMenu - Error Creating {mod.Name}'s SubMenu");
                        }
                    }
                    ImGui.EndMenu();
                }
            }
            catch (Exception ex)
            {
                DefaultCategory.Log.Error($"ModMenu - Error Injecting the Mod Menu: {ex}");
            }
        }
    }
}