using Godot;
using System;

public partial class WhisperController : CharacterBody2D
{
	[Export]
	private int maxSpeed = 550;
	[Export]
	private int maxFallSpeed = 1500;
	[Export]
	private int jumpForce = 900;
	[Export]
	private float bulletCooldown = 0.25f;
	
	private Vector2 UP = new Vector2(0,-1);
	private Vector2 motion;
	private PackedScene laserBullet;
	private Marker2D point;
	private Vector2 rightPoint;
	private Vector2 leftPoint;
	private bool bulletIsReady;
	private bool bufferBullet;
	private bool facingLeft;
	private double bulletTimer;
	private double chargingTimer;
	
	public int whisp {get; set;}

	public override void _Ready(){
		motion = new Vector2();
		laserBullet = ResourceLoader.Load<PackedScene>("res://PreFabs/LaserBullet.tscn");
		point = GetNode<Marker2D>("GunPoint");
		rightPoint = new Vector2(point.Position.X, point.Position.Y);
		leftPoint = new Vector2(-point.Position.X, point.Position.Y);
		bulletIsReady = true;
		bufferBullet = false;
		facingLeft = false;
		bulletTimer = 0;
		chargingTimer = 0;
		whisp = 0;
	}

	public override void _Process(double delta) {
		gravity(delta);
		playerInput(delta);
		this.Velocity = motion;
		MoveAndSlide();
		setBulletIsReady(delta);
	}

 /////////////////////////////////////////////////////////////////////////////////////////////////////////

	private void playerInput(double delta) {
		if(Input.IsActionPressed("ui_right")) {
			motion.X = maxSpeed;
			point.Position = rightPoint;
			facingLeft = false;
		}
			else if(Input.IsActionPressed("ui_left")) {
				motion.X = -maxSpeed;
				point.Position = leftPoint;
				facingLeft = true;
			}
			else {
				motion.X = 0;
			}
		if(IsOnFloor() && Input.IsActionPressed("ui_up")) {
			motion.Y = -jumpForce; 
		}
		if(Input.IsActionPressed("shoot")) {
			if(chargingTimer < 0.6) {
				chargingTimer += delta;
			}
		}
		else if(Input.IsActionJustReleased("shoot") || bufferBullet == true) {
			if(chargingTimer <= 0.6) {
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
		if(Input.IsActionJustPressed("rightWhisp")) {
			whisp += 1;
			if(whisp == 5) {
				whisp = 0;
			}
		}
		else if(Input.IsActionJustPressed("leftWhisp")) {
			whisp -= 1;
			if(whisp == -1) {
				whisp = 4;
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////

	private void gravity(double delta) {
		if(!IsOnFloor()) {
			motion.Y += (float)(20*(delta*60));
			if(motion.Y > -50) {
				motion.Y += (float)(22*(delta*60));
			}
			if(motion.Y > maxFallSpeed) {
				motion.Y = maxFallSpeed;
			}
		}
		else {
			motion.Y = 0;
		} 
		
	}

	private void shootLaserBullet(bool charged) {
		
		LaserBullet bullet = (LaserBullet)laserBullet.Instantiate();
		bullet.GlobalPosition = point.GlobalPosition;
		if(facingLeft) {
			bullet.flipBullet();
		}
		GetParent().AddChild(bullet);
		if(charged) {
			bullet.chargedBullet();
		}
		
	}

	private void setBulletIsReady(double delta) {
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
