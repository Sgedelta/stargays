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
		Tween zoomOut = GetTree().CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);

		zoomOut.TweenProperty(_camera, "position", new Vector2(1920, 3240), _zoomTime);
		zoomOut.Parallel().TweenProperty(_camera, "zoom", new Vector2(.34f, .34f), _zoomTime).SetTrans(Tween.TransitionType.Cubic);

	}
}
