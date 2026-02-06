using Godot;
using System;
using System.Linq;

public partial class TestingDialogEnd : Node
{


    public void OnButtonPress()
    {
        GameManager.Instance.InputTaskCompletionSource.TrySetResult(GameManager.Instance.ValidInputs.Values.First());
    }
}
