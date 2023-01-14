using Godot;
using System;

public class LaserBullet : KinematicBody2D
{
    private Vector2 motion;
    private Vector2 UP = new Vector2(0,-1);
    private AnimatedSprite bullet;

    [Export]
    private int speed = 900;

    public override void _Ready()
    {
        motion = new Vector2();
        bullet = GetNode<AnimatedSprite>("AnimatedSprite");
        bullet.Animation = "default";
        bullet.Playing = true;
    }

    public override void _Process(float delta)
    {
        motion.x = speed;
        motion = MoveAndSlide(motion, UP);
        if(IsOnWall()) {
            GetNode<CollisionShape2D>("CollisionShape2D").Scale = new Vector2(0.5f, 1);
            bulletHit();
        }
        else if(IsOnCeiling()) {
            this.QueueFree();
        }
    }

    public void chargedBullet() {
        GetNode<CollisionShape2D>("CollisionShape2D").Scale = new Vector2(2, 2);
        bullet.Scale = new Vector2(3, 2.5f);
    }

    public void flipBullet () {
        GetNode<AnimatedSprite>("AnimatedSprite").FlipH = true;
        speed = -speed;
    }

    private async void bulletHit() {
        bullet.Animation = "Hit";
        await ToSignal(bullet, "animation_finished");
        this.QueueFree();
    }

    private void onBulletScreenExit() {
        this.QueueFree();
    }
}
