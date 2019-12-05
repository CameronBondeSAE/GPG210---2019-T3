using System;
using UnityEngine;

namespace Cam
{
    public class GameModeManager : MonoBehaviour
    {
//    public LevelManager LevelManager;
        public GameModeBase GameMode;

        public event Action OnGameHasEnded;
        
        private bool _gameActive = true;
        public bool GameActive
        {
            get => _gameActive;
            set
            {
                _gameActive = value;
                if(!value)
                    _HandleEndOfGame();
            }
        }
        private void _HandleEndOfGame() => OnGameHasEnded?.Invoke();
        
        
        
        public event Action OnAnother;

        public delegate void DoThingDelegate();
        public event DoThingDelegate OnDoThing;


        public event Action<int> OnAnotherWithInt;

        public delegate void DoAnotherWithInt(int stuff);
        public event DoAnotherWithInt OnDoAnotherWithInt;


        // Start is called before the first frame update
        void Start()
        {
            if (GameMode != null)
            {
                GameMode.OnGameModeEnded += HandleGameModeEnded;
            }
            
//        LevelManager.LevelStart();
            GameMode.Activate();
            OnDoThing?.Invoke();
            OnAnotherWithInt?.Invoke(5);
        }

        private void OnDestroy()
        {
            if (GameMode != null)
            {
                GameMode.OnGameModeEnded -= HandleGameModeEnded;
            }
        }

        private void HandleGameModeEnded(GameModeBase gameMode)
        {
            // Info: Here you would check if every gamemode is finished or whatever if you could have multiple gamemodes at once.
            GameActive = false;
        }
    }
}