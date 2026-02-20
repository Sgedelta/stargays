using Godot;
using System;

public partial class MainGame : Node2D
{
	[Export] private Camera2D _camera;
	[Export] private float _zoomTime = 5;

	public override void _Ready()
	{
		GameManager.Instance.LoadLevel(GameManager.Instance.FirstLevelName);
		((AnimatedSprite2D)GetNode("%Background")).Play();

	}


	public void AnimateToLargerPos()
	{
		Tween zoomOut = GetTree().CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);

		zoomOut.TweenProperty(_camera, "position", new Vector2(1920, 1500), _zoomTime);
		zoomOut.Parallel().TweenProperty(_camera, "zoom", new Vector2(.4f, .4f), _zoomTime);

	}
}
