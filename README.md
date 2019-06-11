# Mecatronica
This is a personal project. It's a city builder game with procedural terrain generation. I'm currently working on the first playable prototype and you can find a lot of code for managing. But you can navigate on the map and ask for container drop and rover drop (on the TerrainGenerator scene).
You can find other prototyping stuff like shader and window editor tool.

## Scene description:
1. TerrainGenerator : Endless terrain generation (mesh creation, level of detail)
2. UIScene : Contains a simple UI window test (drag'n'drop, resize, movable window)
3. SampleScene : Shader test
4. CoreScene : Contain manager (automaticly loaded when launching other scene)

## Interesting stuff
* Check the script for [endless terrain generation](https://github.com/viveleltsi/Mecatronica/blob/master/Assets/Scripts/Terrain/EndlessTerrainGenerator.cs)
* [Custom property attribute](https://github.com/viveleltsi/Mecatronica/blob/master/Assets/Modules/Manager/Decorator/LinkToScriptableObjectDrawer.cs) for linking data to monobehaviour (Descriptor for example)
* [ScreneScriptBehaviour](https://github.com/viveleltsi/Mecatronica/blob/master/Assets/Modules/SceneSmoothLoader/Scripts/SceneScriptBehaviour.cs) to enabling all monobehaviour on the scene after loading
* [WindowUIBehaviour](https://github.com/viveleltsi/Mecatronica/blob/master/Assets/Modules/UIWindow/Scripts/WindowUIBehaviour.cs) script for drag'n'drop, resize, move a UI window on canvas.

## I see nothing on the Unity Scene view !
I use a custom script to disabling all monobehaviour on scene when it's saved. It's because when I load async the scene I don't want the game freeze on the last 10% of the loading (even if it's "async"). So I use a custom script to enable all monobehaviour after the scene is loaded. This script watch how much time is used for enabling other script and manage enable them on multiple frame to be sure the game wont freeze. If you want to see all monobehaviour activated when working on a scene you can right click on the *[ScreneScriptBehaviour](https://github.com/viveleltsi/Mecatronica/blob/master/Assets/Modules/SceneSmoothLoader/Scripts/SceneScriptBehaviour.cs)* script on "*SceneScriptAutoGenerated*" GameObject and choose "*All Visible*". It's very usefull to see the UI for example.

## What's next ?
I'm working now on the localization module (I want to make it as independant as possible) and resources on the map.
