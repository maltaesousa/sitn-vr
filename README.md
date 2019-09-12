# SITNVR

SITNVR is a prototype of importing different kind of GIS data into a VR environment.

## File structure

    .
    ├── datasource                 # Raw sample data
    ├── import                     # Scripts and their output to be imported into unity
    ├── unity                      # Unity project
    │   ├── Assets                 # Unity standard asset folder
    │   │   ├── Audio
    │   │   ├── Editor             # Scripts meant to be used by Editor, will not be compiled
    │   │   ├── Down Town Pack     # From Asset Store
    │   │   ├── Free_SpeedTrees    # From Asset Store
    │   │   ├── Materials
    │   │   ├── Models             # Generated 3D objects from FME
    │   │   ├── Prefabs            # Custom prefabs
    │   │   ├── Scenes             # Scenes showing state of project at each release
    │   │   ├── Scripts            # Scripts used in game
    │   │   ├── SteamVR            # SteamVR from Asset Store
    │   │   ├── SteamVR_Input      # SteamVR from Asset Store
    │   │   ├── Street lights 1    # From Asset Store
    │   │   ├── Terrain            # Terrain and terrain layers
    │   │   ├── TextMesh Pro       # From Asset Store, better text
    │   │   └── Textures
    │   ├── Packages
    │   └── ProjectSettings
    ├── LICENSE
    └── README.md

## Requirements

Playing the prototype:
- Git LFS
- Unity >= 2019.1
- SteamVR

Process the data:
- FME >= 2019.1
- QGIS >= 3.6

## Playing the prototype

1. Clone this repository
1. Connect your VR device and make sure SteamVR is running
1. Open the Unity Hub, click add and browse the `unity` folder at the root of this project
1. In the Assets explorer, open the `Scenes` folder and double click on `Proto01`
1. Hit the play button in Unity and enjoy

## Documentation

[Integration documentation](../../wiki)
