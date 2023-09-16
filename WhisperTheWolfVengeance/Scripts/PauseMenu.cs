using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	private ColorRect pauseBG;
	private Sprite2D grid;
	private Label header;
	private AnimationPlayer menuAnim;
	private Button[] buttons;
	private PackedScene options;
	private bool onPauseMenu;
	
	public override void _Ready()
	{
		this.Visible = false;
		pauseBG = GetNode<ColorRect>("PauseBG");
		pauseBG.Scale = new Vector2(0.1f, 0.1f);
		grid = GetNode<Sprite2D>("PauseBG/PauseGrid");
		grid.Visible = false;
		header = GetNode<Label>("PauseBG/PauseTitle");
		menuAnim = GetNode<AnimationPlayer>("MenuAnim");
		GetNode<AnimationPlayer>("PauseBG/BGAnimation").Play("Idle");
		GetNode<VBoxContainer>("PauseBG/PauseButtons").Position = new Vector2(692.5f, 220);

		buttons = new Button[GetNode("PauseBG/PauseButtons").GetChildCount()];
		for(int i = 0; i < buttons.Length; i++) {
			buttons[i] = GetNode("PauseBG/PauseButtons").GetChild<Button>(i);
			buttons[i].Disabled = true;
		}
		onPauseMenu = true;
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("pause") && onPauseMenu) {
			PauseButtonPressed();
		}
		if(GetTree().Paused == true) {
			SetButtonFocus();
		}
	}

	public void ReturnToPauseMenu() {
		menuAnim.Play("SlideButtons");
		SetButtonState(false);
		buttons[0].GrabFocus();
		onPauseMenu = true;
		header.Text = "Paused";
	}

	private void SetButtonState(bool disabled) {
		for(int i = 0; i < buttons.Length; i++) {
			buttons[i].Disabled = disabled;
			if(disabled) {
				buttons[i].FocusMode = (Control.FocusModeEnum)0;
			}
			else {
				buttons[i].FocusMode = (Control.FocusModeEnum)2;
			}
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
			menuAnim.Play("FadeIn");
			this.Visible = true;
			await ToSignal(menuAnim, "animation_finished");
			grid.Visible = true;
		}
		else {
			menuAnim.PlayBackwards("FadeIn");
			grid.Visible = false;
			await ToSignal(menuAnim, "animation_finished");
			menuAnim.Play("SlideButtons");
			this.Visible = false;
		}
	}

	/////// Signals /////////////////////////////////////////////////////////////////////////////

	private void OnResumePressed() {
		PauseButtonPressed();
	}

	private void OnWhispsPressed() {
		menuAnim.PlayBackwards("SlideButtons");
		SetButtonState(true);
	}

	private void OnOptionsPressed() {
		onPauseMenu  = false;
		menuAnim.PlayBackwards("SlideButtons");
		SetButtonState(true);
		options = ResourceLoader.Load<PackedScene>("res://Components/OptionsMenu.tscn");
		OptionsMenu optionsMenu = (OptionsMenu)options.Instantiate();
		pauseBG.AddChild(optionsMenu);
	}

	private void OnMainMenuPressed() {
		//TEMP
		GetTree().Quit();
	}
}
