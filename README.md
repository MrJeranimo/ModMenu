# ModMenu

A modding framework for Kitten Space Agency that allows mods to easily add menu entries.

## Example Mod

To see ModMenu in action, it is recommended to install [ModMenu.ExampleMod](https://github.com/MrJeranimo/ModMenu.ExampleMod) and put that mod in your KSA Content folder.

For developers it is also recommended to see how [ModMenu.ExampleMod](https://github.com/MrJeranimo/ModMenu.ExampleMod) uses the ModMenu.

## For Mod Developers

There are two ways to use ModMenu, the first way is a looser method, more dependent on the Mod creator and gives the general user more flexibility. The second is a stricter method that is more dependent on the user and gives the Mod creater less to worry about.

1) Use the ModMenu.Attributes NuGet Package and the `[ModMenuEnty()]` attribute as shown below.
2) Declare ModMenu a non-optional dependency under a `[StarMap]` tag in your mod.toml. Instructions [here](https://github.com/StarMapLoader/StarMap/tree/main/StarMap.API#dependencies).

### Installation Using NuGet

Add the NuGet package to your mod project in Visual Studio, go to Manage NuGet Packages -> 

Search for ModMenu.Attributes.

<img width="1485" height="606" alt="image" src="https://github.com/user-attachments/assets/37132d00-fa78-429a-8bf6-1725604f2370" />

Once installed add the `using ModMenu;` to your Mod and add the `[ModMenuEntry("Mod Name")]`tag , or `[ModMenuEntry("Mod Name", nameof(some_boolean)]` tag to which ever function you want to be called by ModMenu. If you add any ImGui code in the function, it will be drawn inside a submenu that is labeled with the `"Mod Name"` you put in the tag. If you include the `nameof(some_boolean)`, ModMenu will automatically set that boolean to `true` once it has initalized.

### Usage using NuGet
```csharp
using ModMenu;

public class MyMod
{
    [ModMenuEntry("My Mod Name")]
    public static void DrawMenu()
    {
        ImGui.Text("Hello World!");
    }
}
```
or
```csharp
using ModMenu;

public class MyMod
{
    public static bool _isModMenuActive { get; set; } = false;

    [ModMenuEntry("My Mod Name", nameof(_isModMenuActive))]
    public static void DrawMenu()
    {
        ImGui.Text("Hello World!");
    }
}
```

The second way will give you a boolean that ModMenu will set to true once ModMenu has activated. This will let you know whether or not the user has ModMenu enabled/installed so you can create your own ImGui window if you need to.

### Building using NuGet

> [!Warning]
> If you are using the `[ModMenuEntry()]` attribute, you MUST include the `ModMenu.Attributes.dll` file in your Mod folder otherwise the mod WILL CRASH on startup. You can download the `ModMenu.Attributes.dll` in the releases.

Once you have the built files, put your Mod's `.dll` and the `ModMenu.Attributes.dll` into your Mod's folder. Then make sure you have ModMenu installed and put in your `Documents/My Games/Kitten Space Agency/mods` folder and you can launch StarMap and see the Submenu for your mod.

### Usage by Dependency

```csharp
using ModMenu

public class MyMod {
    public static void RegisterModMenu() {
        ModMenu.AddToModMenu("Mod Name", DrawMenu);
    }

    public static void DrawMenu() {
        ImGui.Text("Hello World!");
    }
}
```

The `RegisterModMenu()` function will add an entry to ModMenu where the submenu will be populated by the `DrawMenu()` function.

## For Players

1. Download `ModMenu.zip` from [Releases](https://github.com/MrJeranimo/ModMenu/releases)
2. Extract it to `Documents/My Games/Kitten Space Agency/mods`
3. Add to the `manifest.toml` in `Documents/My Games/Kitten Space Agency/` for Windows.
```toml
[[mods]]
id = "ModMenu"
enabled = true
```
4. Launch the game via StarMap

Any installed mods using ModMenu will automatically appear in the "Mods" menu.

## Repository Structure

- `/ModMenu.Attributes/` - NuGet package with the `[ModMenuEntry]` attribute
- `/ModMenu/` - Main mod DLL

## License

MIT
