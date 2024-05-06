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
	private AudioStreamPlayer jumpSFX;
	private Vector2 shotPoint;
	private bool motionAdded;
	private double bulletTimer;
	
	public double HurtCooldown {get; set;}
	public double ChargingTimer {get; set;}
	public int Whisp {get; set;}
	public bool BulletIsReady {get; set;}
	public bool BufferBullet {get; set;}
	public bool FacingLeft {get; set;}
	public bool IsDucking {get; set;}
	public bool ControlsEnabled{get; set;}

	public override void _Ready(){
		motion = new Vector2();
		externalMotion = new Vector2();
		laserBullet = ResourceLoader.Load<PackedScene>("res://PreFabs/LaserBullet.tscn");
		droppedRing = ResourceLoader.Load<PackedScene>("res://PreFabs/Ring.tscn");
		playerUI = GetNode<PlayerUI>("PlayerUI");
		point = GetNode<Marker2D>("GunPoint");
		pauseMenu = GetNode<PauseMenu>("Pause Menu");
		jumpSFX = GetNode<AudioStreamPlayer>("SFXJump");
		shotPoint = new Vector2(88, 33);
		motionAdded = false;
		bulletTimer = 0;

		HurtCooldown = 0;
		ChargingTimer = 0;
		Whisp = 0;
		BulletIsReady = true;
		BufferBullet = false;
		FacingLeft = false;
		IsDucking = false;
		ControlsEnabled = true;
	}

	public override void _Process(double delta) {
		Gravity(delta);
		if(ControlsEnabled) {
			PlayerInput(delta);
		}
		CalculateExternalMotion(delta);
		
		this.Velocity = motion + externalMotion;
		MoveAndSlide();
		SetBulletIsReady(delta);
	}

 /////////////////////////////////////////////////////////////////////////////////////////////////////////

	private void PlayerInput(double delta) {
		if(HurtCooldown <= 0 && !pauseMenu.Visible) {
			if(Input.IsActionPressed("ui_down") && this.IsOnFloor()) {
				IsDucking = true;
				motion.X = 0;
				shotPoint.Y = 98;
				if(FacingLeft) {
					shotPoint.X = -130;
				}
				else {
					shotPoint.X = 130;
				}
				point.Position = shotPoint;
			}
			else {
				if(Input.IsActionPressed("ui_right")) {
					motion.X = maxSpeed;
					shotPoint.X = 88;
					shotPoint.Y = 33;
					point.Position = shotPoint;
					FacingLeft = false;
					IsDucking = false;
				}
				else if(Input.IsActionPressed("ui_left")) {
					motion.X = -maxSpeed;
					shotPoint.X = -88;
					shotPoint.Y = 33;
					point.Position = shotPoint;
					FacingLeft = true;
					IsDucking = false;
				}
				else {
					motion.X = 0;
					if((Input.IsActionJustReleased("ui_right") && externalMotion.X < 0) || (Input.IsActionJustReleased("ui_left") && externalMotion.X > 0)) {
						externalMotion.X = Velocity.X;
					}
					IsDucking = false;
				}
			}

			if(IsOnFloor() && Input.IsActionJustPressed("jump")) {
				motion.Y = -jumpForce; 
				jumpSFX.Play();
			}
			if(Input.IsActionPressed("shoot")) {
				if(ChargingTimer < 0.6) {
					ChargingTimer += delta;
				}
			}
			else if(Input.IsActionJustReleased("shoot") || BufferBullet == true) {
				if(ChargingTimer <= 0.6) {
					//normal bullet
					ChargingTimer = 0;
					if(BulletIsReady) {
						ShootLaserBullet(false);
						bulletTimer = 0;
						BulletIsReady = false;
						BufferBullet = false;
					}
					else {
							BufferBullet = true;
					}
				}
				else {
					//charged bullet
					ChargingTimer = 0;
					ShootLaserBullet(true);
					bulletTimer = 0;
					BulletIsReady = false;
				}
			}
		}
		else {
			HurtCooldown -= delta;
		}

		if(Input.IsActionJustPressed("rightWhisp")) {
			Whisp += 1;
			if(Whisp == 5) {
				Whisp = 0;
			}
		}
		else if(Input.IsActionJustPressed("leftWhisp")) {
			Whisp -= 1;
			if(Whisp == -1) {
				Whisp = 4;
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
		else if(HurtCooldown <= 0) {
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
		if(FacingLeft) {
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
			BulletIsReady = true;
		}
	}

	public void AddMotion(int xVal, int yVal) {
		externalMotion.X = xVal;
		externalMotion.Y = yVal;
		motion.Y = 0;
		motionAdded = true;
	}

	public void WhipserIsHurt() {
		if(FacingLeft) {
			externalMotion.X = 800;
		}
		else {
			externalMotion.X = -800;
		}
		externalMotion.Y = -1200;
		motion.X = 0;
		motion.Y = 0;
		motionAdded = true;
		HurtCooldown = 0.6;
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
		object[] obj = new object[] {Velocity, IsOnFloor(), !BulletIsReady, BufferBullet, ChargingTimer};
		return obj;
	}

	public void DisableControls() {
		ControlsEnabled = false;
		motion.X = 0;
	}
}
