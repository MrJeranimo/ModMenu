# ModMenu.Attributes

Attributes for registering mods with ModMenu in Kitten Space Agency.

## Installation
```bash
dotnet add package ModMenu.Attributes
```
or install in Visual Studio under Manage NuGet Packages.

## Usage
```csharp
using ModMenu;
using Brutal.ImGui;

public class MyMod
{
    [ModMenuEntry("My Mod Name")]
    public static void DrawMenu()
    {
        ImGui.Text("Hello from my mod!");
        
        if (ImGui.Button("Click Me"))
        {
            Console.WriteLine("Button clicked!");
        }
    }
}
```

That's it! ModMenu will automatically discover and register your menu when the game starts.

## Requirements

- Kitten Space Agency
- ModMenu mod installed

## Documentation

Full documentation available at: https://github.com/yourusername/ModMenu

## License

MIT