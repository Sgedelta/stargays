using Godot;
using System;

[GlobalClass]
public partial class StarResource : Resource
{
	[Export] public string DisplayName;

	[Export] public Godot.Collections.Array<StarData> Stars;

	[Export] public Godot.Collections.Array<StarCombination> ValidCombinations;

	//parameterless constructor is manditory
	public StarResource() { }
}
