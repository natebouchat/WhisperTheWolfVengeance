using Godot;
using System;

public partial class TangleStateManager : CharacterBody2D
{
	[Export]
	private bool facingLeft = false;
	
	AnimatedSprite2D tangleSprites;
	AnimationPlayer tangleAnimations;
	
	public override void _Ready() {
		tangleSprites = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		tangleSprites.FlipH = facingLeft;
		tangleSprites.Play();
		tangleAnimations = GetNode<AnimationPlayer>("AnimationPlayer");
		tangleAnimations.Play("Idle");
	}
}
