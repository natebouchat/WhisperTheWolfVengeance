using Godot;
using System;

public class EggPawn : Area2D
{
    [Export]
    private int maxFallSpeed = 1000;

    private Vector2 motion;
    private Vector2 UP = new Vector2(0,-1);
    private float temp;

    public override void _Ready()
    {
        motion = new Vector2(this.Position);
    }

    public override void _Process(float delta)
    {
        if(this.GetOverlappingBodies().Count != 0) {
            for(int i = 0; i < this.GetOverlappingBodies().Count; i++) {
                if(!(((Node)(this.GetOverlappingBodies()[i])).Name).Equals("TileMap")) {
                    gravity(delta);
                }
                if((((Node)(this.GetOverlappingBodies()[i])).Name).Contains("Whisp")) {
                    if(!(((Node)(this.GetOverlappingBodies()[i])).Name).Equals("Whisper")) {
                        this.QueueFree();
                    }
                }
            }
        }
        else {
            gravity(delta);
        }
    }

    private void gravity(float delta) {
        motion.y += 5*(delta*60);
        if(motion.y > -50) {
            motion.y += 20*(delta*60);
        }
        if(motion.y > maxFallSpeed) {
            motion.y = maxFallSpeed;
        } 
        this.Position = motion;
    }
}
