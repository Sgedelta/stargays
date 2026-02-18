using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	[Export] CanvasModulate _mod;
	Tween _showHideTweener;
	bool _activeShowHide;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_mod.Color = new Color(1, 1, 1, 0);
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PauseGame()
	{
        GD.Print("PAUSE");
        GetTree().Paused = true;

		if (_activeShowHide)
		{
			_showHideTweener.Kill();
		}

        _activeShowHide = true;
        _showHideTweener = GetTree().CreateTween().SetPauseMode(Tween.TweenPauseMode.Process);

		_showHideTweener.TweenProperty(_mod, "color", Color.FromHtml("#ffffffff"), GameManager.Instance.FadeTime * 1.5f);
		_showHideTweener.TweenCallback(Callable.From(() => { _activeShowHide = false; }));

	}

	public void UnpauseGame()
	{
		GD.Print("UNPAUSE");
		

        if (_activeShowHide)
        {
            _showHideTweener.Kill();
        }

		_activeShowHide = true;
        _showHideTweener = GetTree().CreateTween().SetPauseMode(Tween.TweenPauseMode.Process);

        _showHideTweener.TweenProperty(_mod, "color", Color.FromHtml("#ffffff00"), GameManager.Instance.FadeTime * 1.5f);
        _showHideTweener.TweenCallback(Callable.From(() => { _activeShowHide = false; GetTree().Paused = false; }));

    }

}
