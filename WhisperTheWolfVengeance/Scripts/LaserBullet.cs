using Godot;
using System;

public class LaserBullet : KinematicBody2D
{
    private Vector2 motion;
    private Vector2 UP = new Vector2(0,-1);

    [Export]
    private int speed = 900;

    public override void _Ready()
    {
        motion = new Vector2();
    }

    public override void _Process(float delta)
    {
        motion.x = speed;
        motion = MoveAndSlide(motion, UP);
        if(IsOnWall()) {
            bulletHit();
        }
    }

    public void flipBullet () {
        GetNode<AnimatedSprite>("AnimatedSprite").FlipH = true;
        speed = -speed;
    }

    private void bulletHit() {
        GD.Print("Hit");
        onBulletScreenExit();
    }

    private void onBulletScreenExit() {
        this.QueueFree();
    }
}
