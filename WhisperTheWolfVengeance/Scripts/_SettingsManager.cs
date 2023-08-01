using Godot;
using System;

public partial class _SettingsManager : Node
{
    public static int sfxVolume {get; set;}
    public static int musicVolume {get; set;}
	public static bool fullScreen {get; set;}
	public static bool legacyJump {get; set;}
    
    public override void _Ready()
    {
        sfxVolume = 0;
        musicVolume = 0;
		fullScreen = false;
		legacyJump = false;
    }

	public void SaveSettings() {

	}
}
