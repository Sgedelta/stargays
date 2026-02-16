using Godot;
using System;
using static QuestionManager;

[GlobalClass]
public partial class QuestionSettings : Resource
{
    [Export] public string QuestionText;

    [Export] public float ChanceToPick = 0f;
    [Export] public int QuestionOrder = 0;

    [Export] public Godot.Collections.Array<string> Options;
    [Export] public Godot.Collections.Array<ButtonStates> OptionStates;

}
