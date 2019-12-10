using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class hatColourScript : MonoBehaviour
{
    /// <summary>
    /// This just sets the playerCharacters hat colour based on the order in which they joined.
    /// </summary>
    
    public Renderer rend;
    public CinemachineVirtualCamera vc;
    public int playerNumber;
    private float startTime;
    public List<Color> playerColours;
    public bool rainbowcolours;

    private bool rainbowEnabledLastFrame;
    
    
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        rend.materials[0].shader = Shader.Find("Lightweight Render Pipeline/Lit");
        startTime = Time.time;
        playerNumber = vc.gameObject.layer - 9;
        rend.materials[0].SetColor("_BaseColor",playerColours[playerNumber]);
    }

    // Update is called once per frame
    void Update()
    {
        if (rainbowcolours)
        {
            float LifeTime = Time.time *2 - startTime;
            rend.materials[0].SetColor("_BaseColor",RandomColour(LifeTime));
            rainbowEnabledLastFrame = true;
        }
        else if(rainbowEnabledLastFrame)
        {
            rainbowEnabledLastFrame = false;
            rend.materials[0].SetColor("_BaseColor",playerColours[playerNumber]);
        }

    }

    private Color RandomColour(float seed)
    {
        return new Color(Mathf.PerlinNoise(1, seed), Mathf.PerlinNoise(2, seed),
            Mathf.PerlinNoise(3, seed));
    }
    
}
