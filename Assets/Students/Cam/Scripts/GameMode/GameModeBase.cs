using System;
using System.Collections.Generic;
using UnityEngine;

public class GameModeBase : MonoBehaviour
{
    public event Action<PlayerInfo, GameModeBase> OnScoreChanged;

    public virtual void Activate()
    {
        
    }

    public virtual void StartGame()
    {
        // Opportunity to clean up memory while not playing
        GC.Collect();
    }

    protected virtual void InvokeScoreChanged(PlayerInfo info, GameModeBase gameModeBase)
    {
        OnScoreChanged?.Invoke(info, gameModeBase);
    }
}