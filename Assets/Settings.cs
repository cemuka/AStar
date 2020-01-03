using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISettings
{
    float SetVolume{get; set;} 
}

public class Settings : ISettings
{
    public Settings()
    {
        SetVolume = 1f;
    }

    public float SetVolume {get; set;}
    
}

public class WindowSettings
{
    [Inject]
    private Settings settings;
    public float GetVolume => settings.SetVolume;
}

public class MusicPlayer
{
    [Inject]
    private Settings settings;
    public float GetVolume => settings.SetVolume;
}
