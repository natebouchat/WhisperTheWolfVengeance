using Godot;
using System;

public class WhisperController : KinematicBody2D
{
    Vector2 UP = new Vector2(0,-1);
    Vector2 motion;
    const int maxSpeed = 300;
    const int maxFallSpeed = 200;
    const int jumpForce = 400;

    public override void _Ready(){
        motion = new Vector2();
    }

    public override void _Process(float delta) {
        motion.y += 20;
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
