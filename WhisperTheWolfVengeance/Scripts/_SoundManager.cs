using Godot;
using System;

public class _SoundManager : Node
{
    static Node allMusic;
    static Node allSFX;

    public override void _Ready()
    {
        allMusic = GetNode<Node>("Music");
        allSFX = GetNode<Node>("SFX");
    }

    public static void PlayMusic(String name) {
        allMusic.GetNode<AudioStreamPlayer>(name).Play();
    }

    public static void PlaySFX(String name) {
        allSFX.GetNode<AudioStreamPlayer>(name).Play();
    }
}
