using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayerController : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        MusicPlayerController[] musicPlayers = FindObjectsOfType<MusicPlayerController>();
        if (musicPlayers.Length > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);

        audioSource = GetComponent<AudioSource>();

        if (FindObjectOfType<GameSession>().isMusicMuted)
            audioSource.mute = true;
    }

    public void SetMusicMute(bool option)
    {
        audioSource.mute = option;
    }
}
