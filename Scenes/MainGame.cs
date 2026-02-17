using Godot;
using System;

public partial class MainGame : Node2D
{
	public override void _Ready()
	{
		GameManager.Instance.LoadLevel("Tell");
		((AnimatedSprite2D)GetNode("%Background")).Play();
	}
}
