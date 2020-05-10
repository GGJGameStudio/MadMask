using System;

public static class Game
{
    public static string CurrentLevel = "LevelAB2";

    public static readonly string LevelDirectory = $"res://Scenes/Levels";

    public static string GetLevelFullPath(string levelName)
    {
        return $"{LevelDirectory}/{levelName}.tscn";
    }

    internal static string GetCurrentLevelFullPath()
    {
        return GetLevelFullPath(CurrentLevel);
    }
}