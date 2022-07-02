using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;
    public AudioClip menuTheme;

    bool isPlaying;
    void Start()
    {
        AudioManager.instance.PlayMusic(menuTheme, 2);
        isPlaying = false;
    }
    void Update()
    {
        if ((SceneManager.GetActiveScene() == SceneManager.GetSceneByName("FPSGame")) && !isPlaying)
        {
            AudioManager.instance.PlayMusic(mainTheme, 2);
            isPlaying = true;
        }
    }
}
