using Godot;
using System;

public class WhisperStateManager : AnimationPlayer
{
    private WhisperController whisperController;
    private AnimatedSprite mainSprite;
    private AnimatedSprite colorSprite;
    private System.Object[] details;
    private int priority;

    public string state {get; set;}

    public override void _Ready()
    {
        whisperController = GetParent<WhisperController>();
        mainSprite = GetNode<AnimatedSprite>("../MainSprite");
        colorSprite = GetNode<AnimatedSprite>("../MainSprite/ColorSprite");
        state = "default";
        priority = 0;
    }

    public override void _Process(float delta) {
        details = whisperController.getWhisperDetails();
        // Transitions take priority (priority = 3) //

        if(priority <= 2) {
            setSpriteDirection(whisperController.getIsFacingLeft());
            // If moving on x axis // 
            if(((Vector2)details[0]).x != 0) {
                // If on ground //
                if(((Boolean)details[1]) == true) {
                    if(!state.Equals("Run")) {
                        animateTransition("Run", false);
                    }
                    else {
                        setAnimation("Run");
                    }
                }
                // If not on ground //
                else{
                    state = "default";
                    setAnimation("default");
                }
            }
            // If not moving on x axis //
            else {
                // If bullet is not ready (shot)  |OR|  bufferBullet is true//
                if(((Boolean)details[2]) == true ) {
                    if(priority <= 1) {
                        setPriorityAnimation("IdleShoot", 1);
                    }
                    else {
                        animateTransition("IdleShoot", false);
                    }
                }
                // IdleShoot has priority over default idle //
                else if(priority == 0){
                    if(!state.Equals("default")) {
                        animateTransition(state, true);
                    }
                    else {
                        setAnimation("default");
                    }
                }
            }
        }
        
    }

///////////////////////////////////////////////////////////////////////////////////////////////////

    private void setSpriteDirection(bool leftSide) {
        mainSprite.FlipH = leftSide;
        colorSprite.FlipH = leftSide;
    }

    private void setAnimation(string name) {
        mainSprite.Animation = name;
        colorSprite.Animation = name;
    }

    private async void setPriorityAnimation(string name, int num) {
        priority = num;
        state = name;
        mainSprite.Animation = name;
        colorSprite.Animation = name;
        await ToSignal(mainSprite, "animation_finished");
        priority = 0;
    }

    private async void animateTransition(string name, bool toDefault) {
        priority = 3;
        if(toDefault) {
            state = "default";
        }
        else {
            state = name;
        }
        mainSprite.Animation = name + "Transition";
        colorSprite.Animation = name + "Transition";
        await ToSignal(mainSprite, "animation_finished");
        priority = 0;
    }

}
