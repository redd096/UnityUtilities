# Unity Utilities

A collection of useful utilities created from various projects, designed to be used as a Unity package.

### Installation
This project is available as a Unity Package, which can be installed via the Unity Package Manager.

1. In Unity, go to **Window > Package Manager**.
2. Click the `+` button in the top-left corner.
3. Select **Add package from git URL...**.
4. Paste the following URL:
   `https://github.com/redd096/UnityUtilities.git?path=/Packages/com.redd096.redd096-utilities`

The package will be installed in your project's `Packages/com.redd096.redd096-utilities` directory.

---

## Package Contents

The package is organized into several main folders:

* **ExamplePrefabs**: Ready-to-use prefabs that can be dragged and dropped directly into your scene.
* **Samples~**: Samples scenes to download from package manager
* **Scripts**: The core C# scripts for the package's functionalities.
* **Shaders**: A small collection of custom shaders.
* **TODO.txt**: A list of planned features and improvements, primarily for the **Pathfinding** and **SaveManager** systems.

---

## Modules

The package's main functionalities are split into several modules.

### Attributes
Custom Unity `PropertyAttribute` and helper scripts to extend and customize the Unity editor.

### CsvImporter
This module provides a system for downloading and parsing CSV files.

* **Core Script**: The main script is `CsvImporter.cs`, which handles all core functionality.
* **Editor Windows**:
    * Access the windows from the Unity menu: **Tools > redd096 > CSV Importer**.
    * **Download CSV**: Allows you to enter a URL to download a CSV file to a specified folder.
        * Buttons for `DataPath` and `PersistentDataPath` are provided for quick path selection.
        * The **Add Element** button lets you keep multiple urls and paths in memory.
    * **CSV Reader**: A window for parsing and reading a downloaded CSV file.
* **Examples**:
    * An example script, `ExampleSimpleCsvImporterAndReader.cs`, demonstrates how to use `CsvImporter.cs`.
    * You can see the results under **Tools > redd096 > CSV Importer > Examples**.
    * The script contains three main functions: downloading a CSV, creating a `ScriptableObject` from a CSV, and a combined download and creation method.
    * **Helper Functions**: The `helpers` are demonstrated for creating and updating `ScriptableObject` and `Prefabs` with data from a CSV, eliminating the need for manual loops.

### DialogueSystem
For now just a collection of helpers for the **PixelCrushers Dialogue System** asset. 

* **Note**: This module requires the `PIXELCRUSHER_DIALOGUESYSTEM` Scripting Define Symbol to be created.

### IconGenerator
A simple utility for generating icons.

### Main
A miscellaneous collection of scripts used in various projects, including:
* `ScriptableObject` and `Controller` scripts for easily use and keep track of project files and layers.
* **Camera Shake**.
* **Save Manager**.
* **Sound Manager**.
* Other singleton scripts.

### Network
An interface designed to provide a framework for working with various networking solutions (e.g., **Fishnet**, **Mirror**, **Photon**) and transports (e.g., **Steam**, **EOS**).

* **Note**: This module relies on several **Scripting Define Symbols**. Some are **auto-generated** when you import a specific package (e.g., `STEAMWORKS_NET` for Steamworks or `FISHNET` for Fishnet), while others are **globally defined** (e.g., `DISABLESTEAMWORKS` for disabling the Steamworks integration) or are **custom ones** you must create yourself (e.g., `PURRNET` for Purrnet).

### NodesGraph
An implementation of a `GraphNode` using the Unity Editor's `GraphView` API.
* An example of its implementation can be found in a separate project: [HorrorGame on GitHub](https://github.com/redd096/HorrorGame/tree/main/Assets/_Project/Scripts/GraphsEditor)

## OLD
This section contains legacy scripts that are **no longer actively used or maintained**, but are kept in the package for reference or backward compatibility.
This folder mainly exists as an archive and as a source of ideas or code snippets that may still be useful in specific cases.

## Singletons
A collection of different **Singleton patterns** commonly used in Unity projects.

This module provides multiple implementations to cover different use cases and preferences, including:

* **Singleton** – Basic class with `.instance` static variable. Uses `FindObjectOfType` to locate an existing instance in the scene if still not set, otherwise it is set in `Awake`.
* **SimpleInstance** – A minimal version, providing only the `.instance` static variable, without `DontDestroyOnLoad` and without destroying other copies in the scene.
* **LazySingleton** – Automatically creates the singleton instance if it does not exist.
* **StaticSingleton** – Instead of inheriting from `Singleton`, you can set an element as a singleton (`.instance` static variable and `DontDestroyOnLoad`) by calling a static function.

These scripts are meant to be used as base classes or helpers for creating manager-type objects (e.g. AudioManager, GameManager, SaveManager) that must exist only once in a scene or across scenes.

### StateMachine
A generic state machine system.
* You can create serializable `IState` objects to view states directly in the Inspector or attach `IState` to a `MonoBehaviour` to use them as `MonoBehaviour` states.

### UIControl
A set of scripts for controlling and optimizing Unity UI.
* **Custom Button**: A button with multiple target graphics (e.g. for color changes).
* **Dynamic Layout**: Scripts for using `ContentSizeFitter` and `LayoutElement` with percentages and for creating dynamic `GridLayoutGroup` layouts instead of fixed cell sizes.
* **OptimizeEventSystem**: A script that optimizes menu navigation for gamepads.
* **PredictionSlider**: A script for creating health bars that show predicted damage (turning red on hit) or healing (turning green).

### v1
This folder contains various components used in my previous games, including a **StateMachine**, **Pathfinding**, and various game components for both 3D and 2D top-down games.
* This also includes `PlayerController` and `PlayerPawn` scripts.

### v2
An updated version of the `v1` components, where `MonoBehaviour` is replaced with a pure C# script approach.
* Only the `Player` script is a `MonoBehaviour`; all other components are pure C# scripts called by the player.
* This version introduces the concept of `IGameObjectRD` (the player) and `IComponentRD` (the game components).
* The `Examples` folder shows an `ExamplePlayer` with various serializable `IComponentRD` scripts visible in the Inspector.
