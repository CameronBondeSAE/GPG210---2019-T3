using System.Collections;
using System.Collections.Generic;
using Cam;
using UnityEngine;

public class EngineSound : MonoBehaviour
{
    public Rigidbody main;
    public Car car;
    AudioLowPassFilter audioLowPassFilter;
    public float open;
    public float closed;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioLowPassFilter = GetComponent<AudioLowPassFilter>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (car.driving > 0)
        {
            audioLowPassFilter.cutoffFrequency = open;
        }
        else
        {
            audioLowPassFilter.cutoffFrequency = closed;
        }

        audioSource.pitch = 0.85f + main.velocity.magnitude / 10f;
    }
}
