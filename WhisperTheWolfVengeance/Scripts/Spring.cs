using Godot;
using System;

public partial class Spring : StaticBody2D
{
	[Export]
	private int springForce = 2000;

	private Area2D trigger;
	private AudioStreamPlayer bounce;
	
	public override void _Ready()
	{
		trigger = GetNode<Area2D>("Area2D");
		bounce =  GetNode<AudioStreamPlayer>("SpringSFX");
	}
/*
	public override void _Process(double delta)
	{
	}
*/
	private void OnSpringHit(Node Collider) {
		bounce.Play();
		int xVal = (int)(Math.Sin(this.Rotation) * springForce);
		int yVal = (int)(Math.Cos(this.Rotation) * springForce);
		Collider.Call("AddMotion", xVal, -yVal);
	}
}
