using Godot;
using System;

public partial class QuestionManager : CanvasLayer
{

    public enum ButtonStates
    {
        ACTIVE,
        DISABLED,
        END_GAME
    }

    private QuestionSettings _activeQuestion;

    private Godot.Collections.Array<Control> _separatorsAndButtons;

    [Export] Label _questionText;
    [Export] HFlowContainer _container;
    [Export] VSeparator _separator;
    [Export] Button _button;

    private bool _firstShow = true;

    private bool _allowInput = true;


    public override void _Ready()
    {
        _separatorsAndButtons = new Godot.Collections.Array<Control>();

        DisplayQuestion(GameManager.Instance.LoadNextQuestion());

        _questionText.Modulate =Color.FromHtml("#ffffff00");
        
    }

    public void ModulateAll(float progress)
    {
        try
        {
            Color c = new Color(1, 1, 1, progress);
            _questionText.Modulate = c;
            _container.Modulate = c;
            GetNode<Sprite2D>("%QuestionsBG").Modulate = c;
        }
        catch (Exception ex)
        {
            //sometimes they are disposed and womp womp. nothin to do about it.
            return;
        }

    }


    public void DisplayQuestion(QuestionSettings newQuestion)
    {
        if(!_allowInput) { return; }
        _allowInput = false;

        //if we didn't get a question, don't do anything
        if(newQuestion == null)
        {
            ResetAndHide(true);
            return;
        }
        else
        {
            // if we did, make sure we hide the old stuff (if it exists)
            ResetAndHide(false);
        }

        // set the new questions
        _activeQuestion = newQuestion;

        //create the buttons
        for (int i = 0; i < _activeQuestion.Options.Count; i++)
        {
            Tween ctrlTween = GetTree().CreateTween();

            //create new separator
            VSeparator newSep = (VSeparator)_separator.Duplicate();
            _container.AddChild(newSep);
            newSep.Modulate = Color.FromHtml("#ffffff00");
            

            //create new button
            Button newBut = (Button)_button.Duplicate();
            _container.AddChild(newBut);
            newBut.Text = _activeQuestion.Options[i];
            newBut.Modulate = Color.FromHtml("#ffffff00");

            //set the button's state
            switch (_activeQuestion.OptionStates[i])
            {
                case ButtonStates.ACTIVE:
                    newBut.Disabled = false; //should be false by default, but jic
                    newBut.Pressed += () => 
                    {
                        DisplayQuestion(GameManager.Instance.LoadNextQuestion()); 
                    };
                    break;

                case ButtonStates.DISABLED:
                    newBut.Disabled = true;
                    break;

                case ButtonStates.END_GAME:
                    newBut.Disabled = false;
                    newBut.Pressed += () => 
                    {
                        //todo: replace with code to end the game!
                        DisplayQuestion(GameManager.Instance.LoadNextQuestion()); 
                    };
                    break;

            }

            _separatorsAndButtons.Add(newSep);
            _separatorsAndButtons.Add(newBut);

            ctrlTween.TweenCallback(Callable.From(() => {
                newSep.Visible = true;
                newBut.Visible = true;
            })).SetDelay(GameManager.Instance.FadeTime);

        }

        // add a final separator to spread evenly
        VSeparator finalSep = (VSeparator)_separator.Duplicate();
        _container.AddChild(finalSep);
        finalSep.Modulate = Color.FromHtml("#ffffff00");
        _separatorsAndButtons.Add(finalSep);

        //set visible and update text later
        Tween finalSepTween = GetTree().CreateTween();
            
        finalSepTween.TweenCallback(Callable.From(() => {
            finalSep.Visible = true;
            _questionText.Text = _activeQuestion.QuestionText;
        })).SetDelay(GameManager.Instance.FadeTime);

        finalSepTween.TweenCallback(Callable.From(() => { _allowInput = true; }));


        // now show new buttons
        ShowQuestions(_firstShow);
    }

    public void ResetAndHide(bool endGame = false)
    {
        foreach(Control ctrl in _separatorsAndButtons)
        {
            Tween ctrlTween = GetTree().CreateTween();


            ctrlTween.TweenProperty(ctrl, "modulate",
                    Color.FromHtml("#ffffff00"), GameManager.Instance.FadeTime);

            ctrlTween.TweenCallback(Callable.From(() => {
                ctrl.QueueFree();
            }));
        }
        _separatorsAndButtons.Clear();

        GetTree().CreateTween().TweenProperty(_questionText, "modulate", Color.FromHtml("#ffffff00"), GameManager.Instance.FadeTime);


    }



    public void ShowQuestions(bool skipDelay = false)
    {
        _firstShow = false;

        foreach (Control ctrl in _separatorsAndButtons)
        {
            //separators stay hidden
            if(ctrl is Button)
            {
                Tween ctrlTween = GetTree().CreateTween();

                ctrlTween.TweenProperty(ctrl, "modulate",
                    Color.FromHtml("#ffffffff"), GameManager.Instance.FadeTime).From(Color.FromHtml("#ffffff00"))
                    .SetDelay(skipDelay ? GameManager.Instance.FadeTime * 0.5f : GameManager.Instance.FadeTime * 2.5f);
            }
            
        }

        GetTree().CreateTween().TweenProperty(_questionText, "modulate",
                    Color.FromHtml("#ffffffff"), GameManager.Instance.FadeTime).From(Color.FromHtml("#ffffff00"))
                    .SetDelay(skipDelay ? GameManager.Instance.FadeTime * 0.5f : GameManager.Instance.FadeTime * 2.5f);

    }




}
