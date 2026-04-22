using System;

namespace ModMenu
{
    /// <summary>
    /// Attribute to mark methods that should appear in the Mod Menu.
    /// Decorate your menu method with this attribute to automatically register with ModMenu.
    /// </summary>
    /// <example>
    /// Basic usage:
    /// [ModMenuEntry("My Mod Name")]
    /// public static void DrawMenu()
    /// {
    ///     ImGui.Text("Hello World!");
    /// }
    ///
    /// With ModMenu active state tracking:
    /// public static bool IsModMenuActive { get; set; } = false;
    ///
    /// [ModMenuEntry("My Mod Name", isModMenuActivePropertyName: nameof(IsModMenuActive))]
    /// public static void DrawMenu()
    /// {
    ///     ImGui.Text("Hello World!");
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ModMenuEntryAttribute : Attribute
    {
        /// <summary>
        /// The name to display in the Mod Menu.
        /// </summary>
        public string MenuName { get; }

        /// <summary>
        /// Optional. The name of a public static bool property or field on the same class.
        /// ModMenu will set this to <c>true</c> while your menu is being drawn,
        /// and <c>false</c> when it is not. Useful for knowing when ModMenu is active.
        /// </summary>
        public string IsModMenuActivePropertyName { get; }


        /// <summary>
        /// The Attribute to add a ModMenu Entry
        /// </summary>
        /// <param name="menuName">The name to display in the Mod Menu.</param>
        /// <param name="isModMenuActivePropertyName">
        /// Optional. Name of a public static bool property or field on this class that ModMenu
        /// will set to <c>true</c> while drawing your menu, and <c>false</c> otherwise.
        /// Use <c>nameof()</c> to avoid magic strings. Example: nameof(IsModMenuActive)
        /// </param>
        public ModMenuEntryAttribute(string menuName, string isModMenuActivePropertyName = null)
        {
            MenuName = menuName;
            IsModMenuActivePropertyName = isModMenuActivePropertyName;
        }
    }
}