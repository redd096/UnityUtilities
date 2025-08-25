# UnityUtilities
a collections of utilities I created from various projects I worked on

This project is a package, so you can find it in:
Packages/com.redd096.redd096-utilities

It's possible to download by unity PackageManager:
https://github.com/redd096/UnityUtilities.git?path=/Packages/com.redd096.redd096-utilities

Inside the package folder we can find:
- ExamplePrefabs: just a folder to easily drag-and-drop prefabs in your scene
- Scripts: the main folder for this package
- Shaders: few shaders I made
- TODO.txt: a list of TODO (mainly PathFinding and SaveManager) 

# The package is splitted like this:

Attributes:
- various unity PropertyAttribute and helpers I made to customize the editor

CsvImporter:
- the main script is the CsvImporter.cs that user can use to download and read csv
- inside Helpers and Parse folders is possible to find and edit the logic of the CsvImporter
- WindowEditor contains two windows I made to download a generic Csv and read it
    - User can access it from Tools > redd096 > CSV Importer
    a. prova
  b. Download CSV open a window where user can put a CSV link and a folder where to download it
    b.1. DataPath and PersistentDataPath are buttons to automatically set the a folder path
    b.2. DownloadCsv is the button to download it
    b.3. Is possible to keep in memory more files at time with the Add Element button and the dropdown to the left
  c. CSV Reader open a window where user can parse and read a downloaded CSV
- Example contains ExampleSimpleCsvImporterAndReader, a script where user can read how to use CSVimporter.cs
  a. User can access it from Tools > redd096 > CSV Importer > Examples
  b. Inside the script there are 3 functions: just download a CSV, Create an example ScriptableObject from a downloaded csv, Download and Create ScriptableObject in one call
  c. In "example scriptable object with helper" region, there is a way to use helpers to create ScriptableObjects instead of create a for cycle every time
  d. In "example update (not create) prefabs with helper" region, there is a way to update prefabs with helpers

DialogueSystem:
- for now just some helpers I made for PixelCrushers DialogueSystem
- Note: this use a ScriptDefineSymbol PIXELCRUSHER_DIALOGUESYSTEM created by me

IconGenerator:
- an icon generator I made, this is not so usefull

Main:
- contains various miscellaneous scripts I used in my projects (from ScriptableObject and controllers to keep track of every file or layer used in the project, to CameraShake scripts, SaveManager, SoundManager, and various other singletons)

Network:
- An interface I made to move fast between various Network frameworks (Fishnet, Mirror, Photon, etc...) and transports (Steam, EOS, etc...)
- Note: they use vary ScriptDefineSymbol
  a. some are auto-generated when improt the packages, like STEAMWORKS_NET for Steamworks or FISHNET for fishnet
  b. others are used globally, like DISABLESTEAMWORKS for Steamworks
  c. others are created by me, like PURRNET for Purrnet

NodesGraph:
- A GraphNode with UnityEditor's GraphView
  a. Is possible to see a way to implement it in [https://github.com/redd096/HorrorGame](https://github.com/redd096/HorrorGame/tree/main/Assets/_Project/Scripts/GraphsEditor)

OLD:
- obsolete scripts

StateMachine:
- a generic statemachine
- user can create Serializable IState to show states in inspector, or add IState to a MonoBehaviour to have states as MonoBehaviour

UIControl:
- various scripts for unity UI (for example CustomButton with more than one image to change color to, ContentSizeFitter and LayoutElement in percentages, Dynamic GridLayoutGroup instead of fixed cell size)
- this contains also OptimizeEventSystem, the script to make menus work with Gamepad too
- PredictionSlider, the script used to make the HealthBar red when get hit and green when healed

v1:
- various components I used in my games, like StateMachine, Pathfinding, and various Game Components for 3D and TopDown 2D games
- this contains also PlayerController and PlayerPawn

v2:
- a version of v1 components, where instead of be everything a MonoBehaviour, everything is a pure C# script. And Update and unity events are only on the Player and it call the linked function on every component
- there is the conception of IGameObjectRD (our Player) and IComponentRD (our game components)
- in the Examples folder we can see an ExamplePlayer with various IComponentRD in inspector as Serializable scripts
