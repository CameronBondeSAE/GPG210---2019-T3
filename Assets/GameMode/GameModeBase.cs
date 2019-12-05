using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameModeBase : SerializedMonoBehaviour
{
    public event Action<PlayerInfo, GameModeBase> OnScoreChanged;

    public event Action<GameModeBase> OnGameModeEnded;

    public float GameStartTime { get; protected set; }

    private bool _gameModeModeActive;
    public bool GameModeActive
    {
        get => _gameModeModeActive;
        protected set
        {
            _gameModeModeActive = value;
            if(!_gameModeModeActive) _HandleEndOfGameMode();
        }
    }
    protected virtual void NotifyGameModeHasEnded(GameModeBase gameModeBase) => OnGameModeEnded?.Invoke(gameModeBase);
    
    public virtual void Activate()
    {
        GameStartTime = Time.time;
    }

    public virtual void StartGame()
    {
        // Opportunity to clean up memory while not playing
        GC.Collect();
    }
    
    protected virtual void _HandleEndOfGameMode()
    {
        NotifyGameModeHasEnded(this);
    }

    protected virtual void InvokeScoreChanged(PlayerInfo info, GameModeBase gameModeBase)
    {
        OnScoreChanged?.Invoke(info, gameModeBase);
    }

    public virtual bool CheckObjectives() => false;




}