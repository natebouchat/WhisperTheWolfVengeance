using Godot;
using System;

public partial class EnemyInteraction : Area2D
{
	public int damage {get; set;}
	private AnimationPlayer enemyAnimation;

	public override void _Ready()
	{
		damage = 0;
		enemyAnimation = GetNode<AnimationPlayer>("../EnemyAnimation");
		enemyAnimation.Play("Idle");
	}

	public void OnCharacterEntered(Node Collider) {
		if(Collider.Name.Equals("Whisper")) {
			((WhisperController)Collider).WhipserIsHurt();
		}
	}

	public void OnAreaEntered(Node Collider) {
        if(((string)(Collider.Name)).Contains("Whisp")) {
			if(((string)(Collider.Name)).Contains("Charged")) {
				damage += 10;
			}
			else {
				damage += 5;
			}
			enemyAnimation.Play("Hurt");
        }
	}
}
