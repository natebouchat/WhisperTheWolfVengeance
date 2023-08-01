using Godot;
using System;

public partial class OptionsMenu : Control
{
	private AnimationPlayer menuAnim;
	private Control[] buttons;
	private bool settingChanged;
	
	public override void _Ready()
	{	
		GetNode<VBoxContainer>("OptionButtons").Position = new Vector2(2189, 218);
		menuAnim = GetNode<AnimationPlayer>("AnimationPlayer");
		menuAnim.Play("SlideButtonsIn");
		settingChanged = false;

		buttons = new Control[6];
		buttons[0] = GetNode<Control>("OptionButtons/GridContainer/MusicSlider");
		((HSlider)buttons[0]).Value = _SettingsManager.musicVolume;
		buttons[1] = GetNode<Control>("OptionButtons/GridContainer/SFXSlider");
		((HSlider)buttons[1]).Value = _SettingsManager.sfxVolume;
		buttons[2] = GetNode<Control>("OptionButtons/GridContainer/FullScreenCheck");
		buttons[3] = GetNode<Control>("OptionButtons/GridContainer/LegacyJumpCheck");
		buttons[4] = GetNode<Control>("OptionButtons/Controls");
		buttons[5] = GetNode<Control>("OptionButtons/Back");

		buttons[0].GrabFocus();
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("pause")) {
			PauseButtonPressed();
		}
	}

	private void SetButtonFocus(int i) {
		buttons[i].GrabFocus();
	}

	private async void PauseButtonPressed() {
		menuAnim.PlayBackwards("SlideButtonsIn");
		GetNode<PauseMenu>("../../").ReturnToPauseMenu();
		await ToSignal(menuAnim, "animation_finished");
		this.QueueFree();
	}

	private void UpdateSoundVolume(Node aNode) {
		if(aNode is AudioStreamPlayer) {
			if((((string)aNode.Name).Substr(0, 3)).Equals("SFX")) {
				((AudioStreamPlayer)aNode).VolumeDb = _SettingsManager.sfxVolume;
			}
			else {
				((AudioStreamPlayer)aNode).VolumeDb = _SettingsManager.musicVolume;
			}
		}
		for(int i = 0; i < aNode.GetChildCount(); i++) {
			UpdateSoundVolume(aNode.GetChild(i));
		}
	}

/////// Signals /////////////////////////////////////////////////////////////////////////////

	private void OnMusicSliderMoved(float value) {
		_SettingsManager.musicVolume = (int)value;
		settingChanged = true;
	}

	private void OnSFXSliderMoved(float value) {
		_SettingsManager.sfxVolume = (int)value;
		settingChanged = true;
	}

	private void OnFullScreenToggled(bool toggle) {
		_SettingsManager.fullScreen = toggle;
		settingChanged = true;
	}

	private void OnLegacyJumpToggled(bool toggle) {
		_SettingsManager.legacyJump = toggle;
		settingChanged = true;
	}

	private void OnControlsPressed() {

	}
	
	private void OnBackPressed() {
		if(settingChanged) {
			UpdateSoundVolume(GetTree().Root);
		}
		PauseButtonPressed();
	}

}
