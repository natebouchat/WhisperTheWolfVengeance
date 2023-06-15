using Godot;
using System;

public partial class EggPawn : CharacterBody2D
{
    [Export]
    private int maxFallSpeed = 1000;

    private EnemyInteraction interaction;
    private Vector2 motion;
    private int health;

    public override void _Ready()
    {
        interaction = GetNode<EnemyInteraction>("Area2D");
        motion = new Vector2();
        health = 10;
    }

    public override void _Process(double delta)
    {
        Gravity(delta);
        this.Velocity = motion;
        MoveAndSlide();
        if(interaction.damage != 0) {
            health -= interaction.damage;
            if(health <= 0) {
                this.QueueFree();
            }
            interaction.damage = 0;
        }
    }

    private void Gravity(double delta) {
        motion.Y += (float)(20*(delta*60));
        if(motion.Y > -50) {
            motion.Y += (float)(20*(delta*60));
        }
        if(motion.Y > maxFallSpeed) {
            motion.Y = maxFallSpeed;
        } 
        
    }
}
