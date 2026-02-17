using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using YarnSpinnerGodot;

public partial class GameManager : Node
{
	public static GameManager Instance {get; private set;}

	private LevelManager _levelManager;
	public LevelManager LevelManager { get { return _levelManager; } }

    //we should use godot arrays/dictionaries, but DialogueOptions are not Variant type!
    public Dictionary<string, DialogueOption> ValidInputs;
    public YarnTaskCompletionSource<DialogueOption> InputTaskCompletionSource;
    
    
    private Godot.RandomNumberGenerator rng = new Godot.RandomNumberGenerator();

    private PackedScene _questionBase = ResourceLoader.Load<PackedScene>("res://Scenes/Questions/question_base.tscn");

    private int _questionIndex = 0;

    private Godot.Collections.Array<QuestionSettings> _possibleQuestions = new Godot.Collections.Array<QuestionSettings>()
    {
        ResourceLoader.Load<QuestionSettings>("res://Resources/Questions/IsThatHowItHappenedYesNo.tres"),
        ResourceLoader.Load<QuestionSettings>("res://Resources/Questions/AreYouHappy.tres")
    };

    private List<QuestionSettings> _generatedQuestions = new List<QuestionSettings>();

   
	private Godot.Collections.Dictionary<string, PackedScene> _levels = new Godot.Collections.Dictionary<string, PackedScene> 
	{
        {"questions",       ResourceLoader.Load<PackedScene>("res://Scenes/Questions/question_base.tscn")},
		{"firstLevel",      ResourceLoader.Load<PackedScene>("res://Scenes/Levels/TestLevel.tscn")},
		{"goodDontForget",  ResourceLoader.Load<PackedScene>("res://Scenes/Levels/SecondTest.tscn")},
		{"gameOver",        ResourceLoader.Load<PackedScene>("res://Scenes/game_over.tscn")},
		{"goodConfession",  ResourceLoader.Load<PackedScene>("res://Scenes/Levels/ThirdTest.tscn")},
		{"goodClarify",     ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FourthTest.tscn") },
		{"goodChoices",     ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FifthTest.tscn") },
		{"Tell",			ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/TELL.tscn") }, 
		{"IMG_Gay",			ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/IMG/IMG_Gay.tscn")},
		{"IMG_Husband",		ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/IMG/IMG_Husband.tscn")},
		{"IMG_WantKids",	ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/IMG/IMG_WantKids.tscn")},
		{"IMG_NoKids",		ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/IMG/IMG_NoKids.tscn")},
		{"IMG_Change",		ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/IMG/IMG_Change.tscn") },
        {"IMG_Upset",		ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/IMG/IMG_Upset.tscn") }
    };

	public int FadeTime = 1;

    public override void _Ready()
    {
        if(Instance == null)
        {
            Instance = this;
            StartNewGame();
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

    /// <summary>
    /// Sets the internal data to a "new game" state, (currently) run on startup
    /// </summary>
    public void StartNewGame()
    {
        _questionIndex = 0;

        _generatedQuestions.Clear();
        _generatedQuestions.Add(ResourceLoader.Load<QuestionSettings>("res://Resources/Questions/IsThatHowItHappenedNo.tres"));
        _generatedQuestions.Add(ResourceLoader.Load<QuestionSettings>("res://Resources/Questions/AreYouHappyNo.tres"));

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
        //check of questions to hide the 
        if(name == "questions")
        {
            GetNode<CanvasLayer>("../MainGame/YarnSpinnerCanvasLayer").Visible = false;
        }
        else
        {
            GetNode<CanvasLayer>("../MainGame/YarnSpinnerCanvasLayer").Visible = true;
        }

        PackedScene newLevel;
		if (!_levels.TryGetValue(name, out newLevel))
		{
			GD.PrintErr($"[GM] Failed to load level {name} because level was not in levelDictionary!");
			return;
		}


		Tween fadeOut = GetTree().CreateTween();
		if (_levelManager != null)
		{
			Node oldLevel = _levelManager;
			fadeOut.TweenProperty(_levelManager, "modulate", Color.FromHtml("ffffff00"), FadeTime).From(Color.FromHtml("ffffffff"));
			fadeOut.TweenCallback(Callable.From(() => { 
                oldLevel.QueueFree();
                if (_levelManager == oldLevel)
                {
                    _levelManager = null;
                }
            }));
		}

		Node loadedLevel = newLevel.Instantiate();

		GetTree().Root.GetNode("MainGame").AddChild(loadedLevel);


		
		

	}

	public void FadeInCurrentStars()
	{
		_levelManager.ShowStars();
	}


    public void GenerateQuestionList()
    {
        //clear the old list and reset where we are
        _generatedQuestions.Clear();
        _questionIndex = 0;

        //shuffle array order the _possibleQuestions correctly. 
        //  this randomizes order of equally "ordered" questions. Not great but like. fuck effeciency this does not run a lot.
        QuestionSettings[] shuffledSettings = _possibleQuestions.OrderBy(x => rng.RandiRange(0, 100)).ToArray();
                           shuffledSettings = _possibleQuestions.OrderBy(_possibleQuestions => _possibleQuestions.QuestionOrder).ToArray();

        //roll for each question and add them
        foreach (var question in _possibleQuestions)
        {
            if(rng.Randf() <= question.ChanceToPick)
            {
                _generatedQuestions.Add(question);
            }
        }

	}

    /// <summary>
    /// either passes a QuestionSettings to the Level Manager which calls this, OR returns null if there are no questions left
    /// </summary>
    /// <returns></returns>
    public QuestionSettings LoadNextQuestion()
    {
        //we have questions
        if(_questionIndex < _generatedQuestions.Count)
        {
            _questionIndex++;
            return _generatedQuestions[_questionIndex-1];
        }
        //we're out of questions now - go back to the start for now
        else
        {
            GenerateQuestionList();
            LoadLevel("Tell");
            //we need to manually fade out questions, because LoadLevel attempts to fade out the prev level manager
            //  (this is a terrible way to do this, and not at ALL DRY. Womp Womp! three week project)
            Tween fadeOut = GetTree().CreateTween();
            QuestionManager questionsLevel = (QuestionManager)GetNode("/root/MainGame/QuestionBase");
            fadeOut.TweenMethod(Callable.From<float>((x) => { questionsLevel.ModulateAll(x); }), 1.0f, 0.0f, FadeTime);
            fadeOut.TweenCallback(Callable.From(() => { questionsLevel.QueueFree(); }));
        }
        return null;
    }
}
