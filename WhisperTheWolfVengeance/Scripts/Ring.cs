using Godot;
using System;

public class Ring : Area2D
{
    private CollisionShape2D WhisperCollide;
    private PlayerUI playerUI;

    public override void _Ready()
    {  
        WhisperCollide = GetNode<CollisionShape2D>("../Whisper/CollisionShape2D");
        playerUI = GetNode<PlayerUI>("../Whisper/PlayerUI");
    }

    public void OnRingEntered(Node Collider) {
        if(Collider.Name.Equals("Whisper")) {
            _SoundManager.PlaySFX("RingGet");
            playerUI.addRings(1);
            this.QueueFree();
        }
    }
}
