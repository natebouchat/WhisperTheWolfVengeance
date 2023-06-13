using Godot;
using System;

public partial class Ring : Area2D
{
    private CollisionShape2D WhisperCollide;
    private PlayerUI playerUI;
    private AudioStreamPlayer ringGetSFX;
    private bool ringCollected;

    public override void _Ready()
    {  
        WhisperCollide = GetNode<CollisionShape2D>("../Whisper/CollisionShape2D");
        playerUI = GetNode<PlayerUI>("../Whisper/PlayerUI");
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("default");
        ringGetSFX = GetNode<AudioStreamPlayer>("RingGet");
        ringGetSFX.VolumeDb = _SoundManager.sfxVolume;
        ringCollected = false;
    }

    public void OnRingEntered(Node Collider) {
        if(Collider.Name.Equals("Whisper") && ringCollected == false) {
            CollectRing();
        }
    }

    private async void CollectRing() {
            ringCollected = true;
            this.Visible = false;
            playerUI.addRings(1);
            ringGetSFX.Play();
            await ToSignal(ringGetSFX, "finished");
            this.QueueFree();
    }
}
