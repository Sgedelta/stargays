using Godot;
using System;

public partial class Star : Area2D
{
	[Export] public string ExactKey = "";
	[Export] public AnimatedSprite2D AnimatedSprite = null;
	[Export] private Label _label;


	private RandomNumberGenerator rng;

    //a dictionary of node names and the amount of times they need to be visited for this star to be enabled. If empty, star will start enabled. use Tell to count # of loops (1 indexed)
    [Export] private Godot.Collections.Dictionary<string, int> _requiredNodeAmounts = new Godot.Collections.Dictionary<string, int>();

	public override void _Ready()
	{
		rng = new RandomNumberGenerator();
		AnimatedSprite.Play(AnimatedSprite.SpriteFrames.GetAnimationNames()[rng.RandiRange(0, AnimatedSprite.SpriteFrames.GetAnimationNames().Length - 1)]);
		_label.Text = ExactKey;

        Visible = GameManager.Instance.ShouldStarBeEnabled(_requiredNodeAmounts);

	}

	public override void _Process(double delta)
	{

	}

    //a helper method for selecting the star, so multiple input paths can route to here. In case we want keyboard support and to stay DRY
    public void SetStarSelected(bool select)
    {
        if(select)
        {
            GameManager.Instance.LevelManager.SelectStar(this);
        }
        else
        {
            GameManager.Instance.LevelManager.DeselectStar(this);
        }
    }

	//handles mouse input from input_event signal
	private void CustomInput(Node view, InputEvent @event, int shapeInd)
	{
		switch (@event)
		{
			case InputEventMouseButton eventButton:
				HandleMouseButton(view, eventButton, shapeInd);
				break;

			case InputEventMouseMotion mouseMotion:
				// do nothing rn. Don't think we'll need anything, but do it here jic so we can warn correctly
				break;

            default:
                GD.PushWarning($"Star {Name} recieved input that was not mousebutton or mousemotion. Nothing being done.");
                break;
            
        };
    }
    
    private void HandleMouseButton(Node view, InputEventMouseButton @event, int shapeInd)
    {
        if(@event.IsAction("StarSelect") && @event.Pressed)
        {
            SetStarSelected(true);
        }
    }

    private void HandleMouseEnter()
    {
        if (GameManager.Instance.LevelManager.StarsAreSelected)
        {
            if(GameManager.Instance.LevelManager.IsStarSelected(this))
            {
                GameManager.Instance.LevelManager.DeselectStarsIncludingIndex(GameManager.Instance.LevelManager.GetStarIndex(this));
            }
            else
            {
                SetStarSelected(true);
            }
        }
    }



}
