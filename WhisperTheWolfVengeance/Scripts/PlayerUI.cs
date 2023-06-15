using Godot;
using System;

public partial class PlayerUI : CanvasLayer
{
    int rings;

    public override void _Ready()
    { 
        rings = 0;
    }

    public override void _Process(double delta)
    {
      this.GetChild<Label>(0).Text = "Rings: " + rings;
    }

    public void AddRings(int value) {
        rings += value;
    }

    public void DropRings() {
        rings = 0;
    }
}
