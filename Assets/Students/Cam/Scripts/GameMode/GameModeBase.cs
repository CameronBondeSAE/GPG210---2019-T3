using System;
using System.Collections.Generic;
using UnityEngine;

public class GameModeBase : MonoBehaviour
{
    public virtual void Activate()
    {
        
    }

    public virtual void StartGame()
    {
        // Opportunity to clean up memory while not playing
        GC.Collect();
    }
}