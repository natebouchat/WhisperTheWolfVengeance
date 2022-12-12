using Godot;
using System;

public class DebugDetails : CanvasLayer
{

    private VBoxContainer debug;

    public override void _Ready()
    {
        debug = GetNode<VBoxContainer>("VBoxContainer");
    }

    public void setDebugDetails(System.Object[] details) {
        for(int i = 0; i < details.Length; i++) {
            debug.GetChild<Label>(i).Text = debug.GetChild<Label>(i).Name + ": " + details[i];
        }
    }
}
