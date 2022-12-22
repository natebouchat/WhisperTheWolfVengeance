using Godot;
using System;

public class WhisperController : KinematicBody2D
{
    [Export]
    private int maxSpeed = 550;
    [Export]
    private int maxFallSpeed = 1000;
    [Export]
    private int jumpForce = 600;
    
    private Vector2 UP = new Vector2(0,-1);
    private Vector2 motion;

    public override void _Ready(){
        motion = new Vector2();
    }

    public override void _Process(float delta) {
        gravity(delta);
        playerInput();
        motion = MoveAndSlide(motion, UP);
    }

 /////////////////////////////////////////////////////////////////////////////////////////////////////////

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
        if(IsOnFloor() && Input.IsActionPressed("ui_up")) {
            motion.y = -jumpForce; 
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void gravity(float delta) {
        motion.y += 20*(delta*60);
        if(motion.y > -50) {
            motion.y += 20*(delta*60);
        }
        if(motion.y > maxFallSpeed) {
            motion.y = maxFallSpeed;
        } 
    }

    public System.Object[] getWhisperDetails() {
        System.Object[] obj = new System.Object[] {motion, IsOnFloor()};
        return obj;
    }
}
