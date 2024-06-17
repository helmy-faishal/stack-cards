using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] Slider _slider;

    public Action OnSliderRelease;
    public Action<float> OnSliderChange;
    public float SliderValue
    {
        get { return _slider.value; } 
        set { _slider.value = Mathf.Clamp01(value); } 
    }

    private void Start()
    {
        _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener(SetSliderValueChange);
    }

    public void SetSliderValueChange(float value)
    {
        OnSliderChange?.Invoke(value);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnSliderRelease?.Invoke();
    }
}
