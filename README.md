# FrancisMod

A Terraria tModLoader mod themed around the absurd, over-the-top "Francis" aesthetic. This project adds a set of custom enemies, a boss, weapons, and items while fitting into a Calamity-focused gameplay environment.

This repository is archived and kept as a historical snapshot of the mod's source code and assets.

## Overview

FrancisMod is a custom mod for Terraria built with C# and HLSL. The project includes custom content, item definitions, boss logic, and asset organization typical of a tModLoader mod. It was developed around the idea of a comedic and exaggerated "Francis" theme, with all art created in Piskel or Aseprite.

## Features

- 3 new enemies
  - Francis Slime
  - Francis Bot
  - Francis Floater (summons Francis Minions)
- 1 new boss
  - MG Francis
- 5 new weapons
  - Francis Sword
  - The Chair
  - Book of Francis
  - Fish Head Launcher
  - The Francis Slayer
- 8 new items
  - Francis Ore
  - Francis Bar
  - Francis Dust
  - Refined Francis Bar
  - MG Francis Boss Bag
  - MG Francis Relic
  - Skinny Pop
  - Mechanical Skinny Pop

## Music

The mod includes references to music used in the project:

- Red Sun from MGR
- TWISTED GARDEN (remix by Kuudray)

## Project Structure

- `Assets/` - Sprite sheets, textures, and other visual assets
- `Common/` - shared helper code and reusable mod logic
- `Content/` - gameplay content such as items, NPCs, projectiles, and bosses
- `Localization/` - localization files
- `Properties/` - project metadata files
- `FrancisMod.cs` - main mod class entry point
- `FrancisMod.csproj` - project configuration for tModLoader
- `build.txt` - mod metadata and dependency info
- `description.txt` - in-game description and feature summary

## Requirements

To build or run this mod, you will need:

- Terraria
- tModLoader
- .NET 6
- Calamity Mod

The project references the Calamity Mod assembly from a local `ModAssemblies` folder and declares it in `build.txt`.

## Building

1. Open the solution in Visual Studio or another C# IDE.
2. Ensure the tModLoader development environment is set up correctly.
3. Make sure the Calamity Mod DLL is available at the expected local path.
4. Build the solution.

## Notes

- The mod is designed to work as a tModLoader mod.
- It is archived and may not be actively maintained.
- This repository is best treated as a reference or historical artifact.

## Credits

- Francis for being a Francis
- The rest was made by Dolphe
- Art created in Piskel or Aseprite

## License

No explicit license file is included in this repository, so the project should be treated as all rights reserved unless otherwise stated by the author.

## Contact

For questions or updates about the mod, please see the repository owner and project history on GitHub.
