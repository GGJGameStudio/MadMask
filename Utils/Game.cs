using System;
using Godot;

public static class Game
{
    private static readonly PackedScene LevelProxyFactory = GD.Load("res://Scenes/LevelProxy.tscn") as PackedScene;

    public static readonly string LevelDirectory = $"res://Scenes/Levels";

    public static string CurrentLevel { get; private set; } = "LevelAB3";

    public static string GetLevelFullPath(string levelName)
    {
        return $"{LevelDirectory}/{levelName}.tscn";
    }

    public static string GetCurrentLevelPath()
    {
        return GetLevelFullPath(CurrentLevel);
    }

    public static void LoadLevel(SceneTree tree, string levelName)
    {
        GD.Print($"Loading level {levelName}");

        CurrentLevel = levelName;

        tree.ChangeSceneTo(LevelProxyFactory);
    }

    public static void GetBackToLobby(SceneTree tree)
    {
        tree.ChangeScene("res://Scenes/Lobby.tscn");
    }

    public static void ReloadCurrentLevel(SceneTree tree)
    {
        LoadLevel(tree, CurrentLevel);
    }
}