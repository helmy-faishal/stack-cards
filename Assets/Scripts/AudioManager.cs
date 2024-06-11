using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Script untuk mengatur audio selama permainan
public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource, sfxSource;

    [System.Serializable]
    class Sound
    {
        public string soundName;
        public AudioClip clip;
    }
    [SerializeField] Sound[] musicSounds, sfxSounds;
    string currentMusicName;


    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(string name)
    {
        if (currentMusicName == name) return;

        Sound s = Array.Find(musicSounds, x => x.soundName == name);

        if (s == null)
        {
            Debug.LogError("Music name not found!");
            return;
        }

        currentMusicName = s.soundName;
        musicSource.clip = s.clip;
        musicSource.Play();
    }
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.soundName == name);

        if (s == null)
        {
            Debug.LogError("SFX name not found!");
            return;
        }

        sfxSource.PlayOneShot(s.clip);
    }
}
