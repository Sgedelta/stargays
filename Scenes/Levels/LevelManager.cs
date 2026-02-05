using Godot;
using System;

public partial class LevelManager : Node
{



    private Godot.Collections.Array<Star> _selectedStars;

    [Export] private Line2D _starConnectLine;
    [Export] private float StarDeselectSpeed = 1500;

    private bool _isDeselecting = false;

    public bool StarsAreSelected { get { return _selectedStars.Count > 0; } }


    public override void _Ready()
    {
        //Temp, we will need better level loading
        GameManager.Instance.SetActiveLevel(this);
        _selectedStars = new Godot.Collections.Array<Star>();
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

    /// <summary>
    /// Checks if the current _selectedStars is a valid input. If it is, it moves onto the next dialog. If it isn't, it undoes all star input.
    /// </summary>
    public void StarSequenceDone()
    {
        if (IsStarSequenceValid())
        {
            //Nothing for now! lol. TODO
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
        return false; //TEMP, TODO: Fix
    }

    private void DeselectAllStarsAnimated()
    {
        //make sure we only do this once and not multiple times
        if (_isDeselecting) { return; }
        _isDeselecting = true;

        //make a tweener
        Tween deselector = GetTree().CreateTween();

        //remove from mouse pos 
            // Note: we may need to change this if mouse is not being used!
        deselector.TweenMethod(
            Callable.From((Vector2 pos) => { _starConnectLine.SetPointPosition(_starConnectLine.Points.Length - 1, pos); }),
            GetViewport().GetMousePosition(), _selectedStars[^1].Position, 
            (GetViewport().GetMousePosition() - _selectedStars[^1].Position).Length() / StarDeselectSpeed
            );
        deselector.TweenCallback(Callable.From(() => 
        { 
            _starConnectLine.RemovePoint(_starConnectLine.Points.Length - 1);
        }));

        //now denaimate from all stars
        for(int i = _selectedStars.Count; i > 0; i--)
        {

            //don't run on last star, because there's nothing before it (so there'd be a null error)
            if( i != 1)
            {
                deselector.TweenMethod(
                    Callable.From((Vector2 pos) => { _starConnectLine.SetPointPosition(_starConnectLine.Points.Length - 1, pos); }),
                    _selectedStars[i-1].Position, _selectedStars[i-2].Position,
                    (_selectedStars[i-1].Position - _selectedStars[i-2].Position).Length() / StarDeselectSpeed
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
        deselector.TweenCallback(Callable.From(() => { _isDeselecting = false; }));

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
            _selectedStars.Remove(star);
        }
    }

    public void UpdateStarLine()
    {
        if(_isDeselecting)
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

        _starConnectLine.AddPoint(GetViewport().GetMousePosition());
    }
}
