using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	private ColorRect pauseBG;
	
	public override void _Ready()
	{
		this.Visible = false;
		pauseBG = GetNode<ColorRect>("ColorRect");
		GetNode<AnimationPlayer>("ColorRect/AnimationPlayer").Play("Idle");
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("pause")) {
			GetTree().Paused = !GetTree().Paused;
			if(GetTree().Paused) {
				try {
					pauseBG.Modulate = GetNode<AnimatedSprite2D>("../MainSprite/ColorSprite").Modulate;
				}
				catch(Exception) {
					pauseBG.Modulate = new Color(0.42f, 0.76f, 0.74f);
				}
			}
			this.Visible = GetTree().Paused;
		}
	}
}
