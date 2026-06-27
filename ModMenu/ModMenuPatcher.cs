using Brutal.ImGuiApi;
using Brutal.Logging;
using HarmonyLib;
using KSA;
using System.Reflection;
using System.Reflection.Emit;


namespace ModMenu
{
    /// <summary>
    /// Attribute to mark methods that should appear in the Mod Menu.
    /// Just copy this attribute definition to your mod and decorate your menu method with it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ModMenuEntryAttribute : Attribute
    {
        public string MenuName { get; }
        public string? IsModMenuActivePropertyName { get; }

        public ModMenuEntryAttribute(string menuName, string? isModMenuActivePropertyName = null)
        {
            MenuName = menuName;
            IsModMenuActivePropertyName = isModMenuActivePropertyName;
        }
    }

    [HarmonyPatch(typeof(Program))]
    public static class ModMenuPatcher
    {
        private static bool Initialized = false;
        public static readonly List<ModEntry> Mods = new List<ModEntry>();

        // Struct to get the name of the Mod and code to be injected
        public struct ModEntry
        {
            public string Name { get; }
            public Action Callback { get; }
            public BoolRef? IsModMenuActive { get; }

            public ModEntry(string name, Action callback, BoolRef? isModMenuActive = null)
            {
                Name = name;
                Callback = callback;
                IsModMenuActive = isModMenuActive;
            }
        }

        /// <summary>
        /// Finds where to inject the Mod Menu item into the KSA Program
        /// </summary>
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
                        // Insert ldarg.1 to load the viewport parameter, then call InjectModMenu
                        codes.Insert(i + 4, new CodeInstruction(OpCodes.Ldarg_1));  // Load viewport
                        codes.Insert(i + 5, new CodeInstruction(OpCodes.Call, injectMethod));
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
        /// Initializes the Mod Menu by scanning all loaded assemblies for:
        /// 1. Methods decorated with [ModMenuEntry] attribute
        /// 2. Public static methods named "CreateModMenu" or "DrawModMenu"
        /// </summary>
        public static void Initialize()
        {
            if (Initialized) return;
            Initialized = true;

            DefaultCategory.Log.Info("ModMenu - Scanning for mod menu entries...");

            // Scan all loaded assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    // Skip system assemblies for performance
                    var assemblyName = assembly.GetName().Name;
                    if (assemblyName.StartsWith("System") ||
                        assemblyName.StartsWith("Microsoft") ||
                        assemblyName.StartsWith("netstandard") ||
                        assemblyName.StartsWith("mscorlib"))
                    {
                        continue;
                    }

                    foreach (var type in assembly.GetTypes())
                    {
                        try
                        {
                            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                                   BindingFlags.Static | BindingFlags.Instance))
                            {
                                BoolRef? isModMenuActiveRef = null;
                                string? menuName = null;
                                bool shouldAdd = false;

                                // Method 1: Look for [ModMenuEntry] attribute by name
                                var attribute = method.GetCustomAttributes(false)
                                    .FirstOrDefault(a => a.GetType().Name == "ModMenuEntryAttribute");

                                if (attribute != null)
                                {
                                    var menuNameProperty = attribute.GetType().GetProperty("MenuName");
                                    menuName = menuNameProperty?.GetValue(attribute) as string;

                                    // Resolve the BoolRef from the declared property/field name
                                    var isModMenuActiveName = attribute.GetType().GetProperty("IsModMenuActivePropertyName")?.GetValue(attribute) as string;

                                    if (!string.IsNullOrEmpty(isModMenuActiveName))
                                    {
                                        var prop = type.GetProperty(isModMenuActiveName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                                        var field = prop == null ? type.GetField(isModMenuActiveName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static) : null;

                                        if (prop != null && prop.CanWrite)
                                            isModMenuActiveRef = new BoolRef(() => (bool)prop.GetValue(null), v => prop.SetValue(null, v));
                                        else if (field != null)
                                            isModMenuActiveRef = new BoolRef(() => (bool)field.GetValue(null), v => field.SetValue(null, v));
                                        else
                                            DefaultCategory.Log.Warning($"ModMenu - Could not find or write to '{isModMenuActiveName}' on {type.FullName}");
                                    }

                                    shouldAdd = true;
                                }
                                // Method 2: Look for naming convention (public static methods)
                                else if (method.IsStatic && method.IsPublic &&
                                        (method.Name == "CreateModMenu" || method.Name == "DrawModMenu"))
                                {
                                    // Use the type name as the menu name
                                    menuName = type.Name;
                                    // Remove common suffixes
                                    menuName = menuName.Replace("Mod", "").Replace("Menu", "").Trim();
                                    if (string.IsNullOrEmpty(menuName))
                                        menuName = type.Name;
                                    shouldAdd = true;
                                }

                                if (shouldAdd && !string.IsNullOrEmpty(menuName))
                                {
                                    Action callback;

                                    if (method.IsStatic)
                                    {
                                        callback = (Action)Delegate.CreateDelegate(typeof(Action), method);
                                        DefaultCategory.Log.Info($"ModMenu - Found static method '{menuName}' in {type.FullName}.{method.Name}");
                                    }
                                    else
                                    {
                                        var instance = Activator.CreateInstance(type);
                                        callback = (Action)Delegate.CreateDelegate(typeof(Action), instance, method);
                                        DefaultCategory.Log.Info($"ModMenu - Found instance method '{menuName}' in {type.FullName}.{method.Name}");
                                    }

                                    Mods.Add(new ModEntry(menuName, callback, isModMenuActiveRef));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DefaultCategory.Log.Debug($"ModMenu - Could not scan type {type.FullName}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    DefaultCategory.Log.Debug($"ModMenu - Could not scan assembly {assembly.GetName().Name}: {ex.Message}");
                }
            }

            // Sort alphabetically by Mod Name
            Mods.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            DefaultCategory.Log.Info($"ModMenu - Initialized with {Mods.Count} mod(s)");
        }

        /// <summary>
        /// Manually adds your ImGui code to the Mod Menu. Your code will start in your Mod's SubMenu.
        /// This is kept for backwards compatibility, but using [ModMenuEntry] attribute is recommended.
        /// </summary>
        public static void AddToModMenu(string name, Action callback)
        {
            DefaultCategory.Log.Info($"ModMenu: Manually registering entry '{name}'");
            ModEntry mod = new ModEntry(name, callback);

            Mods.Add(mod);

            // Sorts alphabetically by Mod Name
            Mods.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Creates the ImGui Menu "Mods" and each of the added Mod's SubMenus
        /// </summary>
        private static void InjectModMenu(Viewport viewport)
        {
            if (!Initialized)
            {
                Initialize();
                foreach(var mod in Mods)
                {
                    if (mod.IsModMenuActive != null)
                    {
                        mod.IsModMenuActive.Value = true;
                    }
                }
            }

            try
            {
                // Begins the Menu Mods
                if (ImGui.BeginMenu("Mods", true))
                {
                    viewport.MenuBarInUse = true;
                    // Loops through and adds each of the Mods ImGui code to the menu
                    foreach (var mod in Mods)
                    {
                        if (ImGui.BeginMenu(mod.Name))
                        {
                            try
                            {
                                mod.Callback();
                            }
                            catch (Exception ex)
                            {
                                DefaultCategory.Log.Error($"ModMenu - Mod '{mod.Name}' Callback Error: {ex}");
                            }
                            ImGui.EndMenu();
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