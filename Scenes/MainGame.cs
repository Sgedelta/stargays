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
        ((AnimatedSprite2D)GetNode("%SadBackground")).Play();

		AnimateToLargerPos();

    }


	public void AnimateToLargerPos()
	{
		Tween zoomOut = GetTree().CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);

		zoomOut.TweenProperty(GetNode("%Background"), "modulate", Color.FromHtml("ffffff00"), GameManager.Instance.FadeTime / 10f);
		zoomOut.TweenProperty(_camera, "position", new Vector2(1920, 3240), _zoomTime);
        zoomOut.Parallel().TweenMethod(Callable.From<Vector2>((scale) => { GameManager.Instance.SetPauseOffset(scale); }), Vector2.Zero, new Vector2(-3840, 0), _zoomTime);
        zoomOut.Parallel().TweenProperty(_camera, "zoom", new Vector2(.34f, .34f), _zoomTime).SetTrans(Tween.TransitionType.Cubic);
		zoomOut.Parallel().TweenMethod(Callable.From<Vector2>((scale) => { GameManager.Instance.SetPauseScale(scale); }), _camera.Zoom, new Vector2(1/.33f, 1/.33f), _zoomTime).SetTrans(Tween.TransitionType.Cubic);

	}
}
