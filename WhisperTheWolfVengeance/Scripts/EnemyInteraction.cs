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
	}

	public void OnAreaEntered(Node Collider) {
        if(((string)(Collider.Name)).Contains("Whisp")) {
			if(((string)(Collider.Name)).Contains("Charged")) {
				damage += 10;
			}
			else {
				damage += 5;
			}
        }
	}
}
