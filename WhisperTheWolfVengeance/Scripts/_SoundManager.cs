using Godot;
using System;

public partial class _SoundManager : Node
{
    public static int sfxVolume {get; set;}
    public static int musicVolume {get; set;}
    
    public override void _Ready()
    {
        sfxVolume = 0;
        musicVolume = 0;
    }


}
