using Godot;
using System;

public partial class WhisperStateManager : AnimationPlayer
{
	private WhisperController whisperController;
	private AnimatedSprite2D mainSprite;
	private AnimatedSprite2D colorSprite;
	private AnimatedSprite2D chargeSprite;
	private AnimationPlayer chargeLightAnimations;
	private AudioStreamPlayer dropRingsSFX;
	private System.Object[] details;
	private Vector2 chargeOffset;
	private bool facingLeft;
	private bool facingChanged;
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
		chargeLightAnimations = GetNode<AnimationPlayer>("ChargeLightAnimations");

		dropRingsSFX = GetNode<AudioStreamPlayer>("../DropRings");
        dropRingsSFX.VolumeDb = _SoundManager.sfxVolume;

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
	}

	public override void _Process(double delta) {
		details = whisperController.GetWhisperDetails();
		SetSpriteDirection(whisperController.GetIsFacingLeft());
		state = mainSprite.Animation;
		if(whisperController.hurtCooldown <= 0) {
			SetAnimationState();
			SetChargeLight();
		}
		else if(!(mainSprite.Animation).Equals("Hurt")) {
			dropRingsSFX.Play();
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
				if(!((mainSprite.Animation).Equals("Jump") || (mainSprite.Animation).Equals("JumpEnd"))) {
					this.Play("WhisperJump");
				}
			}
			else { // If downwards motion //
				if(!(mainSprite.Animation).Equals("Fall")) {
					this.Play("WhisperFall");
				}
			}
		}
		// If no longer falling, Transition from falling //
		else if((mainSprite.Animation).Equals("Fall")) {
			TransitionAnimation();
		}
		else { // If on ground //
			// If moving on x axis // 
			if(((Vector2)details[0]).X != 0) {
				if(!(mainSprite.Animation).Equals("Run") || facingChanged) {
					this.Play("WhisperRun");
					facingChanged = false;
				}
			}
			else { // If not moving on x axis //
				// If bullet is not ready (shot)  OR  Bullet is buffered, AND not currently charging //
				if((((Boolean)details[2]) == true || ((Boolean)details[3]) == true) && ((double)details[details.Length - 1] < 0.1)) {
					if(!(mainSprite.Animation).Equals("IdleShoot")) {
						this.Play("WhisperIdleShoot");
					}
				}
				// Bullet Charging or Charged //
				else if((double)details[details.Length - 1] >= 0.1) {
					if(!((mainSprite.Animation).Equals("IdleCharging") || ((mainSprite.Animation).Equals("IdleCharged")))) {
						if((double)details[details.Length - 1] < 0.6) {
							this.Play("WhisperIdleCharge");
						}
						else { // Maintain Charged Sprite2D //
							this.Play("WhisperIdlePreCharged");
						}
					}
				}
				else{ // Idle //
					if(!(mainSprite.Animation).Equals("default")) {
						this.Play("WhisperIdle");
					}
				}
			}
		}
	}



	private void SetChargeLight() {
		// If Bullet is Charging/Charged //
		if((double)details[details.Length - 1] >= 0.1) {
			//if not fully charged
			if((double)details[details.Length - 1] < 0.6) {
				if(!(chargeSprite.Animation).Equals("Charging")) {
					chargeLightAnimations.Play("Charging");
				}
			}
			// If running //
			else if((mainSprite.Animation).Equals("Run") && !(chargeSprite.Animation).Equals("ChargedRun")) {
				chargeSprite.Animation = "ChargedRun";
				chargeSprite.Frame = mainSprite.Frame;
			}
			// If not running //
			else if(!(chargeSprite.Animation).Equals("Charged") && !(mainSprite.Animation).Equals("Run")) {
				chargeLightAnimations.Play("Charged");
			}
		}
		else if((chargeSprite.Animation).Equals("Charged") || (chargeSprite.Animation).Equals("ChargedRun")) {
			chargeLightAnimations.Play("Dissolve");
		}
		else if(!(chargeSprite.Animation).Equals("Dissolve")){
			chargeLightAnimations.Play("NoCharge");
		}
		// Force running animation sync //
		if((chargeSprite.Animation).Equals("ChargedRun")) {
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
		chargeSprite.Modulate = (colorSprite.Modulate)*1.5f;
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

	////// SIGNALS ///////////////////////////////////////////////////////

	private void TransitionAnimation() {
		if(!(mainSprite.Animation).Equals("default") && !((string)(mainSprite.Animation)).Contains("Transition")) {
			mainSprite.Animation = mainSprite.Animation + "Transition";
			colorSprite.Animation = colorSprite.Animation + "Transition";
		}
	}
}
