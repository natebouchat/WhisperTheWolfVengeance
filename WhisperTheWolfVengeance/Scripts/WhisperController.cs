using Godot;
using System;

public class WhisperController : KinematicBody2D
{
    [Export]
    private int maxSpeed = 450;
    [Export]
    private int maxFallSpeed = 1000;
    [Export]
    private int jumpForce = 600;
    
    private Vector2 UP = new Vector2(0,-1);
    private Vector2 motion;
    private WhisperStateManager stateManager;
    private DebugDetails debug;

    public override void _Ready(){
        motion = new Vector2();
        stateManager = GetNode<WhisperStateManager>("WhisperAnimations");
        debug = GetNode<DebugDetails>("PlayerUI/DebugDetails");
    }

    public override void _Process(float delta) {
        gravity(delta);
        playerInput();
        motion = MoveAndSlide(motion, UP);
        if(debug.Visible) {
            System.Object[] details = new System.Object[] {motion, IsOnFloor()};
            debug.setDebugDetails(details);
        }
    }

 /////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void playerInput() {
        if(Input.IsActionPressed("ui_right")) {
            motion.x = maxSpeed;
            stateManager.flipSprite(false);
        }
            else if(Input.IsActionPressed("ui_left")) {
                motion.x = -maxSpeed;
                stateManager.flipSprite(true);
            }
            else {
                motion.x = 0;
            }
        if(IsOnFloor() && Input.IsActionPressed("ui_up")) {
            motion.y = -jumpForce; 
        }
        if(Input.IsActionJustPressed("ui_focus_next")) {
            debug.Visible = !debug.Visible;
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
}
