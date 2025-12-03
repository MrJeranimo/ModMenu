# ModMenu

A modding framework for Kitten Space Agency that allows mods to easily add menu entries.

## Example Mod

To see ModMenu in action, it is recommended to install [ModMenu.ExampleMod](https://github.com/MrJeranimo/ModMenu.ExampleMod) and put that mod in your KSA Content folder.

For developers it is also recommended to see how [ModMenu.ExampleMod](https://github.com/MrJeranimo/ModMenu.ExampleMod) uses the ModMenu.

## For Mod Developers

### Installation

Add the NuGet package to your mod project:
```bash
dotnet add package ModMenu.Attributes
```

or in Visual Studio go to Manage NuGet Packages -> Search using Github -> ModMenu.Attributes and install that. Once installed add the `using ModMenu;` to your Mod and add the `[ModMenuEntry("Mod Name")]` tag to which ever function you want to be called by ModMenu. If you add any ImGui code in the function, it will be drawn inside a submenu that is labeled with the `"Mod Name"` you put in the tag.

### Usage
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

### Building

!!!WARNING!!!

You MUST include the `ModMenu.Attributes.dll` file in your Mod folder otherwise the mod WILL CRASH on startup. If the `ModMenu.Attributes.dll` is not showing up when building add 

`<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>` to your `.csproj` file.

Example:
```csproj
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
 <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
</PropertyGroup>
```

Note that this will copy all of your NuGet Packages `.dll`s into the build folder. I currently do not know a better way to do this.

Once you have the built files, put your Mod's `.dll` and the `ModMenu.Attributes.dll` into your Mod's folder. Then make sure you have ModMenu installed and put in your `KSA/Content` folder and you can launch StarMap and see the Submenu for your mod.

## For Players

1. Download `ModMenu.zip` from [Releases](https://github.com/MrJeranimo/ModMenu/releases)
2. Extract it to `Kitten Space Agency/Content/`
3. Launch the game via StarMap

Any installed mods using ModMenu will automatically appear in the "Mods" menu.

## Repository Structure

- `/ModMenu.Attributes/` - NuGet package with the `[ModMenuEntry]` attribute
- `/ModMenu/` - Main mod DLL (or wherever your main mod code is)

## License

MIT
