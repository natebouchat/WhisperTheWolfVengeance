using Godot;
using System;

public class PlayerUI : CanvasLayer
{
    int rings;

    public override void _Ready()
    { 
        rings = 0;
    }

    public override void _Process(float delta)
    {
      this.GetChild<Label>(0).Text = "Rings: " + rings;
    }

    public void addRings(int value) {
        rings += value;
    }

    public void dropRings() {
        rings = 0;
    }
}