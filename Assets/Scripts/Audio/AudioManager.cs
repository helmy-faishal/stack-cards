using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Script untuk mengatur audio selama permainan
public class AudioManager : MonoBehaviour
{
    public AudioSource _musicSource, _sfxSource;

    [System.Serializable]
    class Sound
    {
        public string soundName;
        public AudioClip clip;
    }
    [SerializeField] Sound[] _musicSounds, _sfxSounds;
    string _currentMusicName;

    AudioData _audioData;

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _audioData = new AudioData();
        _audioData.LoadAudioData();
        SetAudioMuted(_audioData.IsMute);
        SetAudioVolume(_audioData.Volume);
    }

    public void PlayMusic(string name)
    {
        if (_currentMusicName == name) return;

        Sound s = Array.Find(_musicSounds, x => x.soundName == name);

        if (s == null)
        {
            Debug.LogError("Music name not found!");
            return;
        }

        _currentMusicName = s.soundName;
        _musicSource.clip = s.clip;
        _musicSource.Play();
    }
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(_sfxSounds, x => x.soundName == name);

        if (s == null)
        {
            Debug.LogError("SFX name not found!");
            return;
        }

        _sfxSource.PlayOneShot(s.clip);
    }

    public void SetAudioVolume(float volume)
    {
        _musicSource.volume = volume;
        _sfxSource.volume = volume;
    }

    public void SetAudioMuted(bool mute)
    {
        _musicSource.mute = mute;
        _sfxSource.mute = mute;
    }
}
