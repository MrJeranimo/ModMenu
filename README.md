# ModMenu

A modding framework for Kitten Space Agency that allows mods to easily add menu entries.

## Example Mod

To see ModMenu in action, it is recommended to install [ModMenu.ExampleMod](https://github.com/MrJeranimo/ModMenu.ExampleMod) and put that mod in your KSA Content folder.

For developers it is also recommended to see how [ModMenu.ExampleMod](https://github.com/MrJeranimo/ModMenu.ExampleMod) uses the ModMenu.

## For Mod Developers

### Installation

Add the NuGet package to your mod project in Visual Studio, go to Manage NuGet Packages -> 

<img width="1491" height="617" alt="Screenshot 2025-12-09 231613" src="https://github.com/user-attachments/assets/fc328d9e-799d-4191-b597-7c4edcfafaa6" /> 

Click on the settings icon -> 

<img width="1120" height="561" alt="Screenshot 2025-12-09 231345" src="https://github.com/user-attachments/assets/c7bdfcad-d8c5-4e09-9356-9715317447ed" />

Click Add ->

<img width="489" height="544" alt="image" src="https://github.com/user-attachments/assets/38cc5147-ed54-4f08-a7fa-c17f5a4e2a68" />

Put in a name you want and for the source put in this URL: `https://nuget.pkg.github.com/MrJeranimo/index.json`. Then Click 'Save' ->

<img width="1062" height="372" alt="image" src="https://github.com/user-attachments/assets/bc202691-3a59-4361-b5f9-eaf5d078d5e0" />

You'll see the new Source. Go to back to the Manage NuGet Packages page ->

<img width="1481" height="615" alt="Screenshot 2025-12-09 232144" src="https://github.com/user-attachments/assets/d61d671e-ba23-4058-9b35-12e9191d5336" />

Change the source to the new ModMenu source you added or All -> 

<img width="1479" height="607" alt="Screenshot 2025-12-09 232306" src="https://github.com/user-attachments/assets/5e0719ec-dc92-47cb-b533-928e7f52c3b7" />

If you do the new source, it should be the only one.

If you do All you must search for ModMenu.Attributes

<img width="1485" height="606" alt="image" src="https://github.com/user-attachments/assets/37132d00-fa78-429a-8bf6-1725604f2370" />

 Once installed add the `using ModMenu;` to your Mod and add the `[ModMenuEntry("Mod Name")]` tag to which ever function you want to be called by ModMenu. If you add any ImGui code in the function, it will be drawn inside a submenu that is labeled with the `"Mod Name"` you put in the tag.

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

You MUST include the `ModMenu.Attributes.dll` file in your Mod folder otherwise the mod WILL CRASH on startup. You can either download the `ModMenu.Attributes.dll` in the releases, or if the `ModMenu.Attributes.dll` is not showing up when building add 

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
- `/ModMenu/` - Main mod DLL (or wherever your main mod code is)

## License

MIT
