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


    private Godot.Collections.Dictionary<string, PackedScene> _levels = new Godot.Collections.Dictionary<string, PackedScene> 
    {
        {"goodDontForget",  ResourceLoader.Load<PackedScene>("res://Scenes/Levels/SecondTest.tscn")},
        {"gameOver",        ResourceLoader.Load<PackedScene>("res://Scenes/game_over.tscn")},
        {"goodConfession",  ResourceLoader.Load<PackedScene>("res://Scenes/Levels/ThirdTest.tscn")},
        {"goodClarify",     ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FourthTest.tscn") },
        {"goodChoices",     ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FifthTest.tscn") }
        
    
    
    };

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

    [YarnCommand("LoadLevel")]
    public void LoadLevel(string name)
    {
        PackedScene newLevel; 
        if(!_levels.TryGetValue(name, out newLevel))
        {
            return;
        }

        if (_levelManager != null)
        {
            _levelManager.QueueFree();
        }

        Node loadedLevel = newLevel.Instantiate();
        GetTree().Root.AddChild(loadedLevel);
        
    }

}
