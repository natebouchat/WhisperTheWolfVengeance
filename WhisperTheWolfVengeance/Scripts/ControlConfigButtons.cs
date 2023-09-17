using Godot;
using System;

public partial class ControlConfigButtons : Button
{
    bool waitingInput;
    public InputEventKey key {get; set;}
	public InputEventMouseButton mouseButton {get; set;}
    public InputEventJoypadButton joyButton {get; set;}
	public enum KeybindMode {Keyboard, Xbox, Playstation, Nintendo};


    public override void _Ready()
    {
        waitingInput = false;
		key = null;
		mouseButton = null;
		joyButton = null;
    }

    public override void _Input(InputEvent anEvent) {
        if(waitingInput == true) {
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
				mouseButton.ButtonIndex = aMouseButton.ButtonIndex;
                this.ButtonPressed = false;
                waitingInput = false;
            }
            else if(anEvent is InputEventJoypadButton aJoyButton) {
                this.Text = GetControllerBindName((int)aJoyButton.ButtonIndex);
				joyButton.ButtonIndex = aJoyButton.ButtonIndex;
                this.ButtonPressed = false;
                waitingInput = false;
            }
        }
    }

    public override void _Toggled(bool wasPressed) {
        if(wasPressed == true) {
            waitingInput = true;
            this.Text = "Waiting";
        }

    }

	public void SetButtonText(KeybindMode bind) {
		if(bind == KeybindMode.Keyboard) {
			if(key != null) {
				this.Text = OS.GetKeycodeString(key.PhysicalKeycode);
			}
			else {
				this.Text = GetMouseBindName((int)mouseButton.ButtonIndex);
			}
		}
		else if(bind == KeybindMode.Xbox) {

		}
		else if(bind == KeybindMode.Playstation) {
			
		}
		else if(bind == KeybindMode.Nintendo) {
			
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

	private string GetControllerBindName(int buttonCode) {
        if(buttonCode == 0) {
            return "A";
        }
        else if(buttonCode == 1) {
            return "B";
        }
        else if(buttonCode == 2) {
            return "X";
        }
        else if(buttonCode == 3) {
            return "Y";
        }
        else if(buttonCode == 4) {
            return "L1";
        }
        else if(buttonCode == 5) {
            return "L2";
        }
        else if(buttonCode == 6) {
            return "L3";
        }
        else if(buttonCode == 7) {
            return "R1";
        }
        else if(buttonCode == 8) {
            return "R2";
        }
        else if(buttonCode == 9) {
            return "R3";
        }
        else if(buttonCode == 10) {
            return "Select";
        }
        else if(buttonCode == 11) {
            return "Start";        
        }
        else if(buttonCode == 12) {
            return "D-Pad Up";
        }
        else if(buttonCode == 13) {
            return "D-Pad Down";
        }
        else if(buttonCode == 14) {
            return "D-Pad Left";
        }
        else if(buttonCode == 15) {
            return "D-Pad Right";
        }
        return "Button " + buttonCode;
    }

}