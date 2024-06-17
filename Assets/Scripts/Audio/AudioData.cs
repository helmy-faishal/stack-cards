using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioData
{
    private const string AUDIO_KEY = "AudioData";

    public bool IsMute;
    [Range(0f, 1f)]
    public float Volume;

    public void LoadAudioData()
    {
        string json = PlayerPrefs.GetString(AUDIO_KEY, string.Empty);

        AudioData data = new AudioData();

        if (json != string.Empty)
        {
            data = JsonUtility.FromJson<AudioData>(json);
        }

        this.IsMute = data.IsMute;
        this.Volume = data.Volume;
    }

    public void SaveAudioData(AudioData data)
    {
        if (data == null)
        {
            Debug.LogWarning("Data is null, data not saved!");
            return;
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(AUDIO_KEY, json);
    }

    public void SaveAudioData()
    {
        SaveAudioData(this);
    }
}
