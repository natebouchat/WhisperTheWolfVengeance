using Godot;
using System;

public partial class PlayerUI : CanvasLayer
{
    public int rings {get; set;}

    public override void _Ready()
    { 
        rings = 5;
    }

    public override void _Process(double delta)
    {
      this.GetChild<Label>(0).Text = "Rings: " + rings;
    }

    public void AddRings(int value) {
        rings += value;
    }

}
