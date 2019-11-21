using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TreeEditor;
using UnityEngine;

public class hatColourScript : MonoBehaviour
{
    public Renderer rend;
    public CinemachineVirtualCamera vc;
    public int playerNumber;
    private float startTime;
    public List<Color> playerColours;
    public bool rainbowcolours;

    private bool rainbowEnabledLastFrame;
    // Start is called before the first frame update
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
