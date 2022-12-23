using Godot;
using System;

public class WhisperController : KinematicBody2D
{
    [Export]
    private int maxSpeed = 550;
    [Export]
    private int maxFallSpeed = 1000;
    [Export]
    private int jumpForce = 600;
    
    private Vector2 UP = new Vector2(0,-1);
    private Vector2 motion;
    private PackedScene laserBullet;
    private Position2D point;
    private bool facingLeft;
    private Vector2 rightPoint;
    private Vector2 leftPoint;
    

    public override void _Ready(){
        motion = new Vector2();
        laserBullet = ResourceLoader.Load<PackedScene>("res://PreFabs/LaserBullet.tscn");
        point = GetNode<Position2D>("GunPoint");
        rightPoint = new Vector2(point.Position.x, point.Position.y);
        leftPoint = new Vector2(-point.Position.x, point.Position.y);
        facingLeft = false;
    }

    public override void _Process(float delta) {
        gravity(delta);
        playerInput();
        motion = MoveAndSlide(motion, UP);
    }

 /////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void playerInput() {
        if(Input.IsActionPressed("ui_right")) {
            motion.x = maxSpeed;
            point.Position = rightPoint;
            facingLeft = false;
        }
            else if(Input.IsActionPressed("ui_left")) {
                motion.x = -maxSpeed;
                point.Position = leftPoint;
                facingLeft = true;
            }
            else {
                motion.x = 0;
            }
        if(IsOnFloor() && Input.IsActionPressed("ui_up")) {
            motion.y = -jumpForce; 
        }
        if(Input.IsActionJustPressed("ui_accept")) {
            LaserBullet bullet = (LaserBullet)laserBullet.Instance();
            bullet.GlobalPosition = point.GlobalPosition;
            if(facingLeft) {
                bullet.flipBullet();
            }
            GetParent().AddChild(bullet);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void gravity(float delta) {
        motion.y += 20*(delta*60);
        if(motion.y > -50) {
            motion.y += 20*(delta*60);
        }
        if(motion.y > maxFallSpeed) {
            motion.y = maxFallSpeed;
        } 
    }

    public System.Object[] getWhisperDetails() {
        System.Object[] obj = new System.Object[] {motion, IsOnFloor()};
        return obj;
    }

    public bool getIsFacingLeft() {
        return facingLeft;
    }
}
