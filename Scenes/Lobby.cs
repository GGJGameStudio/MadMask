using Godot;
using System;
using System.Collections.Generic;
using SystemPath = System.IO.Path;

public class Lobby : Node
{
    private const string LevelDirectoryPath = "res://Scenes/Levels";
    private readonly List<string> levelList = new List<string>();
    private ItemList itemList;

    public override void _Ready()
    {
        var levelDirectory = new Directory();
        levelDirectory.Open(LevelDirectoryPath);
        levelDirectory.ListDirBegin(true, true);

        this.itemList = this.GetNode<ItemList>("ItemList");

        while (true)
        {
            var file = levelDirectory.GetNext();

            if (string.IsNullOrEmpty(file))
            {
                break;
            }

            if (!file.EndsWith(".tscn"))
            {
                continue;
            }

            GD.Print($"Discovered scene: {file}");

            this.itemList.AddItem(SystemPath.GetFileNameWithoutExtension(file));
            this.levelList.Add(file);
        }

        this.itemList.Select(0);
        this.itemList.GrabFocus();
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionPressed("ui_accept"))
        {
            var selectedLevel = (string)this.levelList[this.itemList.GetSelectedItems()[0]];

            var levelPath = SystemPath.Combine(LevelDirectoryPath, selectedLevel);

            this.GetTree().ChangeScene(levelPath);
        }
    }
}
