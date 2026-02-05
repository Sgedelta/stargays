using Godot;
using System;

public partial class GameManager : Node
{
    public static GameManager Instance {get; private set;}

    private LevelManager _levelManager;
    public LevelManager LevelManager { get { return _levelManager; } }


    public Godot.Collections.Array<string> ValidInputs;

    public override void _Ready()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            GD.PrintErr("Two GameManagers Created! Deleting self.");
            QueueFree();
        }

        ValidInputs = new Godot.Collections.Array<string>();
    }

    public override void _Process(double delta)
    {
        
    }

    public void SetActiveLevel(LevelManager newLevel)
    {
        string oldName = _levelManager == null ? "[Manager Null or Deleted!]" : _levelManager.Name;
        GD.Print($"[GM] Setting {newLevel.Name} to the active level manager. Previous one was {oldName}");
        _levelManager = newLevel;
    }

}
