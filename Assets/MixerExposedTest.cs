using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerExposedTest : MonoBehaviour
{
    public AudioMixerGroup MusicGroup;
    public AudioMixer AudioMixer;

    public float musicVolume;

    public float MusicVolume
    {
        get => musicVolume;
        set => musicVolume = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AudioMixer.SetFloat("Music", musicVolume);
    }
}
