using Godot;
using System;

public partial class EnemyInteraction : Area2D
{
	public int damage {get; set;}

	public override void _Ready()
	{
		damage = 0;
	}

	public void OnCharacterEntered(Node Collider) {
		((WhisperController)Collider).WhipserIsHurt();
	}

	public void OnAreaEntered(Node Collider) {
        if(((string)(Collider.Name)).Contains("Whisp")) {
			if(((string)(Collider.Name)).Contains("Charged")) {
				damage += 25;
			}
			else {
				damage += 10;
			}
        }
	}
}
