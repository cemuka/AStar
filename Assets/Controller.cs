using Minic.DI;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private void Start()
    {
        var window   = new WindowSettings();
        var player   = new MusicPlayer();

        var injector = new Injector();

        var settings = new Settings(){
            SetVolume = .8f
        };

        injector.AddBinding<Settings>().ToValue(settings);
        injector.InjectInto(window);
        injector.InjectInto(player);

        Debug.Log(window.GetVolume);
        Debug.Log(player.GetVolume);
    } 
}