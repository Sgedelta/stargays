using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using YarnSpinnerGodot;

public partial class GameManager : Node
{
	public static GameManager Instance {get; private set;}

	public GameManager InstanceButForGD { get { return Instance;} }

	private LevelManager _levelManager;
	public LevelManager LevelManager { get { return _levelManager; } }

	public string FirstLevelName;

	//we should use godot arrays/dictionaries, but DialogueOptions are not Variant type!
	public Dictionary<string, DialogueOption> ValidInputs;
	public YarnTaskCompletionSource<DialogueOption> InputTaskCompletionSource;
	
	
	private Godot.RandomNumberGenerator rng = new Godot.RandomNumberGenerator();

	private PackedScene _questionBase = ResourceLoader.Load<PackedScene>("res://Scenes/Questions/question_base.tscn");

	private int _questionIndex = 0;

	private Godot.Collections.Array<QuestionSettings> _possibleQuestions = new Godot.Collections.Array<QuestionSettings>()
	{
		ResourceLoader.Load<QuestionSettings>("res://Resources/Questions/IsThatHowItHappenedYesNo.tres"),
		ResourceLoader.Load<QuestionSettings>("res://Resources/Questions/TryAgain.tres")
	};

	private List<QuestionSettings> _generatedQuestions = new List<QuestionSettings>();

   
	private Godot.Collections.Dictionary<string, PackedScene> _levels = new Godot.Collections.Dictionary<string, PackedScene> 
	{
		//{"empty",           ResourceLoader.Load<PackedScene>("res://Scenes/Levels/empty_level.tscn")},
		{"questions",       ResourceLoader.Load<PackedScene>("res://Scenes/Questions/question_base.tscn")},
		//{"firstLevel",      ResourceLoader.Load<PackedScene>("res://Scenes/Levels/TestLevel.tscn")},
		//{"goodDontForget",  ResourceLoader.Load<PackedScene>("res://Scenes/Levels/SecondTest.tscn")},
		{"game_over",        ResourceLoader.Load<PackedScene>("res://Scenes/game_over.tscn")},
		//{"goodConfession",  ResourceLoader.Load<PackedScene>("res://Scenes/Levels/ThirdTest.tscn")},
		//{"goodClarify",     ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FourthTest.tscn") },
		//{"goodChoices",     ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FifthTest.tscn") },
		{"Tell",			ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/TELL.tscn") }, 
		{"IMG_Gay",			ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/IMG/IMG_Gay.tscn")},
		{"IMG_Husband",		ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/IMG/IMG_Husband.tscn")},
		{"IMG_WantKids",	ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/IMG/IMG_WantKids.tscn")},
		{"IMG_NoKids",		ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/IMG/IMG_NoKids.tscn")},
		{"IMG_Change",		ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/IMG/IMG_Change.tscn") },
		{"IMG_Upset",		ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/IMG/IMG_Upset.tscn") },
		{"firstLevel",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FIRST/FIRST_Aigo.tscn") },
		{"FIRST_Baby",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FIRST/FIRST_Baby.tscn") },
		{"FIRST_Boys",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FIRST/FIRST_Boys.tscn") },
		{"FIRST_Dipper",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FIRST/FIRST_Dipper.tscn") },
		{"FIRST_Friend",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FIRST/FIRST_Friend.tscn") },
		{"FIRST_Gay",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FIRST/FIRST_Gay.tscn") },
		{"FIRST_Happy",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FIRST/FIRST_Happy.tscn") },
		{"FIRST_Okay",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FIRST/FIRST_Okay.tscn") },
		{"FIRST_Sorry",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FIRST/FIRST_Sorry.tscn") },
		{"FIRST_Tell",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FIRST/FIRST_Tell.tscn") },
		{"FIRST_What",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FIRST/FIRST_What.tscn") },
		{"FIRST_Years",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/FIRST/FIRST_Years.tscn") },
		{"ILG_Men",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/ILG/ILG_Men.tscn") },
		{"ILG_Quiet",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/ILG/ILG_Quiet.tscn") },
		{"ITIMG_Gay",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/ITIMG/ITIMG_Gay.tscn") },
		{"ITIMG_Raise",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/ITIMG/ITIMG_Raise.tscn") },
		{"ITIMG_Mother",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/ITIMG/ITIMG_Mother.tscn") },
		{"ITIMG_Love",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/ITIMG/ITIMG_Love.tscn") },
		{"ITILG_Man",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/ITILG/ITILG_Man.tscn") },
		{"NM_Sure",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/NM/NM_Sure.tscn") },
		{"NM_Ending",       ResourceLoader.Load<PackedScene>("res://Scenes/Levels/LoopLevels/NM/NM_Ending.tscn") }
	};

	public int FadeTime = 1;

	private PauseMenu _pauseMenu;

	//TODO: replace this with a game state enum if it ever does anything more than making sure you can't pause the main menu...
	private bool _gameStarted = false;
	public bool GameStarted { get { return _gameStarted; } }


	//====== DATA TRACKING ======
	private Godot.Collections.Dictionary<string, int> _dataNodeVisitedCount = new Godot.Collections.Dictionary<string, int>();
	private Godot.Collections.Dictionary<int, Godot.Collections.Array<string>> _dataLoopDialoguesTried = new Godot.Collections.Dictionary<int, Godot.Collections.Array<string>>();
	private Godot.Collections.Dictionary<int, Godot.Collections.Dictionary<string, string>> _dataLoopQuestionAnswers = new();
	private int _dataLoopCount = 0;



	public override void _Ready()
	{
		if(Instance == null)
		{
			Instance = this;
			Callable.From(SetupPause).CallDeferred();
			this.ProcessMode = ProcessModeEnum.Always;
		}
		else
		{
			GD.PrintErr("Two GameManagers Created! Deleting self.");
			QueueFree();
		}

		ValidInputs = new Dictionary<string, DialogueOption>();
	}

	private void SetupPause()
	{
		_pauseMenu = ResourceLoader.Load<PackedScene>("res://Scenes/pause_menu.tscn").Instantiate<PauseMenu>();
		GetTree().Root.AddChild(_pauseMenu);
	}

	/// <summary>
	/// Sets the internal data to a "new game" state, run when play is pressed
	/// </summary>
	public void StartNewGame()
	{
		//reset questions to first loop questions
		_questionIndex = 0;

		_generatedQuestions.Clear();
		_generatedQuestions.Add(ResourceLoader.Load<QuestionSettings>("res://Resources/Questions/FirstLoopQuestions/IsThatHowItHappenedNo.tres"));
		_generatedQuestions.Add(ResourceLoader.Load<QuestionSettings>("res://Resources/Questions/FirstLoopQuestions/TryAgainYes.tres"));

		//reset tracked data (SAVE THIS BEFORE CALLING NEW GAME UNLESS YOU WANT TO LOSE IT FOREVER!!)
		_dataLoopDialoguesTried.Clear();
		_dataNodeVisitedCount.Clear();
		_dataLoopQuestionAnswers.Clear();
		_dataLoopCount = 0;

		//tell the game we're started.
		_gameStarted = true;

	}

	public void ResetGameToMainMenu()
	{
		SaveGameDataToFile();
		GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
		_gameStarted = false;
	}

	[YarnCommand("EndGame")]
	public void EndGame()
	{
		LoadLevel("empty");
		GetTree().Root.GetNode<MainGame>("MainGame").AnimateToLargerPos();
	}

	public void SetActiveLevel(LevelManager newLevel)
	{
		string oldName = !IsInstanceValid(_levelManager) ? "[Manager Null or Deleted!]" : _levelManager.Name;
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

		if(name == "Tell") //Tell is always the first node of the loop - change this if that fact changes
		{
			_dataLoopCount++;
		}

		PackedScene newLevel;
		if (!_levels.TryGetValue(name, out newLevel))
		{
			GD.PrintErr($"[GM] Failed to load level {name} because level was not in levelDictionary!");
			return;
		}


		Tween fadeOut = GetTree().CreateTween();
		if (IsInstanceValid(_levelManager))
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

		//now do data tracking
		if(_dataNodeVisitedCount.ContainsKey(name))
		{
			_dataNodeVisitedCount[name] += 1;
		} else
		{
			_dataNodeVisitedCount.Add(name, 1);
		}

		
		

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

	public void LogAttemptedDialog(string attempt)
	{
		if(_dataLoopDialoguesTried.TryGetValue(_dataLoopCount, out Godot.Collections.Array<string> opts))
		{
			opts.Add(attempt);
		} else
		{
			_dataLoopDialoguesTried.Add(_dataLoopCount, new Godot.Collections.Array<string>() { attempt });
		}
	}

	public void LogQuestionAnswer(string question, string answer)
	{
		if (_dataLoopQuestionAnswers.TryGetValue(_dataLoopCount, out Godot.Collections.Dictionary<string, string> questions))
		{
			if (questions.ContainsKey(question))
			{
				questions[question] = answer;
			}
			else
			{
				questions.Add(question, answer);
			}
		} else
		{
			_dataLoopQuestionAnswers.Add(_dataLoopCount, new Godot.Collections.Dictionary<string, string>() { {question, answer } });
		}
	}

	public bool ShouldStarBeEnabled(Godot.Collections.Dictionary<string, int> reqs)
	{
		//check all requirements for any false ones
		foreach (var req in reqs)
		{
			//if the key is in nodes visited and the count is less than the required amount, return false
			if (_dataNodeVisitedCount.TryGetValue(req.Key, out int count))
			{
				if (count < req.Value) //nested loops so we can fail out of this case in case of passing and not hit next case
				{
					GD.Print($"[GM] Hiding star because required node {req.Key} needs {req.Value} and was only visited {count} times");
					return false;
				}
			}
			//if the key is not in node and it's positive (in case someone forgot to delete it or set it negative for some reason), return false 
			else if (req.Value > 0)
			{
				GD.Print($"[GM] Hiding star because required node {req.Key} was not found in _dataNodeVisitedCount dictionary and it must be visited");
				return false;
			}
			//otherwise continue on because this requirement has been passed
		}

		//otherwise all requirements (including case of no requirements) passed, return true
		return true;

	}

	private void SaveGameDataToFile()
	{
		GD.Print("res://Saves/" + Time.GetDateStringFromSystem() + "-" + Time.GetTimeStringFromSystem().Replace(":", "-") + ".txt");
		//we don't have to come up with good names that we can reopen because fuck you! we never load data
		using FileAccess saveFile = FileAccess.Open("res://Saves/" + Time.GetDateStringFromSystem() + "_" + Time.GetTimeStringFromSystem().Replace(":", "-") + ".txt", FileAccess.ModeFlags.Write);

		saveFile.StoreString(GetDataAsJSON());

		saveFile.Close();
	}

	private string GetDataAsJSON()
	{
		//create an object to hold all the highest levels of data we want
		Godot.Collections.Dictionary<string, string> jsonDatas = new Godot.Collections.Dictionary<string, string>();
		
		//add all the data
		jsonDatas.Add("nodes_visited", Json.Stringify(_dataNodeVisitedCount));
		jsonDatas.Add("loop_count", Json.Stringify(_dataLoopCount));
		jsonDatas.Add("dialogue_attempts_by_loop_count", Json.Stringify(_dataLoopDialoguesTried));
		jsonDatas.Add("question_answers_by_loop_count", Json.Stringify(_dataLoopQuestionAnswers));

		//construct a Json object with all the data
		string constructedJson = "{ ";
		foreach (string key in jsonDatas.Keys)
		{
			constructedJson += $"\"{key}\": {jsonDatas[key]}, ";
		}
		constructedJson = constructedJson.Substr(0, constructedJson.Length - 2) + " }"; //trim last space and comma, add ending }

		//return a Jsonified collection of the jsons
		return constructedJson;
	}


	//Yarn Data Retrieval (because I can't figure out how to query it in yarn for godot :[ )
	[YarnFunction("get_first_level_name")]
	public static string GetFirstLevelName()
	{
		return Instance.FirstLevelName;
	}


	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("ui_cancel"))
		{
			_pauseMenu.PauseGame();
		}
	}



}
