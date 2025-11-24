# ModMenu

A modding framework for Kitten Space Agency that allows mods to easily add menu entries.

## For Mod Developers

### Installation

Add the NuGet package to your mod project:
```bash
dotnet add package ModMenu.Attributes
```

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

## For Players

1. Download `ModMenu.dll` from [Releases](https://github.com/yourusername/ModMenu/releases)
2. Place it in `Kitten Space Agency/Content/ModMenu/`
3. Restart the game

Any installed mods using ModMenu will automatically appear in the "Mods" menu.

## Repository Structure

- `src/ModMenu.Attributes/` - NuGet package with the `[ModMenuEntry]` attribute
- `src/ModMenu/` - Main mod DLL (or wherever your main mod code is)

## License

MIT