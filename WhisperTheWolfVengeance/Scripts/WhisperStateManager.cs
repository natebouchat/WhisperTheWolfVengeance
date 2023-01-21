using Godot;
using System;

public class WhisperStateManager : AnimationPlayer
{
    private WhisperController whisperController;
    private AnimatedSprite mainSprite;
    private AnimatedSprite colorSprite;
    private AnimatedSprite chargeSprite;
    private AnimationPlayer chargeLightAnimations;
    private System.Object[] details;
    private bool facingLeft;
    private bool facingChanged;

    public string state {get; set;}

    public override void _Ready()
    {
        whisperController = GetParent<WhisperController>();
        mainSprite = GetNode<AnimatedSprite>("../MainSprite");
        mainSprite.Animation = "default";
        mainSprite.Playing = true;
        colorSprite = GetNode<AnimatedSprite>("../MainSprite/ColorSprite");
        colorSprite.Animation = "default";
        colorSprite.Playing = true;
        chargeSprite = GetNode<AnimatedSprite>("../MainSprite/ChargingSprite");
        chargeSprite.Animation = "default";
        chargeSprite.Playing = true;
        chargeLightAnimations = GetNode<AnimationPlayer>("ChargeLightAnimations");

        facingLeft = false;
        facingChanged = false;
    }

    public override void _Process(float delta) {
        details = whisperController.getWhisperDetails();
        setSpriteDirection(whisperController.getIsFacingLeft());
        state = mainSprite.Animation;

        // If moving on x axis // 
        if(((Vector2)details[0]).x != 0) {
            // If on ground //
            if(((Boolean)details[1]) == true) {
                if(!(mainSprite.Animation).Equals("Run") || facingChanged) {
                    this.Play("WhisperRun");
                    facingChanged = false;
                }
            }
            // If not on ground //
            else{
                if(!(mainSprite.Animation).Equals("default")) {
                    this.Play("WhisperIdle");
                }
            }
        }
        // If not moving on x axis //
        else {
            // If bullet is not ready (shot)  OR  Bullet is buffered //
            if(((Boolean)details[2]) == true || ((Boolean)details[3]) == true) {
                if(!(mainSprite.Animation).Equals("IdleShoot")) {
                    this.Play("WhisperIdleShoot");
                }
            }
            else if((float)details[details.Length - 1] >= 0.1f) {
                if(!((mainSprite.Animation).Equals("IdleCharging") || ((mainSprite.Animation).Equals("IdleCharged")))) {
                    this.Play("WhisperIdleCharge");
                }
            }
            else{
                if(!(mainSprite.Animation).Equals("default")) {
                    this.Play("WhisperIdle");
                }
            }
        }

        ////// Charge Light //////

        if((float)details[details.Length - 1] >= 0.1f) {
            if(!(chargeSprite.Animation).Equals("Charging") && (float)details[details.Length - 1] <= 0.6f) {
                chargeLightAnimations.Play("Charging");
            }
        }
        else if((chargeSprite.Animation).Equals("Charged")){
            chargeLightAnimations.Play("Dissolve");
        }
        else if(!(chargeSprite.Animation).Equals("Dissolve")){
            chargeLightAnimations.Play("NoCharge");
        }
                    
    }

///////////////////////////////////////////////////////////////////////////////////////////////////

    private void setSpriteDirection(bool leftSide) {
        mainSprite.FlipH = leftSide;
        colorSprite.FlipH = leftSide;
        chargeSprite.FlipH = leftSide;
        if(facingLeft != leftSide) {
            facingLeft = leftSide;
            facingChanged = true;
        }
    }

    private void setColorModulation() {

    }

    ////// SIGNALS ///////////////////////////////////////////////////////

    private void transitionToDefault() {
        if(!(mainSprite.Animation).Equals("default") && !(mainSprite.Animation).Contains("Transition")) {
            mainSprite.Animation = mainSprite.Animation + "Transition";
            colorSprite.Animation = colorSprite.Animation + "Transition";
        }
    }

}
