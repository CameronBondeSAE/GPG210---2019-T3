using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineFilter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<AudioLowPassFilter>().cutoffFrequency = Random.Range(0, 22000);
    }
}
