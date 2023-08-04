using Godot;
using System;

public partial class ControlsMenu : Control
{
	private AnimationPlayer menuAnim;
	private PackedScene options;
	private bool disabled;
	
	public override void _Ready()
	{
		GetNode<Label>("../PauseTitle").Text = "Controls";
		
		menuAnim = GetNode<AnimationPlayer>("AnimationPlayer");
		menuAnim.Play("SlideIn");
		disabled = false;
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("pause") && !disabled) {
			LeaveControls();
		}
	}

	private async void LeaveControls() {
		disabled = true;
		menuAnim.PlayBackwards("SlideIn");
		options = ResourceLoader.Load<PackedScene>("res://Components/OptionsMenu.tscn");
		OptionsMenu optionsMenu = (OptionsMenu)options.Instantiate();
		GetParent().AddChild(optionsMenu);
		optionsMenu.SlideButtonsFromRight();
		await ToSignal(menuAnim, "animation_finished");
		this.QueueFree();
	}

	/////// Signals /////////////////////////////////////////////////////////////////////////////

	private void OnBackPressed() {
		LeaveControls();
	}
}
