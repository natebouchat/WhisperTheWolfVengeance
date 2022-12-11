using Godot;
using System;

public class WhisperStateManager : AnimationPlayer
{
    AnimatedSprite mainSprite;
    AnimatedSprite colorSprite;

    public override void _Ready()
    {
        mainSprite = GetNode<AnimatedSprite>("../KinematicBody2D/MainSprite");
        colorSprite = GetNode<AnimatedSprite>("../KinematicBody2D/MainSprite/ColorSprite");
    }

    public void flipSprite(bool leftSide) {
        mainSprite.FlipH = leftSide;
        colorSprite.FlipH = leftSide;
    }


}
