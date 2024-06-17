using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    [SerializeField] Button _closeSetting;
    [SerializeField] Button _audioToggleButton;
    [SerializeField] GameObject _audioIconMute;
    [SerializeField] AudioSlider _audioSlider;
    [SerializeField] TMP_Text _audioText;

    AudioData _audioData;

    private void Awake()
    {
        _audioData = new AudioData();
        _audioData.LoadAudioData();
    }

    // Start is called before the first frame update
    void Start()
    {
        _closeSetting.onClick.AddListener(CloseSetting);

        _audioToggleButton.onClick.AddListener(AudioToggle);
        _audioIconMute.SetActive(_audioData.IsMute);

        _audioSlider.OnSliderChange += SetAudioText;
        _audioSlider.SliderValue = _audioData.Volume;
        SetAudioText(_audioData.Volume);

        _audioSlider.OnSliderRelease += SetAudioReleaseValue;
    }

    void CloseSetting()
    {
        gameObject.SetActive(false);
    }

    void AudioToggle()
    {
        _audioData.IsMute = !_audioData.IsMute;
        _audioIconMute.SetActive(_audioData.IsMute);
        AudioManager.Instance.SetAudioMuted(_audioData.IsMute);
        _audioData.SaveAudioData();
    }

    void SetAudioText(float value)
    {
        _audioText.text = $"{(value * 100):0} %";
        AudioManager.Instance.SetAudioVolume(value);
    }

    void SetAudioReleaseValue()
    {
        _audioData.Volume = _audioSlider.SliderValue;
        AudioManager.Instance.SetAudioVolume(_audioData.Volume);
        _audioData.SaveAudioData();
    }
}
