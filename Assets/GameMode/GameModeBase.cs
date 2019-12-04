using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameModeBase : SerializedMonoBehaviour
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