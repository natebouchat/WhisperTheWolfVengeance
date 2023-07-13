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
        if(CheckToggleDebug()) {
            SetDebugDetails(whisperController.GetWhisperDetails());
        }
    }


    private bool CheckToggleDebug() {
        if(Input.IsActionJustPressed("ui_focus_next")) {
            this.Visible = !this.Visible;
        }
        return this.Visible;
    }

    private void SetDebugDetails(System.Object[] details) {
        debug.GetChild<Label>(0).Text = "Velocity: " + ((Vector2)(details[0])).Floor();
        debug.GetChild<Label>(1).Text = "On Floor: " + details[1];
        debug.GetChild<Label>(2).Text = "Cooldown: " + details[2];
        debug.GetChild<Label>(3).Text = "Bullet Buffer: " + details[3];
        debug.GetChild<Label>(4).Text = "Charge Time: " + Math.Round((double)details[4], 2);

        debug.GetChild<Label>(5).Text = "State: " + whisperStateManager.state;
    }
}
