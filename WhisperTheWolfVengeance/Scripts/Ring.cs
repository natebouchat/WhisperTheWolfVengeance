using Godot;
using System;

public partial class Ring : CharacterBody2D
{
    private PlayerUI playerUI;
    private AudioStreamPlayer ringGetSFX;
    private Vector2 motion;
    private Random random;
    private bool ringHasPhysics;
    private bool hasBounced;
    private double recollectDelay;

    public override void _Ready()
    {  
        playerUI = GetNode<PlayerUI>("../Whisper/PlayerUI");
        ringGetSFX = GetNode<AudioStreamPlayer>("SFXRingGet");
        ringGetSFX.VolumeDb = _SettingsManager.sfxVolume;
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Modulate = new Color(1,1,1,1);
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("default");

        motion = new Vector2(400, 400);
        ringHasPhysics = false;
        hasBounced = false;
        recollectDelay = 0;
    }

    public override void _Process(double delta) {
        if(ringHasPhysics) {
            RingPhysics(delta);
            MoveAndSlide();
            recollectDelay -= delta;
        }
    }

    public void OnRingEntered(Node Collider) {
        if(recollectDelay <= 0) {
            CollectRing();
        }
    }

    public void DropRing(Vector2 spawnPosition) {
        recollectDelay = 0.6;
        this.CollisionMask = 1;
        ringHasPhysics = true;
        this.GlobalPosition = spawnPosition;
        random = new Random();
        int angle = random.Next(360);
        motion.X = (float)(Math.Cos(angle) * 600);
        motion.Y = (float)(Math.Sin(angle) * 600);
        GetNode<AnimationPlayer>("AnimationPlayer").Play("DelayedDisappear"); 
    }

    private async void CollectRing() {
        recollectDelay = 100;
        this.Visible = false;
        playerUI.AddRings(1);
        ringGetSFX.Play();
        await ToSignal(ringGetSFX, "finished");
        this.QueueFree();
    }

    private void RingPhysics(double delta) {
		if(!IsOnFloor()) {
			motion.Y += (float)(delta*1200);
			if(motion.Y > 800) {
				motion.Y = 800;
			}
		}
        else if(hasBounced == false) {
            hasBounced = true;
            motion.Y = (-1*motion.Y)/2;
        }
		else {
			motion.Y = 0;
		}

        if(motion.X > 0) {
            motion.X -= (float)(Math.Sqrt(delta*1500));
            if(motion.X < 0.1) {
                motion.X = 0;
            }
        }
        else if(motion.X < 0) {
            motion.X += (float)(Math.Sqrt(delta*1500));
            if(motion.X > -0.1) {
                motion.X = 0;
            }
        }

        this.Velocity = motion;
	}

}
