using Godot;
using System;

public partial class LaserBullet : Area2D
{
	private Vector2 motion;
	private AnimatedSprite2D bullet;

	[Export]
	private int speed = 15;

	public override void _Ready()
	{
		motion = new Vector2(speed, 0);
		bullet = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		bullet.Play("default");
	}

	public override void _Process(double delta)
	{
		motion.X = speed;
		this.Position += motion;
	}

	public void ChargedBullet() {
		GetNode<CollisionShape2D>("CollisionShape2D").Scale = new Vector2(2, 2);
		bullet.Scale = new Vector2(3, 2.5f);
		this.Name += "Charged";
	}

	public void FlipBullet () {
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = true;
		speed = -speed;
	}

	private async void BulletHit(Node Collider) {
		bullet.Animation = "Hit";
		speed = 0;
		await ToSignal(bullet, "animation_finished");
		this.QueueFree();
	}

	private void OnBulletScreenExit() {
		this.QueueFree();
	}
}
