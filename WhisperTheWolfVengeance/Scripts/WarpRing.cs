using Godot;
using System;

public partial class WarpRing : Area2D
{
	private SubViewport ringView;
	private Sprite2D renderedSprite;

	public override void _Ready()
	{
		ringView = GetNode<SubViewport>("SubViewport");
		renderedSprite = GetNode<Sprite2D>("Sprite2D");
		ringView.GetNode<AnimationPlayer>("WarpRingRenderer/AnimationPlayer").Play("temp");
	}

	public override void _Process(double delta)
	{
		renderedSprite.Texture = ringView.GetTexture();
	}
}
