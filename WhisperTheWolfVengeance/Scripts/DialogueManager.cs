using Godot;
using System;
using System.Collections.Generic;

public partial class DialogueManager : CanvasLayer
{
	[Export]
	private float textSpeed = 0.05f;

	private Polygon2D indicator;
	private RichTextLabel displayedText;
	private Timer timer;
	private Sprite2D characterSpriteL;
	private Sprite2D characterSpriteR;
	private List<string> dialogue;
	private int phraseNum;
	private bool finished;
	private bool hasPulledFromFile;

	public WhisperController controller{get; set;}

	public override void _Ready()
	{
		indicator = GetNode<Polygon2D>("Indicator");
		displayedText = GetNode<RichTextLabel>("TextBox/RichTextLabel");
		timer = GetNode<Timer>("Timer");
		characterSpriteL = GetNode<Sprite2D>("LeftPosition/CharacterSpriteL");
		characterSpriteR = GetNode<Sprite2D>("RightPosition/CharacterSpriteR");
		characterSpriteL.GlobalPosition = GetNode<Marker2D>("LeftPosition").GlobalPosition;
		characterSpriteR.GlobalPosition = GetNode<Marker2D>("RightPosition").GlobalPosition;

		timer.WaitTime = textSpeed;
		phraseNum = 0;
		finished = false;
		hasPulledFromFile = false;

	}

	public override void _Process(double delta)
	{
		indicator.Visible = finished;
		if(Input.IsActionJustPressed("shoot") && hasPulledFromFile) {
			if(finished) {
				NextPhrase();
			}
			else {
				displayedText.VisibleCharacters = (displayedText.Text).Length;
			}
		}
	}

	public void GetDialogueFromFile(string path, Node player) {
		dialogue = new List<string>();
		string aLine = "";
		controller = (WhisperController)player;

		try {
			using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			while (file.GetPosition() < file.GetLength())
			{
				aLine = file.GetLine();
				if(!string.IsNullOrWhiteSpace(aLine)) {
					dialogue.Add(aLine);
				}
			}
			file.Close();
		}
		catch(Exception e) {
			dialogue.Add("Error reading from file: " + path);
		}
		hasPulledFromFile = true;
		NextPhrase();
	}
 
	private async void NextPhrase() {
		if(phraseNum >= dialogue.Count) {
			controller.disableControls = false;
			this.QueueFree();
		}
		else {
			while(dialogue[phraseNum][0] == '[') {
				if(dialogue[phraseNum][1] == '<') {
					characterSpriteL.Texture = SetSpriteTexture(dialogue[phraseNum]);
				}
				else {
					characterSpriteR.Texture = SetSpriteTexture(dialogue[phraseNum]);
				}
				phraseNum++;
			}

			finished = false;
			displayedText.Text = dialogue[phraseNum];
			displayedText.VisibleCharacters = 0;
			
			while (displayedText.VisibleCharacters < (displayedText.Text).Length) {
				displayedText.VisibleCharacters += 1;
				
				timer.Start();
				await ToSignal(timer, "timeout");
			}
			phraseNum++;
			finished = true;
		}

	}

	private Texture2D SetSpriteTexture(string characterStill) {
		string emotion = characterStill.Substring(3, characterStill.Length - 4);
		string name = "";
		for(int i = 0; i < emotion.Length; i++) {
			if(emotion[i] == '_') {
				break;
			}
			name += emotion[i];
		}
		CompressedTexture2D image = ResourceLoader.Load<CompressedTexture2D>("res://Assets/Dialogue/Stills/" + name + "/" + emotion + ".png");
		return image;
	}
}
