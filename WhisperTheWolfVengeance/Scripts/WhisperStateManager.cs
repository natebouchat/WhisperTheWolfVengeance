using Godot;
using System;

public class WhisperStateManager : AnimationPlayer
{
    private AnimatedSprite mainSprite;
    private AnimatedSprite colorSprite;

    public Vector2 motion {get; set;}
    bool onFloor {get; set;}

    public override void _Ready()
    {
        mainSprite = GetNode<AnimatedSprite>("../MainSprite");
        colorSprite = GetNode<AnimatedSprite>("../MainSprite/ColorSprite");
    }

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


}
