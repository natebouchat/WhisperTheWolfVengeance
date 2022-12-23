using Godot;
using System;

public class WhisperStateManager : AnimationPlayer
{
    private WhisperController whisperController;
    private AnimatedSprite mainSprite;
    private AnimatedSprite colorSprite;
    private System.Object[] details;
    private bool transition;

    public String state {get; set;}

    public override void _Ready()
    {
        whisperController = GetParent<WhisperController>();
        mainSprite = GetNode<AnimatedSprite>("../MainSprite");
        colorSprite = GetNode<AnimatedSprite>("../MainSprite/ColorSprite");
        state = "default";
        transition = false;
    }

    public override void _Process(float delta) {
        details = whisperController.getWhisperDetails();
        if(transition == false) {
            flipSprite(whisperController.getIsFacingLeft());
            if(((Vector2)details[0]).x != 0) { 
                if(((Boolean)details[1]) == true) {
                    if(!state.Equals("Run")) {
                        runTransition("Run");
                    }
                    else {
                        running();
                    }
                }
                else{
                    state = "default";
                    idle();
                }
            }
            else {
                if(!state.Equals("default")) {
                    runTransition("default");
                }
                else {
                    idle();
                }
            }
        }
    }

///////////////////////////////////////////////////////////////////////////////////////////////////

    public void flipSprite(bool leftSide) {
        mainSprite.FlipH = leftSide;
        colorSprite.FlipH = leftSide;
    }

    public void running() {
        mainSprite.Animation = "Run";
        colorSprite.Animation = "Run";
    }

    public void idle() {
        mainSprite.Animation = "default";
        colorSprite.Animation = "default";

    }
    public async void runTransition(String name) {
        transition = true;
        state = name;
        mainSprite.Animation = "RunTransition";
        colorSprite.Animation = "RunTransition";
        await ToSignal(mainSprite, "animation_finished");
        transition = false;
    }

}
