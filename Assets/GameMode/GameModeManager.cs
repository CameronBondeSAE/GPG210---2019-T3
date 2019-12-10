
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Students.Luca.Scripts.Helper;
using UnityEngine;
using Random = UnityEngine.Random;

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

            // 
            Dictionary<string, int> newHighscoreList = new Dictionary<string, int>();
            
            // Get stored values
            //var storedScores = new List<int>();
            for (var i = 1; i <= 5; i++)
            {
                var val = PlayerPrefs.GetInt("ScorePoints" + i, 0);
                var name = PlayerPrefs.GetString("ScoreName" + i, randomNameGenerator?.GetRandomName() ?? "Hans");
                
                if (val > 0)
                    newHighscoreList.Add(name,val);//storedScores.Add(val);
                
                
            }
            
            // Get scores from current game & sort
            //var pInfoSorted = new List<PlayerInfo>(pm.playerInfos){};
            //pInfoSorted.Sort((p1, p2) => (p1.score > p2.score ? 1 : (p1.score < p2.score ? -1 : 0)));
            pm?.playerInfos?.ForEach(info =>
            {
                /*if (storedScores.Count > 0)
                {
                    for (var i = 0; i < storedScores.Count; i++)
                    {
                        if (info.score >= storedScores[i])
                        {
                            newHighscoreList.Add(randomNameGenerator?.GetRandomName(), info.score);
                        }
                        else
                        {
                            var storedName = PlayerPrefs.GetString("ScoreName" + (i + 1), randomNameGenerator?.GetRandomName());
                            newHighscoreList.Add(storedName, storedScores[i]);
                        }
                    }
                    return;
                }*/
                string newName;
                do
                {
                    newName = randomNameGenerator?.GetRandomName() ?? "Hans"+Random.Range(0,10000);
                }
                while (newHighscoreList.ContainsKey(newName)) ;
                
                newHighscoreList.Add(newName, info.score);
            });

            var sortedHighscoreKeyList = newHighscoreList.Keys.ToList();
            sortedHighscoreKeyList.Sort((x,y) => (newHighscoreList[x] > newHighscoreList[y] ? 1 : (newHighscoreList[x] < newHighscoreList[y] ? -1 : 0)));

            
            
            // Save New HighCores
            var rankCounter = 1;
            sortedHighscoreKeyList.ForEach(key =>
            {
                if (newHighscoreList[key] <= 0 || rankCounter > 5) return;
                PlayerPrefs.SetString("ScoreName"+rankCounter, key);
                PlayerPrefs.SetInt("ScorePoints"+rankCounter, newHighscoreList[key]);
                rankCounter++;

            });
            PlayerPrefs.Save();
            Debug.Log("Score saved!");
        }
    }
}