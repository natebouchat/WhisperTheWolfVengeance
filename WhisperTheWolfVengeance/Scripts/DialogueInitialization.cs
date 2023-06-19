using Godot;
using System;

public partial class DialogueInitialization : Area2D
{
	[Export]
	public string textFilePath = "";
	private PackedScene dialogueManager;
	
	public override void _Ready()
	{
		dialogueManager = ResourceLoader.Load<PackedScene>("res://PreFabs/DialogueManager.tscn");
	}

	public void BeginDialogueMode(Node Collider) {
		((WhisperController)Collider).disableControls = true;
		DialogueManager dialogue = (DialogueManager)dialogueManager.Instantiate();
		AddChild(dialogue);
		dialogue.GetDialogueFromFile(textFilePath, Collider);
	}



}
