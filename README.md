# sitn-vr

sitn-vr is a prototype of importing different kind of GIS data into a VR environment.

## File structure

    .
    ├── datasource        # Raw data
    ├── documentation     # Instructions to process data
    ├── import            # Scripts and their output to import data into unity
    ├── unity             # Unity project
    ├── LICENSE
    └── README.md

## Requirements

Playing the prototype:
- Unity >= 2019.1
- SteamVR

Process the data:
- FME >= 2019.0
- QGIS >= 3.6

## Playing the prototype

1. Clone this repository
1. Connect your VR device and make sure SteamVR is running
1. Open the Unity Hub, click add and browse the `unity` folder at the root of this project
1. In the Assets explorer, open the `Scenes` folder and double click on `Proto01`
1. Hit the play button in Unity and enjoy
