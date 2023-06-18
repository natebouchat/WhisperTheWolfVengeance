using Godot;
using System;
using System.Collections.Generic;

public partial class DialogueManager : ColorRect
{
	[Export]
	public string dialoguePath {get; set;}
	[Export]
	private float textSpeed = 0.05f;

	private Polygon2D indicator;
	private RichTextLabel displayedText;
	private Timer timer;
	private List<string> dialogue;
	private int phraseNum;
	private bool finished;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		indicator = GetNode<Polygon2D>("Indicator");
		displayedText = GetNode<RichTextLabel>("RichTextLabel");
		timer = GetNode<Timer>("Timer");
		timer.WaitTime = textSpeed;
		
		dialogue = GetDialogue();
		phraseNum = 0;
		finished = false;
		NextPhrase();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		indicator.Visible = finished;
		if(Input.IsActionJustPressed("shoot")) {
			if(finished) {
				NextPhrase();
			}
			else {
				displayedText.VisibleCharacters = (displayedText.Text).Length;
			}
		}
	}

	private List<string> GetDialogue() {

		List<string> lines = new List<string>();
		
		try {
			using var file = FileAccess.Open(dialoguePath, FileAccess.ModeFlags.Read);
			while (file.GetPosition() < file.GetLength())
			{
				lines.Add(file.GetLine());
				if(string.IsNullOrWhiteSpace(lines[lines.Count-1])) {
					lines.RemoveAt(lines.Count - 1);
				}
			}
			file.Close();
		}
		catch(Exception e) {
			lines.Add("Error reading from file: " + dialoguePath);
			GD.PrintErr(e);
		}
		
		return lines;
	}

	
	
 
	private async void NextPhrase() {
		
		if(phraseNum >= dialogue.Count) {
			QueueFree();
		}
		else {
			finished = false;
		
			displayedText.Text = dialogue[phraseNum];
			displayedText.VisibleCharacters = 0;

			/*
			//File f = File.new()
			//var img = dialog[phraseNum]["Name"] + dialog[phraseNum]["Emotion"] + ".png";
			//if f.file_exists(img):
			//	$Portrait.texture = load(img);
			//else: $Portrait.texture = null
			*/
			
			while (displayedText.VisibleCharacters < (displayedText.Text).Length) {
				displayedText.VisibleCharacters += 1;
				
				timer.Start();
				await ToSignal(timer, "timeout");
			}
			
			finished = true;
			phraseNum += 1;

		}

	}
}
