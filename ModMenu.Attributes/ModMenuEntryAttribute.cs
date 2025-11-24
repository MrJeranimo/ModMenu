using System;

namespace ModMenu
{
    /// <summary>
    /// Attribute to mark methods that should appear in the Mod Menu.
    /// Decorate your menu method with this attribute to automatically register with ModMenu.
    /// </summary>
    /// <example>
    /// [ModMenuEntry("My Mod Name")]
    /// public static void DrawMenu()
    /// {
    ///     ImGui.Text("Hello World!");
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ModMenuEntryAttribute : Attribute
    {
        /// <summary>
        /// The name that will appear in the Mods menu
        /// </summary>
        public string MenuName { get; }

        /// <summary>
        /// Creates a new ModMenuEntry attribute
        /// </summary>
        /// <param name="menuName">The name to display in the mod menu</param>
        public ModMenuEntryAttribute(string menuName)
        {
            MenuName = menuName;
        }
    }
}