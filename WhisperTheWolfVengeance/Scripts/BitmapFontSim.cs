using Godot;
using System;

[Tool]
public partial class BitmapFontSim : Label
{
	[Export]
	private int kerning = -2;
	[Export]
	private Vector2 letterScale = new Vector2(1,1);
	
	private AnimatedSprite2D fontSprites;
	private Vector2 letterPos;
	private Vector2 labelSize;
	private string labelText;

	public override void _Ready()
	{
		fontSprites = GetNode<AnimatedSprite2D>("FontSheet");
		letterPos = new Vector2(0,letterScale.X * 30);
		labelSize = new Vector2(10, letterScale.X * 70);
		labelText = "";
	}
	

	public override void _Process(double delta)
	{
		if(!labelText.Equals(this.Text)) {
			labelText = this.Text;
			ResetFontChildren();
			SetTextToFont();
		}
	}

	private void SetTextToFont() {
		for(int i = 0; i < this.Text.Length; i++) {
			Sprite2D letter = new Sprite2D();
			if(this.Text[i] == ' ') {
				letterPos.X += letterScale.X * 50;
			}
			else {
				letter.Texture = GetLetterTexture(this.Text[i]);
				letter.Scale = letterScale;
				letterPos.X += (letter.Texture.GetWidth()/2 + kerning) * letterScale.X;
				letter.Position = letterPos;
				letterPos.X += (letter.Texture.GetWidth()/2 + kerning) * letterScale.X;
				
				labelSize.X = letterPos.X;
				labelSize.Y = letterScale.X * 70;
				this.Size = labelSize;
				AddChild(letter);
			}
		}
	}
	
	private void ResetFontChildren() {
		letterPos = new Vector2(0,letterScale.X * 30);
		labelSize.X = 10;
		for(int i = 1; i < this.GetChildCount(); i++) {
			this.GetChild(i).QueueFree();
		}
	}

	private Texture2D GetLetterTexture(char ch) {
		//if number
		if(((int)ch) >= 48 && ((int)ch) <= 57 ) {
			return fontSprites.SpriteFrames.GetFrameTexture("default", ch - 48);
		}
		//if Uppercase
		else if((((int)ch) >= 65 && ((int)ch) <= 90)) {
			return fontSprites.SpriteFrames.GetFrameTexture("default", ch - 55);
		}
		//if Lowercase
		else if((((int)ch) >= 97 && ((int)ch) <= 122)) {
			return fontSprites.SpriteFrames.GetFrameTexture("default", ch - 87);
		}
		//if created symbol
		else {
			if(ch == '?') {
				return fontSprites.SpriteFrames.GetFrameTexture("default", 36);
			}
			else if(ch == ':') {
				return fontSprites.SpriteFrames.GetFrameTexture("default", 37);
			}
			else if(ch == ',') {
				return fontSprites.SpriteFrames.GetFrameTexture("default", 38);
			}
			else if(ch == '!') {
				return fontSprites.SpriteFrames.GetFrameTexture("default", 39);
			}
			else if(ch == '.') {
				return fontSprites.SpriteFrames.GetFrameTexture("default", 40);
			}
			else if(ch == '"') {
				return fontSprites.SpriteFrames.GetFrameTexture("default", 41);
			}
			else {
				return fontSprites.SpriteFrames.GetFrameTexture("default", 42);
			}
		}
	}
}
