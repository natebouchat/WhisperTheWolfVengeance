using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	private ColorRect pauseBG;
	private Sprite2D grid;
	private AnimationPlayer fadeInOut;
	private Button[] buttons;
	
	public override void _Ready()
	{
		this.Visible = false;
		pauseBG = GetNode<ColorRect>("ColorRect");
		pauseBG.Scale = new Vector2(0.1f, 0.1f);
		grid = GetNode<Sprite2D>("ColorRect/PauseGrid");
		grid.Visible = false;
		fadeInOut = GetNode<AnimationPlayer>("FadeInOut");
		GetNode<AnimationPlayer>("ColorRect/BGAnimation").Play("Idle");

		buttons = new Button[GetNode("ColorRect/VBoxContainer").GetChildCount()];
		for(int i = 0; i < buttons.Length; i++) {
			buttons[i] = GetNode("ColorRect/VBoxContainer").GetChild<Button>(i);
			buttons[i].Disabled = true;
		}
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("pause")) {
			PauseButtonPressed();
		}
		if(GetTree().Paused == true) {
			SetButtonFocus();
		}
	}

	private void SetButtonState(bool disabled) {
		for(int i = 0; i < buttons.Length; i++) {
			buttons[i].Disabled = disabled;
		}
	}

	private void SetButtonFocus() {
		for(int i = 0; i < buttons.Length; i++) {
			if(buttons[i].IsHovered()) {
				buttons[i].GrabFocus();
				break;
			}
		}
	}

	private void PauseButtonPressed() {
		GetTree().Paused = !GetTree().Paused;
		if(GetTree().Paused) {
			FadeAnim(true);
			SetButtonState(false);
			buttons[0].GrabFocus();
			try {
				//set bg color to current whisp color
				pauseBG.Modulate = GetNode<AnimatedSprite2D>("../MainSprite/ColorSprite").Modulate;
			}
			catch(Exception) {
				pauseBG.Modulate = new Color(0.42f, 0.76f, 0.74f);
			}
		}
		else {
			FadeAnim(false);
			SetButtonState(true);
			}
	}

	private async void FadeAnim(bool fadeIn) {
		if(fadeIn) {
			fadeInOut.Play("FadeIn");
			this.Visible = true;
			await ToSignal(fadeInOut, "animation_finished");
			grid.Visible = true;
		}
		else {
			fadeInOut.PlayBackwards("FadeIn");
			grid.Visible = false;
			await ToSignal(fadeInOut, "animation_finished");
			this.Visible = false;
		}
	}

	/////// Signals /////////////////////////////////////////////////////////////////////////////

	private void OnResumePressed() {
		PauseButtonPressed();
	}

	private void OnWhispsPressed() {

	}

	private void OnOptionsPressed() {
		
	}

	private void OnMainMenuPressed() {
		//TEMP
		GetTree().Quit();
	}
}