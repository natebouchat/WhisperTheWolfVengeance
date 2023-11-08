using Godot;
using System;

public partial class TailRotation : AnimatedSprite2D
{
	private WhisperController whisperController;
	private Vector2 tailOffset;
	private float ratio;

	public override void _Ready()
	{
		whisperController = GetParent().GetParent<WhisperController>();
		tailOffset = new Vector2();
		this.Visible = false;
	}

	public override void _Process(double delta)
	{
		if(this.Visible) {
			if(whisperController.Velocity.Y == 0) {
				ratio = whisperController.Velocity.X;
			}
			else {
				ratio = whisperController.Velocity.X/whisperController.Velocity.Y;
			}
			
			this.Rotation = (float)(Math.PI - (Math.Atan(ratio) + (Math.PI * 0.5)));
			if(whisperController.Velocity.Y < 0) {
				this.Rotation = (float)Math.PI + this.Rotation;
			}
			GD.Print(this.Rotation);

			tailOffset.X = (float)(Math.Cos(this.Rotation) * -75);
			tailOffset.Y = (float)((Math.Sin(this.Rotation) * -75) + 30);
			this.Position = tailOffset;
		}
	}
}
