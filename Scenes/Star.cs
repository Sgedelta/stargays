using Godot;
using System;

public partial class Star : Area2D
{

    public override void _Ready()
    {
         
    }

    public override void _Process(double delta)
    {

    }

    //a helper method for selecting the star, so multiple input paths can route to here. In case we want keyboard support and to stay DRY
    public void SelectThisStar()
    {
        GameManager.Instance.LevelManager.SelectStar(this);
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
            SelectThisStar();
        }
    }

    private void HandleMouseEnter()
    {
        if (GameManager.Instance.LevelManager.StarsAreSelected)
        {
            SelectThisStar();
        }
    }



}
