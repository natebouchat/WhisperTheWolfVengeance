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
    [Export]
    private float bulletCooldown = 0.25f;
    
    private Vector2 UP = new Vector2(0,-1);
    private Vector2 motion;
    private PackedScene laserBullet;
    private Position2D point;
    private Vector2 rightPoint;
    private Vector2 leftPoint;
    private bool bulletIsReady;
    private bool bufferBullet;
    private bool facingLeft;
    private float bulletTimer;
    private float chargingTimer;

    public override void _Ready(){
        motion = new Vector2();
        laserBullet = ResourceLoader.Load<PackedScene>("res://PreFabs/LaserBullet.tscn");
        point = GetNode<Position2D>("GunPoint");
        rightPoint = new Vector2(point.Position.x, point.Position.y);
        leftPoint = new Vector2(-point.Position.x, point.Position.y);
        bulletIsReady = true;
        bufferBullet = false;
        facingLeft = false;
        bulletTimer = 0;
        chargingTimer = 0;
    }

    public override void _Process(float delta) {
        gravity(delta);
        playerInput(delta);
        motion = MoveAndSlide(motion, UP);
        setBulletIsReady(delta);
    }

 /////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void playerInput(float delta) {
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
        if(Input.IsActionPressed("shoot")) {
            if(chargingTimer < 0.6f) {
                chargingTimer += delta;
            }
        }
        else if(Input.IsActionJustReleased("shoot") || bufferBullet == true) {
            if(chargingTimer <= 0.6f) {
                //normal bullet
                chargingTimer = 0;
                if(bulletIsReady) {
                    shootLaserBullet(false);
                    bulletTimer = 0;
                    bulletIsReady = false;
                    bufferBullet = false;
                }
                else {
                        bufferBullet = true;
                }
            }
            else {
                //charged bullet
                chargingTimer = 0;
                shootLaserBullet(true);
                bulletTimer = 0;
                bulletIsReady = false;
            }
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

    private void shootLaserBullet(bool charged) {
        LaserBullet bullet = (LaserBullet)laserBullet.Instance();
        bullet.GlobalPosition = point.GlobalPosition;
        if(facingLeft) {
            bullet.flipBullet();
        }
        GetParent().AddChild(bullet);
        if(charged) {
            bullet.chargedBullet();
        }
    }

    private void setBulletIsReady(float delta) {
        if(bulletTimer < bulletCooldown) {
            bulletTimer += delta;
        }
        else {
            bulletIsReady = true;
        }
    }

    public System.Object[] getWhisperDetails() {
        System.Object[] obj = new System.Object[] {motion, IsOnFloor(), !bulletIsReady, bufferBullet, chargingTimer};
        return obj;
    }

    public bool getIsFacingLeft() {
        return facingLeft;
    }
}
