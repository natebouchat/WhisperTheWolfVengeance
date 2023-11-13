using Godot;
using System;

public partial class WhisperStateManager : AnimationPlayer
{
	private WhisperController whisperController;
	private AnimatedSprite2D mainSprite;
	private AnimatedSprite2D colorSprite;
	private AnimatedSprite2D chargeSprite;
	private AnimatedSprite2D tailSprite;
	private AnimationPlayer chargeLightAnimations;
	private AudioStreamPlayer dropRingsSFX;
	private Timer timer;
	private object[] details;
	private Vector2 chargeOffset;
	private bool facingLeft;
	private bool facingChanged;
	private bool shotFiredOrCharged;
	private bool extendSpinJump;
	private int currentWhisp;
	private Color cyan; 
	private Color green;
	private Color pink;
	private Color blue;
	private Color orange; 

	public string state {get; set;}

	public override void _Ready()
	{
		whisperController = GetParent<WhisperController>();
		mainSprite = GetNode<AnimatedSprite2D>("../MainSprite");
		mainSprite.Play("default");
		colorSprite = GetNode<AnimatedSprite2D>("../MainSprite/ColorSprite");
		colorSprite.Play("default");
		chargeSprite = GetNode<AnimatedSprite2D>("../MainSprite/ChargingSprite");
		chargeSprite.Play("default");
		tailSprite = GetNode<AnimatedSprite2D>("../MainSprite/TailSprite");
		tailSprite.Play("default");
		chargeLightAnimations = GetNode<AnimationPlayer>("ChargeLightAnimations");
		timer = GetNode<Timer>("../Timer");

		dropRingsSFX = GetNode<AudioStreamPlayer>("../SFXDropRings");
        dropRingsSFX.VolumeDb = _SettingsManager.sfxVolume;
		

		cyan = new Color(0.42f, 0.76f, 0.74f);      //#6dc3be
		green = new Color(0.4f, 0.8f, 0.38f);       //#65cd62
		pink = new Color(0.91f, 0.41f, 0.94f);      //#e869ef
		blue = new Color(0.13f, 0.19f, 0.9f);       //#2031e5
		orange = new Color(0.84f, 0.44f, 0.11f);    //#d66f1c
		currentWhisp = whisperController.whisp;
		SetColorModulation(whisperController.whisp);

		chargeOffset = new Vector2(0, 0);
		facingLeft = false;
		facingChanged = false;
		shotFiredOrCharged = false;
		extendSpinJump = false;
	}

	public override void _Process(double delta) {
		details = whisperController.GetWhisperDetails();
		SetSpriteDirection(whisperController.GetIsFacingLeft());
		state = mainSprite.Animation;
		shotFiredOrCharged = ((Boolean)details[2]) == true || ((double)details[4] >= 0.1);
		SetExtendSpinJump();

		if(whisperController.hurtCooldown <= 0) {
			SetAnimationState();
			SetChargeLight();
		}
		else if(!mainSprite.Animation.Equals("Hurt")) {
			if(_SettingsManager.sfxVolume > -25) {
				dropRingsSFX.Play();
			}
			this.Play("WhisperHurt");
		}
		CheckWhispsSwitched();  
	}

///////////////////////////////////////////////////////////////////////////////////////////////////

	private void SetAnimationState() {
		// If not on ground //
		if(((Boolean)details[1]) == false) {
			// If upwards motion //
			if(((Vector2)details[0]).Y < 0) {
				// If not currently jumping/falling, AND shot has not been shot/fired //
				if(!state.Contains("Jump") && !state.Equals("Fall") && !shotFiredOrCharged) {
					this.Play("WhisperSpinJump");
				}
				else {
					// If shot fired/charged switch to jump animation
					if((!(state.Equals("Jump") || state.Equals("JumpEnd"))) && shotFiredOrCharged) {
						this.Play("WhisperJump");
					}
				}
			}
			else { // If downwards motion  AND  if bullet shot or charging //
				if(shotFiredOrCharged) {
					if(!state.Equals("Fall")) {
						this.Play("WhisperFall");
					}
				}
			}
			
		}
		// If no longer falling, Transition from falling //
		else if(state.Equals("Fall")) {
			TransitionAnimation();
		}
		else if(!extendSpinJump) { // If on ground AND not on spring //
			// If moving on x axis // 
			if(((Vector2)details[0]).X != 0) {
				if(!state.Equals("Run") || facingChanged) {
					// If charging/charged, skip transition frame //
					if((double)details[4] >= 0.1) {
						mainSprite.Animation = "Run";
						colorSprite.Animation = "Run";
					}
					else {
						this.Play("WhisperRun");
						facingChanged = false;
					}
				}
			}
			else { // If not moving on x axis //
				// If bullet shot  OR  Bullet is buffered, AND not currently charging //
				if((((Boolean)details[2]) == true || ((Boolean)details[3]) == true) && ((double)details[4] < 0.1)) {
					if(!state.Equals("IdleShoot")) {
						this.Play("WhisperIdleShoot");
					}
				}
				// Bullet Charging or Charged //
				else if((double)details[4] >= 0.1) {
					if(!(state.Equals("IdleCharging") || state.Equals("IdleCharged"))) {
						if((double)details[4] < 0.6) {
							this.Play("WhisperIdleCharge");
						}
						else { // Maintain Charged Sprite2D //
							this.Play("WhisperIdlePreCharged");
						}
					}
				}
				else{ // Idle //
					if(!state.Equals("default")) {
						this.Play("WhisperIdle");
					}
				}
			}
		}

		if(!state.Equals("SpinJump")) {
			colorSprite.Visible = true;
			tailSprite.Visible = false;
		}
	}



