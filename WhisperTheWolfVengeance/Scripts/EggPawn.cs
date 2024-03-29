using Godot;
using System;

public partial class EggPawn : CharacterBody2D
{
    [Export]
    private int maxFallSpeed = 1000;

    private EnemyInteraction interaction;
    private AnimationPlayer enemyAnimation;
    private AnimatedSprite2D explosion;
    private Vector2 motion;
    private int health;

    public override void _Ready()
    {
        interaction = GetNode<EnemyInteraction>("Area2D");
        enemyAnimation = GetNode<AnimationPlayer>("EnemyAnimation");
		enemyAnimation.Play("Idle");
        explosion = GetNode<AnimatedSprite2D>("Explosion");
        explosion.Visible = false;
        motion = new Vector2();
        health = 25;

    }

    public override void _Process(double delta)
    {
        Gravity(delta);
        this.Velocity = motion;
        MoveAndSlide();
        if(interaction.damage != 0) {
            health -= interaction.damage;
            if(health <= 0) {
                explosion.Play();
                interaction.CollisionMask = 0;
                enemyAnimation.Play("Die");
            }
            else {
                enemyAnimation.Play("Hurt");
                interaction.damage = 0;
            }
        }
    }

    public void AddMotion(int xVal, int yVal) {
		motion.X += xVal;
		motion.Y += yVal;
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
