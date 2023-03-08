using Godot;
using System;

public partial class DebugDetails : CanvasLayer
{
    private VBoxContainer debug;
    private WhisperController whisperController;
    private WhisperStateManager whisperStateManager;

    public override void _Ready()
    {
        debug = GetNode<VBoxContainer>("VBoxContainer");
        whisperController = GetParent().GetParent<WhisperController>();
        whisperStateManager = GetNode<WhisperStateManager>("../../WhisperAnimations");
    }

    public override void _Process(double delta) {
        checkToggleDebug();
        setDebugDetails(whisperController.getWhisperDetails());
    }


    private void checkToggleDebug() {
        if(Input.IsActionJustPressed("ui_focus_next")) {
            this.Visible = !this.Visible;
        }
    }

    private void setDebugDetails(System.Object[] details) {
        for(int i = 0; i < details.Length; i++) {
            debug.GetChild<Label>(i).Text = debug.GetChild<Label>(i).Name + ": " + details[i];
        }
        debug.GetChild<Label>(debug.GetChildCount()-1).Text = "State: " + whisperStateManager.state;
    }
}
