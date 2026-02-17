using Godot;
using System;
using System.Linq;

public partial class LevelManager : Node2D
{



	private Godot.Collections.Array<Star> _selectedStars;

	private Line2D _starConnectLine;
	[Export] private float StarDeselectSpeed = 1500;

	private bool _isDeselecting = false;

	private string constructedKey = "";

	public bool StarsAreSelected { get { return _selectedStars.Count > 0; } }

	private bool _starsAlreadyShown = false;
	private bool _lineFollowsMouse = true;

	public override void _Ready()
	{
		GameManager.Instance.SetActiveLevel(this);
		_selectedStars = new Godot.Collections.Array<Star>();

        Modulate = Color.FromHtml("ffffff00");

        _starConnectLine = GetNode<Line2D>("%StarInputLine");

        Tween starConnectTween = GetTree().CreateTween();

        //make loop and animate right
        starConnectTween.SetLoops();
        starConnectTween.SetTrans(Tween.TransitionType.Sine);

        //do the animation
        starConnectTween.TweenProperty(_starConnectLine, "width", 14, 2).From(8);
        starConnectTween.TweenProperty(_starConnectLine, "width", 8, 2).From(14);


		Modulate = Color.FromHtml("ffffff00");

    }

	public override void _Process(double delta)
	{
		UpdateStarLine();
	}


	public override void _Input(InputEvent @event)
	{
		//we want specifically mouse button up events
		if( @event is InputEventMouseButton mouseEvent && mouseEvent.IsActionReleased("StarSelect") )
		{
			StarSequenceDone();
		}
	}

	public void ShowStars()
	{
		if (_starsAlreadyShown)
		{
			return;
		}
		_starsAlreadyShown = true;
		Tween fadeIn = GetTree().CreateTween();
		fadeIn.TweenProperty(this, "modulate", Color.FromHtml("ffffffff"), GameManager.Instance.FadeTime).From(Color.FromHtml("ffffff00"));
	}

    /// <summary>
    /// Checks if the current _selectedStars is a valid input. If it is, it moves onto the next dialog. If it isn't, it undoes all star input.
    /// </summary>
    public void StarSequenceDone()
    {
        ConstructKey();
        if (IsStarSequenceValid())
        {
            GameManager.Instance.InputTaskCompletionSource.TrySetResult(GameManager.Instance.ValidInputs[constructedKey]);
            _lineFollowsMouse = false;
            //DeselectAllStarsAnimated();
        } 
        else
        {
            DeselectAllStarsAnimated();
        }
    }

	/// <summary>
	/// returns true if _selectedStars is a valid input for this level
	/// </summary>
	/// <returns></returns>
	private bool IsStarSequenceValid()
	{
		GD.Print($"[LM] constructed key is: {constructedKey}");

		return GameManager.Instance.ValidInputs.ContainsKey( constructedKey );
	}

	private void ConstructKey()
	{
		constructedKey = "";
		for(int i = 0; i < _selectedStars.Count; i++)
		{
			constructedKey += _selectedStars[i].ExactKey;
			if( i !=  _selectedStars.Count - 1 )
			{
				constructedKey += " ";
			}
		}

	}

	private void DeselectAllStarsAnimated()
	{
		//make sure we only do this once and not multiple times, and reset key
		if (_selectedStars.Count == 0)
		{
			return;
		}
		if (_isDeselecting) { return; }
		_isDeselecting = true;
		
		//make a tweener
		Tween deselector = GetTree().CreateTween();

		//remove from mouse pos 
			// Note: we may need to change this if mouse is not being used!
		deselector.TweenMethod(
			Callable.From((Vector2 pos) => { 

				_starConnectLine.SetPointPosition(_starConnectLine.Points.Length - 1, pos); }),
				GetViewport().GetMousePosition(), _selectedStars[^1].Position, 
				(GetViewport().GetMousePosition() - _selectedStars[^1].Position).Length() / StarDeselectSpeed
			);
		deselector.TweenCallback(Callable.From(() => 
		{ 
			_starConnectLine.RemovePoint(_starConnectLine.Points.Length - 1);
		}));

		//now denaimate from all stars
		for (int i = _selectedStars.Count; i > 0; i--)
		{

			//don't run on last star, because there's nothing before it (so there'd be a null error)
			if (i != 1)
			{
				deselector.TweenMethod(
					Callable.From((Vector2 pos) => { _starConnectLine.SetPointPosition(_starConnectLine.Points.Length - 1, pos); }),
					_selectedStars[i - 1].Position, _selectedStars[i - 2].Position,
					(_selectedStars[i - 1].Position - _selectedStars[i - 2].Position).Length() / StarDeselectSpeed
				);
			}

			//remove the point and the star
			deselector.TweenCallback(Callable.From(() =>
			{
				_starConnectLine.RemovePoint(_starConnectLine.Points.Length - 1);
				_selectedStars.RemoveAt(_selectedStars.Count - 1);
				
			}));



			

		}

		//allow further animations
		deselector.TweenCallback(Callable.From(() => {
			
			_isDeselecting = false; 
		}));

	}



	public bool IsStarSelected(Star star)
	{
		return _selectedStars.Contains(star);
	}

	public void SelectStar(Star star)
	{
		if (!_isDeselecting && !IsStarSelected(star))
		{
			GD.Print($"[LM] Added star {star.Name}");
			_selectedStars.Add(star);
		}
	}

    public void DeselectStar(Star star)
    {
        if (IsStarSelected(star))
        {
            GD.Print($"[LM] Removed star {star.Name}");
            _selectedStars.Remove(star);
        }
    }

    public void DeselectStarsIncludingIndex(int index)
    {
        for(int i = _selectedStars.Count-1; i >= index; i--)
        {
            _selectedStars.RemoveAt(i);
        }
    }

    public int GetStarIndex(Star star)
    {
        return _selectedStars.IndexOf(star);
    }

	public void UpdateStarLine()
	{

        if (_isDeselecting)
		{
			//can't update this, because we're already updating line position in the deanimation code!
			return;
		}

		//clear out old points
		_starConnectLine.ClearPoints();


		//add points at star locations
		foreach (Star star in _selectedStars)
		{
			_starConnectLine.AddPoint(star.Position);
		}

		if(_lineFollowsMouse)
		{
            _starConnectLine.AddPoint(GetViewport().GetMousePosition());
        }

		
		
	}


}
