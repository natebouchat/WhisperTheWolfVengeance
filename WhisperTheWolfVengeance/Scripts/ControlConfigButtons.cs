using Godot;
using System;
using System.Runtime.CompilerServices;

/*
TODO 
Properly save mouse input to save.cfg
*/


public partial class ControlConfigButtons : Button
{
    public InputEventKey key {get; set;}
	public InputEventMouseButton mouseButton {get; set;}
    public InputEventJoypadButton joyButton {get; set;}
	public enum KeybindMode {Keyboard, Xbox, Playstation, Nintendo};
    private KeybindMode currentMode;
    private bool waitingInput;
    private bool ignoreConfirm;


    public override void _Ready() {
        waitingInput = false;
        ignoreConfirm = false;
		key = null;
		mouseButton = null;
		joyButton = null;
    }

    public override void _Input(InputEvent anEvent) {   
        if(waitingInput == true) {
            this.ToggleMode = true;
            if(anEvent.IsAction("ui_accept")) {
                ignoreConfirm = true;
            }
            if(anEvent is InputEventKey aKey) {
                this.Text = OS.GetKeycodeString(aKey.PhysicalKeycode);
				if(key == null) {
					key = new InputEventKey();
					mouseButton = null;
				}
            	key.PhysicalKeycode = aKey.PhysicalKeycode;
                this.ButtonPressed = false;
                waitingInput = false;
            }
			else if(anEvent is InputEventMouseButton aMouseButton) {
				this.Text = GetMouseBindName((int)aMouseButton.ButtonIndex);
				if(mouseButton == null) {
					mouseButton = new InputEventMouseButton();
					key = null;
				}
                if((int)aMouseButton.ButtonIndex == 0) {
                    ignoreConfirm = true;
                }
				mouseButton.ButtonIndex = aMouseButton.ButtonIndex;
                this.ButtonPressed = false;
                waitingInput = false;
            }
            else if(anEvent is InputEventJoypadButton aJoyButton) {
                this.Text = GetControllerBindName((int)aJoyButton.ButtonIndex, currentMode);
				joyButton.ButtonIndex = aJoyButton.ButtonIndex;
                this.ButtonPressed = false;
                waitingInput = false;
            }
        }
    }

    public override void _Toggled(bool wasPressed) {
        if(wasPressed == true && !ignoreConfirm) {
            this.ToggleMode = false;
            waitingInput = true;
            this.Text = "Waiting";
        }
        else if(ignoreConfirm && this.ToggleMode) {
            ignoreConfirm = false;
            GD.Print("print");
        }

    }

	public void SetButtonText(KeybindMode bind) {
		currentMode = bind;
        if(bind == KeybindMode.Keyboard) {
			if(key != null) {
				this.Text = OS.GetKeycodeString(key.PhysicalKeycode);
			}
			else {
				this.Text = GetMouseBindName((int)mouseButton.ButtonIndex);
			}
		}
        else {
            this.Text = GetControllerBindName((int)joyButton.ButtonIndex, bind);
        }
	}

	private string GetMouseBindName(int buttonCode) {
		if(buttonCode == 1) {
			return "LMB";
		}
		else if(buttonCode == 2) {
			return "RMB";
		}
		else {
			return "Mouse " + buttonCode;
		}
	}

	private string GetControllerBindName(int buttonCode, KeybindMode mode) {
        if(buttonCode >= 0 && buttonCode < 4) {
            if(buttonCode == 0) {
                if(mode == KeybindMode.Xbox) {this.Icon = (CompressedTexture2D)ResourceLoader.Load("res://Assets/UI/Buttons/A.png");}
                if(mode == KeybindMode.Playstation) {this.Icon = (CompressedTexture2D)ResourceLoader.Load("res://Assets/UI/Buttons/X.png");}
                if(mode == KeybindMode.Nintendo) {this.Icon = (CompressedTexture2D)ResourceLoader.Load("res://Assets/UI/Buttons/B.png");}
            }
            else if(buttonCode == 1) {
                if(mode == KeybindMode.Xbox) {this.Icon = (CompressedTexture2D)ResourceLoader.Load("res://Assets/UI/Buttons/B.png");}
                if(mode == KeybindMode.Playstation) {this.Icon = (CompressedTexture2D)ResourceLoader.Load("res://Assets/UI/Buttons/O.png");}
                if(mode == KeybindMode.Nintendo) {this.Icon = (CompressedTexture2D)ResourceLoader.Load("res://Assets/UI/Buttons/A.png");}
            }
            else if(buttonCode == 2) {
                if(mode == KeybindMode.Xbox) {this.Icon = (CompressedTexture2D)ResourceLoader.Load("res://Assets/UI/Buttons/X.png");}
                if(mode == KeybindMode.Playstation) {this.Icon = (CompressedTexture2D)ResourceLoader.Load("res://Assets/UI/Buttons/Triangle.png");}
                if(mode == KeybindMode.Nintendo) {this.Icon = (CompressedTexture2D)ResourceLoader.Load("res://Assets/UI/Buttons/Y.png");}
            }
            else if(buttonCode == 3) {
                if(mode == KeybindMode.Xbox) {this.Icon = (CompressedTexture2D)ResourceLoader.Load("res://Assets/UI/Buttons/Y.png");}
                if(mode == KeybindMode.Playstation) {this.Icon = (CompressedTexture2D)ResourceLoader.Load("res://Assets/UI/Buttons/Square.png");}
                if(mode == KeybindMode.Nintendo) {this.Icon = (CompressedTexture2D)ResourceLoader.Load("res://Assets/UI/Buttons/X.png");}
            }
            return "";
        }
        else {
            this.Icon = null;
            if(buttonCode == 4) {
                return "Select";
            }
            else if(buttonCode == 5) {
                return "Home";
            }
            else if(buttonCode == 6) {
                return "Start";
            }
            else if(buttonCode == 7) {
                return "L3";
            }
            else if(buttonCode == 8) {
                return "R3";
            }
            else if(buttonCode == 9) {
                return "L1";
            }
            else if(buttonCode == 10) {
                return "R1";
            }
            else if(buttonCode == 11) {
                return "DPad Up";        
            }
            else if(buttonCode == 12) {
                return "DPad Down";
            }
            else if(buttonCode == 13) {
                return "DPad Left";
            }
            else if(buttonCode == 14) {
                return "DPad Right";
            }
            else if(buttonCode == 15) {
                return "Select";
            }
            else {
                return "Button " + buttonCode;
            }
        }
    }

}