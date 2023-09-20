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

        SaveKeyBinds();
    }


    private static void SaveKeyBinds() {
        string[] allActions = new string[InputMap.GetActions().Count];
        for(int i = 0; i < InputMap.GetActions().Count; i++) {
            allActions[i] = InputMap.GetActions()[i];
        }
        Array.Sort(allActions);
        
        for(int i = 0; i < allActions.Length; i++) {
            string allInputs = "";
            var actions = InputMap.ActionGetEvents(allActions[i]);
            foreach(var a in actions) {
                if (a is InputEventKey key) {
                    allInputs += "Key:" + (int)key.PhysicalKeycode + ", ";
                }
                else if (a is InputEventJoypadButton aButton) {
                    allInputs += "JoypadButton:" + (int)aButton.ButtonIndex + ", ";
                }
                else if (a is InputEventJoypadMotion joystick) {
                    allInputs += "JoypadMotion:" + (int)joystick.Axis;
                    if (allActions[i].Equals("ui_up") || allActions[i].Equals("ui_left")) {
                        allInputs += "/-, ";
                    }
                    else {
                        allInputs += "/+, ";
                    }
                }
            }
            if(allInputs.Length != 0) {
                file.SetValue("Controls", allActions[i], allInputs);
            }
        }
    }

    private static void LoadGame() {
       	file = new ConfigFile();

        Error err = file.Load("user://save.cfg");
        if(err == Error.Ok) {
            musicVolume = (int)file.GetValue("Options", "Music Volume");
            sfxVolume = (int)file.GetValue("Options", "SFX Volume");
            SetScreenMode((bool)file.GetValue("Options", "Full Screen"));
            legacyJump = (bool)file.GetValue("Options", "Legacy Jump");

            ParseKeybinds();
        }
        else {
            GD.Print("Load failed. Creating default save");
            SetDefaultSettings();
            SaveSettings();
        }  
    }

    public static void SetScreenMode(bool setFullScreen) {
        fullScreen = setFullScreen;
        if(setFullScreen) {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
        else {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
    }

    private static void ParseKeybinds() {
        string[] allActions = file.GetSectionKeys("Controls");

        for(int i = 0; i < allActions.Length; i++) {
            InputMap.ActionEraseEvents(allActions[i]);
            string[] allInputs = ((string)file.GetValue("Controls", allActions[i])).Split(", ");
            for(int j = 0; j < allInputs.Length; j++) {
                string[] inputAndKey = allInputs[j].Split(':');

                if(inputAndKey[0].Equals("Key")) {
                    InputEventKey aKey = new InputEventKey();
                    aKey.PhysicalKeycode = (Key)Convert.ToInt32(inputAndKey[1]);
                    InputMap.ActionAddEvent(allActions[i], aKey);
                }
                else if(inputAndKey[0].Equals("JoypadButton")) {
                    InputEventJoypadButton newButton = new InputEventJoypadButton();
                    newButton.ButtonIndex = (JoyButton)Convert.ToInt32(inputAndKey[1]);
                    InputMap.ActionAddEvent(allActions[i], newButton);
                }
                else if(inputAndKey[0].Equals("JoypadMotion")) { 
                    string[] axisAndDirection = inputAndKey[1].Split("/");
                    InputEventJoypadMotion newJoyMotion = new InputEventJoypadMotion();
                    newJoyMotion.Axis = (JoyAxis)Convert.ToInt32(axisAndDirection[0]);
                    if(axisAndDirection[1].Equals("-")) {
                        newJoyMotion.AxisValue = -1.0f;
                    }
                    else {
                        newJoyMotion.AxisValue = 1.0f;
                    }
                    InputMap.ActionAddEvent(allActions[i], newJoyMotion);
                }
            }
        }
    }

    private static void SetDefaultSettings() {
        sfxVolume = 0;
        musicVolume = 0;        
		fullScreen = false;
		legacyJump = false;

        InputMap.LoadFromProjectSettings();
    }
}
