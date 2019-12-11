
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Students.Luca.Scripts.GameSave;
using Students.Luca.Scripts.Helper;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cam
{
    public class GameModeManager : MonoBehaviour
    {
        public static string highscorePlayerPrefsKey = "HighScoreList"; // Super hacky to store this here. Just for simplicity
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

        // HACK; Temporary - Get random names for player score data
        public RandomNameGenerator randomNameGenerator;

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
            if(!GameActive)
                return;
            
            // Info: Here you would check if every gamemode is finished or whatever if you could have multiple gamemodes at once.
            GameActive = false;

            SaveScore();
        }

        // Store Scores; SUPER HACKY
        private void SaveScore()
        {
            var pm = FindObjectOfType<PlayerManager>();

            var hsl = new HighScoreList(highscorePlayerPrefsKey);
            if(hsl.Equals(default)) hsl = new HighScoreList();

            var existingNames = (hsl.scoreEntries?.Select(entry => entry.name).ToList()) ?? new List<string>(); // Hacky; Ensure uniqueness of names
            pm?.playerInfos?.ForEach(info =>
            {
                string newName;
                do
                {
                    newName = randomNameGenerator?.GetRandomName() ?? "Hans"+Random.Range(0,10000);
                }
                while (existingNames.Contains(newName));
                existingNames.Add(newName);
                hsl.scoreEntries.Add(new HighScoreEntry(newName, info.score));
            });
            hsl.Save(highscorePlayerPrefsKey);
            
            Debug.Log("Score saved!");
        }
    }
}