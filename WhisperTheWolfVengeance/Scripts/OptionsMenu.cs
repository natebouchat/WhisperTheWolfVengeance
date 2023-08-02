using Godot;
using System;

public partial class OptionsMenu : Control
{
	private AnimationPlayer menuAnim;
	private AudioStreamPlayer audioTest;
	private Label musicLabel;
	private Label sfxLabel;
	private Control[] buttons;
	private bool settingChanged;
	
	public override void _Ready()
	{	
		this.Position = new Vector2(1931, 10);
		menuAnim = GetNode<AnimationPlayer>("AnimationPlayer");
		audioTest = GetNode<AudioStreamPlayer>("SFXRingGet");
		musicLabel = GetNode<Label>("VolumeLabels/MusicVolume");
		sfxLabel = GetNode<Label>("VolumeLabels/SFXVolume");
		menuAnim.Play("SlideButtonsIn");
		settingChanged = false;

		buttons = new Control[6];
		buttons[0] = GetNode<Control>("OptionButtons/GridContainer/MusicSlider");
		buttons[1] = GetNode<Control>("OptionButtons/GridContainer/SFXSlider");
		buttons[2] = GetNode<Control>("OptionButtons/GridContainer/FullScreenCheck");
		buttons[3] = GetNode<Control>("OptionButtons/GridContainer/LegacyJumpCheck");
		buttons[4] = GetNode<Control>("OptionButtons/Controls");
		buttons[5] = GetNode<Control>("OptionButtons/Back");

		((HSlider)buttons[0]).Value = _SettingsManager.musicVolume;
		musicLabel.Text = (_SettingsManager.musicVolume + 25).ToString();
		((HSlider)buttons[1]).Value = _SettingsManager.sfxVolume;
		sfxLabel.Text = (_SettingsManager.sfxVolume + 25).ToString();
		((CheckBox)buttons[2]).ButtonPressed = _SettingsManager.fullScreen;
		((CheckBox)buttons[3]).ButtonPressed = _SettingsManager.legacyJump;

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
			if(((string)aNode.Name).Substr(0, 3).Equals("SFX")) {
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
		//TO DO -> Change Current Musics Volume
		musicLabel.Text = (value + 25).ToString();
		settingChanged = true;
	}

	private void OnSFXSliderMoved(float value) {
		_SettingsManager.sfxVolume = (int)value;
		sfxLabel.Text = (value + 25).ToString();
		settingChanged = true;
	}
	private void OnSFXDragEnded(bool valueChanged) {
		audioTest.VolumeDb = _SettingsManager.sfxVolume;
		if(_SettingsManager.sfxVolume > -25) {
			audioTest.Play();
		}
	}

	private void OnFullScreenToggled(bool toggle) {
		_SettingsManager.SetScreenMode(toggle);
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
			_SettingsManager.SaveSettings();
		}
		PauseButtonPressed();
	}

}
