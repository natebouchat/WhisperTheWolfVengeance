using Godot;
using System;

public partial class _SettingsManager : Node
{
    public static int sfxVolume {get; set;}
    public static int musicVolume {get; set;}
    public static bool fullScreen {get; set;}
	public static bool legacyJump {get; set;}

    static private ConfigFile file {get; set;}
    
    public override void _Ready()
    {
        sfxVolume = 0;
        musicVolume = 0;        
		fullScreen = false;
		legacyJump = false;

        LoadGame();
    }

	public static void SaveSettings() {
        SaveOptions();

        file.Save("user://save.cfg");
	}

    private static void SaveOptions() {
        file.SetValue("Options", "Music Volume", musicVolume);
        file.SetValue("Options", "SFX Volume", sfxVolume);
        file.SetValue("Options", "Full Screen", fullScreen);
        file.SetValue("Options", "Legacy Jump", legacyJump);
    }

    private void LoadGame() {
       	file = new ConfigFile();

        Error err = file.Load("user://save.cfg");
        if(err == Error.Ok) {
            musicVolume = (int)(file.GetValue("Options", "Music Volume"));
            sfxVolume = (int)(file.GetValue("Options", "SFX Volume"));
            SetScreenMode((bool)(file.GetValue("Options", "Full Screen")));
            legacyJump = (bool)(file.GetValue("Options", "Legacy Jump"));
        }
        else {
            GD.Print("Load failed. Creating default save");
            SaveSettings();
        }  
    }

    public static void SetScreenMode(bool setFullScreen) {
        fullScreen = setFullScreen;
        if(setFullScreen) {
            DisplayServer.WindowSetMode((DisplayServer.WindowMode)3);
        }
        else {
            DisplayServer.WindowSetMode((DisplayServer.WindowMode)0);
        }
    }
}
