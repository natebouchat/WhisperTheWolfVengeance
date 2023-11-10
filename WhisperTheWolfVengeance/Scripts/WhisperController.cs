using Godot;
using System;

public partial class WhisperController : CharacterBody2D
{
	[Export]
	private int maxSpeed = 700;
	[Export]
	private int maxFallSpeed = 1500;
	[Export]
	private int jumpForce = 800;
	[Export]
	private float bulletCooldown = 0.25f;
	
	private Vector2 UP = new Vector2(0,-1);
	private Vector2 motion;
	private Vector2 externalMotion;
	private PackedScene laserBullet;
	private PackedScene droppedRing;
	private PlayerUI playerUI;
	private Marker2D point;
	private PauseMenu pauseMenu;
	private Vector2 rightPoint;
	private Vector2 leftPoint;
	private bool bulletIsReady;
	private bool bufferBullet;
	private bool facingLeft;
	private bool motionAdded;
	private double bulletTimer;
	private double chargingTimer;
	
	public double hurtCooldown {get; set;}
	public int whisp {get; set;}
	public bool controlsEnabled{get; set;}

	public override void _Ready(){
		motion = new Vector2();
		externalMotion = new Vector2();
		laserBullet = ResourceLoader.Load<PackedScene>("res://PreFabs/LaserBullet.tscn");
		droppedRing = ResourceLoader.Load<PackedScene>("res://PreFabs/Ring.tscn");
		playerUI = GetNode<PlayerUI>("PlayerUI");
		point = GetNode<Marker2D>("GunPoint");
		pauseMenu = GetNode<PauseMenu>("Pause Menu");
		rightPoint = new Vector2(point.Position.X, point.Position.Y);
		leftPoint = new Vector2(-point.Position.X, point.Position.Y);
		bulletIsReady = true;
		bufferBullet = false;
		facingLeft = false;
		motionAdded = false;
		bulletTimer = 0;
		chargingTimer = 0;
		hurtCooldown = 0;
		whisp = 0;
		controlsEnabled = true;
	}

	public override void _Process(double delta) {
		Gravity(delta);
		if(controlsEnabled) {
			PlayerInput(delta);
		}
		CalculateExternalMotion(delta);
		
		this.Velocity = motion + externalMotion;
		MoveAndSlide();
		SetBulletIsReady(delta);
	}

 /////////////////////////////////////////////////////////////////////////////////////////////////////////

	private void PlayerInput(double delta) {
		if(hurtCooldown <= 0 && !pauseMenu.Visible) {
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
				if((Input.IsActionJustReleased("ui_right") && externalMotion.X < 0) || (Input.IsActionJustReleased("ui_left") && externalMotion.X > 0)) {
					externalMotion.X = Velocity.X;
				}
			}

			if(IsOnFloor() && Input.IsActionPressed("jump")) {
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
						ShootLaserBullet(false);
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
					ShootLaserBullet(true);
					bulletTimer = 0;
					bulletIsReady = false;
				}
			}
		}
		else {
			hurtCooldown -= delta;
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

	private void Gravity(double delta) {
		if(!IsOnFloor()) {
			motion.Y += (float)(delta*1200);
			if(motion.Y > -800) {
				motion.Y += (float)(delta*1320);
			}
			if(motion.Y > maxFallSpeed) {
				motion.Y = maxFallSpeed;
			}
		}
		else if(hurtCooldown <= 0) {
			motion.Y = 0;
		}
	}

	private void CalculateExternalMotion(double delta) {
			if(externalMotion.X > 0) {
				externalMotion.X -= (float)Math.Sqrt(delta * 20 * externalMotion.X);
				if(externalMotion.X < 100) {
					externalMotion.X = 0;
				}
			}
			else if(externalMotion.X < 0) {
				externalMotion.X += (float)Math.Sqrt(delta * 20 * -externalMotion.X);
				if(externalMotion.X > -100) {
					externalMotion.X = 0;
				}
			}

			if((!IsOnFloor() && externalMotion.Y != 0) || motionAdded) {
				motionAdded = false;
				externalMotion.Y += (float)(delta*1200);
				if(externalMotion.Y > -400) {
					externalMotion.Y += (float)(delta*1320);
				}
				if(externalMotion.Y > maxFallSpeed) {
					externalMotion.Y = maxFallSpeed;
				}
			}
			else {
				externalMotion.Y = 0;
			}
		
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////

	private void ShootLaserBullet(bool charged) {
		LaserBullet bullet = (LaserBullet)laserBullet.Instantiate();
		bullet.GlobalPosition = point.GlobalPosition;
		if(facingLeft) {
			bullet.FlipBullet();
		}
		GetParent().AddChild(bullet);
		if(charged) {
			bullet.ChargedBullet();
		}
		
	}

	private void SetBulletIsReady(double delta) {
		if(bulletTimer < bulletCooldown) {
			bulletTimer += delta;
		}
		else {
			bulletIsReady = true;
		}
	}

	public void AddMotion(int xVal, int yVal) {
		externalMotion.X = xVal;
		externalMotion.Y = yVal;
		motion.Y = 0;
		motionAdded = true;
	}

	public void WhipserIsHurt() {
		if(facingLeft) {
			externalMotion.X = 800;
		}
		else {
			externalMotion.X = -800;
		}
		externalMotion.Y = -1200;
		motion.X = 0;
		motion.Y = 0;
		motionAdded = true;
		hurtCooldown = 0.6;
		DropAllRings();
	}

	private void DropAllRings() {
		for(int i = 0; i < playerUI.rings; i++) {
			Ring ring = (Ring)droppedRing.Instantiate();
			GetParent().CallDeferred("add_child", ring);
			ring.CallDeferred("DropRing", this.GlobalPosition);
		}
		playerUI.rings = 0;
	}

	public object[] GetWhisperDetails() {
		object[] obj = new object[] {Velocity, IsOnFloor(), !bulletIsReady, bufferBullet, chargingTimer};
		return obj;
	}

	public bool GetIsFacingLeft() {
		return facingLeft;
	}

	public void disableControls() {
		controlsEnabled = false;
		motion.X = 0;
	}
}
