using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string LevelToLoad;
    [SerializeField] private SceneFader sceneFader;

    public void Play()
    {
        sceneFader.FadeTo(LevelToLoad);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
