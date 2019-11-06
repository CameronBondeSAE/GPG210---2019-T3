using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MusicVolume : MonoBehaviour
{
    public Slider slider;
    public MixerExposedTest MixerExposedTest;
    
    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(Change);
    }

    private void Change(float value)
    {
        MixerExposedTest.MusicVolume = value;
    }
}