	private void SetChargeLight() {
		// If Bullet is Charging/Charged //
		if((double)details[4] >= 0.1) {
			//if not fully charged
			if((double)details[4] < 0.6) {
				if(!chargeSprite.Animation.Equals("Charging")) {
					chargeLightAnimations.Play("Charging");
				}
			}
			// If running //
			else if(state.Equals("Run") && !chargeSprite.Animation.Equals("ChargedRun")) {
				chargeSprite.Animation = "ChargedRun";
				chargeSprite.Frame = mainSprite.Frame;
			}
			// If not running //
			else if(!state.Equals("Run") && !chargeSprite.Animation.Equals("Charged")) {
				chargeLightAnimations.Play("Charged");
			}
		}
		else if(chargeSprite.Animation.Equals("Charged") || chargeSprite.Animation.Equals("ChargedRun")) {
			chargeLightAnimations.Play("Dissolve");
		}
		else if(!chargeSprite.Animation.Equals("Dissolve")){
			chargeLightAnimations.Play("NoCharge");
		}
		// Force running animation sync //
		if(chargeSprite.Animation.Equals("ChargedRun")) {
			chargeSprite.Frame = mainSprite.Frame;
		}
		SetChargePosition();
	}

 ///// Helpers ///////////////////////////////////////////////////////////////////////////////////////////

	private void SetSpriteDirection(bool leftSide) {
		mainSprite.FlipH = leftSide;
		colorSprite.FlipH = leftSide;
		chargeSprite.FlipH = leftSide;
		if(facingLeft != leftSide) {
			facingLeft = leftSide;
			facingChanged = true;
		}
	}

	private void CheckWhispsSwitched() {
		if(whisperController.whisp != currentWhisp) {
			currentWhisp = whisperController.whisp;
			SetColorModulation(currentWhisp);
		}
	}

	private void SetColorModulation(int whisp) {
		if(whisp == 1) {
			colorSprite.Modulate = green;
		}
		else if(whisp == 2) {
			colorSprite.Modulate = pink;
		}
		else if(whisp == 3) {
			colorSprite.Modulate = blue;
		}
		else if(whisp == 4) {
			colorSprite.Modulate = orange;
		}
		else {
			colorSprite.Modulate = cyan;
		}
		chargeSprite.Modulate = colorSprite.Modulate * 1.5f;
	}

	private void SetChargePosition() {
		if(state.Contains("Jump") || state.Contains("Fall")) {
			if(state.Equals("Fall")) {
				chargeOffset.Y = -1;
			}
			if(facingLeft) {
				chargeOffset.X = -2;
			}
			else {
				chargeOffset.X = 2;
			}
		}
		else {
			chargeOffset.X = 0;
			chargeOffset.Y = 0;
		}
		chargeSprite.Position = chargeOffset;
	}

	private void SetExtendSpinJump() {
		for(int i = 0; i < whisperController.GetSlideCollisionCount() ; i++) {
				if((whisperController.GetSlideCollision(i).GetCollider() as Node).Name.Equals("Spring")) {
					extendSpinJump = true;
					timer.Start(0.1);
				}
		}
	}

	////// SIGNALS ///////////////////////////////////////////////////////

	private void TransitionAnimation() {
		if(!state.Equals("default") && !state.Contains("Transition")) {
			if(state.Equals("Run") || state.Equals("IdleShoot") || state.Equals("Fall")) {
				mainSprite.Animation = mainSprite.Animation + "Transition";
				colorSprite.Animation = colorSprite.Animation + "Transition";
			}
			else {
				mainSprite.Animation = "default";
				colorSprite.Animation = "default";
			}
		}
	}

	private void EndExtendSpinJump() {
		extendSpinJump = false;
	}
}
