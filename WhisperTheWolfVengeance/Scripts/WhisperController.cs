using Godot;
using System;

public class WhisperController : KinematicBody2D
{
    private Vector2 UP = new Vector2(0,-1);
    private Vector2 motion;
    private const int maxSpeed = 300;
    private const int maxFallSpeed = 200;
    private const int jumpForce = 400;

    public override void _Ready(){
        motion = new Vector2();
    }

    public override void _Process(float delta) {
        motion.y += 20*(delta*60);
        if(motion.y > maxFallSpeed) {

        } 
        playerInput();
        motion = MoveAndSlide(motion, UP);
    }



    private void playerInput() {
        if(Input.IsActionPressed("ui_right")) {
            motion.x = maxSpeed;
        }
        else if(Input.IsActionPressed("ui_left")) {
            motion.x = -maxSpeed;
        }
        else {
            motion.x = 0;
        }
        
        if(IsOnFloor()) {
            if(Input.IsActionPressed("ui_up")) {
                motion.y = -jumpForce;
            }  
        }

    }
}
