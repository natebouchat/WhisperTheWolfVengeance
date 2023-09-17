using Godot;
using System;

public partial class ControlsMenu : Control
{
	private AnimationPlayer menuAnim;
	private PackedScene options;
	private OptionButton promptMode;
	private ControlConfigButtons[] keybindButtons;
	private bool disabled;
	
	public override void _Ready()
	{
		GetNode<Label>("../PauseTitle").Text = "Controls";
		
		promptMode = GetNode<OptionButton>("ScrollContainer/VBoxContainer/HBoxContainer/OptionButton");
		menuAnim = GetNode<AnimationPlayer>("AnimationPlayer");
		menuAnim.Play("SlideIn");
		keybindButtons = new ControlConfigButtons[GetNode("ScrollContainer/VBoxContainer/ControlsButtons").GetChildCount()/2];
		for(int i = 1; i < keybindButtons.Length * 2; i += 2) {
			keybindButtons[(i-1)/2] = GetNode("ScrollContainer/VBoxContainer/ControlsButtons").GetChild<ControlConfigButtons>(i);
		}
		disabled = false;

		InitializePromptButtons();
		SetControlPrompts();
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("pause") && !disabled) {
			LeaveControls();
		}
	}

	private async void LeaveControls() {
		disabled = true;
		_SettingsManager.SaveSettings();
		menuAnim.PlayBackwards("SlideIn");
		options = ResourceLoader.Load<PackedScene>("res://Components/OptionsMenu.tscn");
		OptionsMenu optionsMenu = (OptionsMenu)options.Instantiate();
		GetParent().AddChild(optionsMenu);
		optionsMenu.SlideButtonsFromRight();
		await ToSignal(menuAnim, "animation_finished");
		this.QueueFree();
	}

	private void InitializePromptButtons() {
		foreach(ControlConfigButtons aButton in keybindButtons) {
			foreach(InputEvent anEvent in InputMap.ActionGetEvents(aButton.Name)) {
				if(anEvent is InputEventKey aKey) {
					aButton.key = aKey;
				}
				else if(anEvent is InputEventMouseButton aMouseButton) {
					aButton.mouseButton = aMouseButton;
				}
				else if(anEvent is InputEventJoypadButton aJoyButton) {
					aButton.joyButton = aJoyButton;
				}
			}
		}
	}

	private void SetControlPrompts() {		
		if(promptMode.Selected == 0) {
			//detect
			if(Input.GetConnectedJoypads().Count == 0) {
				foreach(ControlConfigButtons aButton in keybindButtons) {
					aButton.SetButtonText(ControlConfigButtons.KeybindMode.Keyboard);
				}
			}
		}
		else if(promptMode.Selected == 1) {
			foreach(ControlConfigButtons aButton in keybindButtons) {
				aButton.SetButtonText(ControlConfigButtons.KeybindMode.Keyboard);
			}
		}
		else if(promptMode.Selected == 2) {
			//Xbox
		}
		else if(promptMode.Selected == 3) {
			//Playstation
		}
		else if(promptMode.Selected == 4) {
			//Nintendo
		}
	}

	/////// Signals /////////////////////////////////////////////////////////////////////////////

	private void OnBackPressed() {
		LeaveControls();
	}
}
