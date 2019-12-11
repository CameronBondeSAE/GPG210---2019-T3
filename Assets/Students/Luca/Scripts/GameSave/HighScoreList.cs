using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Students.Luca.Scripts.GameSave
{
    [Serializable]
    public struct HighScoreList
    {
        //public Dictionary<string, int> scores;
        [SerializeField]
        public List<HighScoreEntry> scoreEntries;

        /*public Dictionary<string, int> GetSortedHighScoreDict()
        {
            
            var sortedHighscoreKeyList = scores.Keys.ToList();
            sortedHighscoreKeyList.Sort(Compare);

            var sortedDict = new Dictionary<string, int>();
            
            // Save New HighCores
            var rankCounter = 1;
            foreach (var key in sortedHighscoreKeyList)
            {
                sortedDict.Add(key, scores[key]);
            }

            return sortedDict;
        }*/

        public HighScoreList(string key)
        {
            var hsl = LoadFromPlayerPrefs(key);
            if (hsl.Equals(default) || hsl.scoreEntries == null)
            {
                scoreEntries = new List<HighScoreEntry>();
            }
            else
            {
                scoreEntries = hsl.scoreEntries;
            }
            
        }
        
        

        public IEnumerable<HighScoreEntry> GetSortedHighscoreList()
        {
            
            scoreEntries.Sort();
            return scoreEntries;
        }
        /*private int Compare(string entry1, string entry2)
        {
            return (scores[entry1] > scores[entry2] ? 1 : (scores[entry1] < scores[entry2] ? -1 : 0));
        }*/

        public void Save(string key)
        {
            var jsonScoreObj = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(key, jsonScoreObj);
            PlayerPrefs.Save();
        }

        public static HighScoreList LoadFromPlayerPrefs(string key)
        {
            if (key.Equals(""))
                return default;
            
            var json = PlayerPrefs.GetString(key);
            HighScoreList hsl = default;
            try
            {
                hsl = JsonUtility.FromJson<HighScoreList>(json);
            }
            catch
            {
                Debug.Log("Couldn't load Highscore List with key "+key+"");
            }

            return hsl;

        }
    }
}
