using Godot;
using System;
using System.Collections.Generic;
using YarnSpinnerGodot;

public partial class GameManager : Node
{
    public static GameManager Instance {get; private set;}

    private LevelManager _levelManager;
    public LevelManager LevelManager { get { return _levelManager; } }

    //we should use godot arrays/dictionaries, but DialogueOptions are not Variant type!
    public Dictionary<string, DialogueOption> ValidInputs;
    public YarnTaskCompletionSource<DialogueOption> InputTaskCompletionSource;

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

        ValidInputs = new Dictionary<string, DialogueOption>();
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
