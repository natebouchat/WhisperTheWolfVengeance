using Godot;
using System;

public partial class ControlsMenu : Control
{
	private AnimationPlayer menuAnim;
	private PackedScene options;
	private PackedScene confirmationMenu;
	private OptionButton promptMode;
	private ControlConfigButtons[] keybindButtons;
	private bool disabled;
	
	public override void _Ready()
	{
		GetNode<Label>("../PauseTitle").Text = "Controls";
		confirmationMenu = ResourceLoader.Load<PackedScene>("res://Components/ConfirmationMenu.tscn");
		
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
		promptMode.GrabFocus();
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("pause") && !disabled) {
			OnBackPressed();
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

	private void SaveAndLeaveControls() {
		_SettingsManager.SaveSettings();
		LeaveControls();
	}

	private void ResetBinds() {
		InputMap.LoadFromProjectSettings();
		InitializePromptButtons();
		SetControlPrompts();
		promptMode.GrabFocus();
	}

	private void ResetFocus() {
		promptMode.GrabFocus();
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
			else if(Input.GetJoyName(0).ToLower().Contains("nintendo")) {
				foreach(ControlConfigButtons aButton in keybindButtons) {
					aButton.SetButtonText(ControlConfigButtons.KeybindMode.Nintendo);
				}
			}
			else if(Input.GetJoyName(0).ToLower().Contains("ps") || Input.GetJoyName(0).ToLower().Contains("playstation")) {
				foreach(ControlConfigButtons aButton in keybindButtons) {
					aButton.SetButtonText(ControlConfigButtons.KeybindMode.Playstation);
				}
			}
			else {
				foreach(ControlConfigButtons aButton in keybindButtons) {
					aButton.SetButtonText(ControlConfigButtons.KeybindMode.Xbox);
				}
			}
		}
		else if(promptMode.Selected == 1) {
			foreach(ControlConfigButtons aButton in keybindButtons) {
				aButton.SetButtonText(ControlConfigButtons.KeybindMode.Keyboard);
			}
		}
		else if(promptMode.Selected == 2) {
			foreach(ControlConfigButtons aButton in keybindButtons) {
				aButton.SetButtonText(ControlConfigButtons.KeybindMode.Xbox);
			}
		}
		else if(promptMode.Selected == 3) {
			foreach(ControlConfigButtons aButton in keybindButtons) {
				aButton.SetButtonText(ControlConfigButtons.KeybindMode.Playstation);
			}
		}
		else if(promptMode.Selected == 4) {
			foreach(ControlConfigButtons aButton in keybindButtons) {
				aButton.SetButtonText(ControlConfigButtons.KeybindMode.Nintendo);
			}
		}
	}

	/////// Signals /////////////////////////////////////////////////////////////////////////////

	private void ControlPromptsChanged(int aNum) {
		promptMode.Selected = aNum;
		SetControlPrompts();
	}
	
	private void OnBackPressed() {
		ConfirmationMenu confirmation = (ConfirmationMenu)confirmationMenu.Instantiate();
		this.AddChild(confirmation);
		confirmation.SetMenuElements("Save Bindings?", "Yes", "No", SaveAndLeaveControls, LeaveControls);
	}

	private void OnResetPressed() {
		ConfirmationMenu confirmation = (ConfirmationMenu)confirmationMenu.Instantiate();
		this.AddChild(confirmation);
		confirmation.SetMenuElements("Reset Bindings?", "Yes", "No", ResetBinds, ResetFocus);
	}
}
