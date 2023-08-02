using Godot;
using System;

public partial class Spring : StaticBody2D
{
	[Export]
	private int springForce = 2000;

	private Area2D trigger;
	private AudioStreamPlayer bounceSFX;
	private AnimatedSprite2D spring;
	
	public override void _Ready()
	{
		trigger = GetNode<Area2D>("Area2D");
		bounceSFX = GetNode<AudioStreamPlayer>("SFXSpring");
		bounceSFX.VolumeDb = _SettingsManager.sfxVolume;
		spring = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		spring.Animation = "default";
	}

	private void OnSpringHit(Node Collider) {
		if(_SettingsManager.sfxVolume > -25) {
			bounceSFX.Play();
		}
		spring.Play("Bounce");
		int xVal = (int)(Math.Sin(this.Rotation) * springForce);
		int yVal = (int)(Math.Cos(this.Rotation) * springForce);
		Collider.Call("AddMotion", xVal, -yVal);
	}
}
