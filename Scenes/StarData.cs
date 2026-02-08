using Godot;
using System;


public partial class StarData : Resource
{
	[Export] public string Id;
	[Export] public string Word;
	[Export] public Vector2 Position;

	public StarData()
	{

	}
}
