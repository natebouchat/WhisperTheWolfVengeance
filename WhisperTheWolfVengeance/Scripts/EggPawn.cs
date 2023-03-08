using Godot;
using System;

public partial class EggPawn : Area2D
{
    [Export]
    private int maxFallSpeed = 1000;

    private Vector2 motion;
    private Vector2 UP = new Vector2(0,-1);
    private float temp;

    public override void _Ready()
    {
        motion = new Vector2(this.Position.X, this.Position.Y);
    }

    public override void _Process(double delta)
    {
        if(this.GetOverlappingBodies().Count != 0) {
            for(int i = 0; i < this.GetOverlappingBodies().Count; i++) {
                if(!(((Node)(this.GetOverlappingBodies()[i])).Name).Equals("TileMap")) {
                    gravity(delta);
                }
                /*
                if((((Node)(this.GetOverlappingBodies()[i])).Name).Contains("Whisp")) {
                    if(!(((Node)(this.GetOverlappingBodies()[i])).Name).Equals("Whisper")) {
                        this.QueueFree();
                    }
                }
                */
            }
        }
        else {
            gravity(delta);
        }
    }

    private void gravity(double delta) {
        motion.Y += (float)(5*(delta*60));
        if(motion.Y > -50) {
            motion.Y += (float)(20*(delta*60));
        }
        if(motion.Y > maxFallSpeed) {
            motion.Y = maxFallSpeed;
        } 
        this.Position = motion;
    }
}
