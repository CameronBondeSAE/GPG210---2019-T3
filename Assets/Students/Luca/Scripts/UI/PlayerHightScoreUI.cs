using System;
using System.Collections;
using System.Collections.Generic;
using Cam;
using Sirenix.Utilities;
using Students.Luca.Scripts.GameSave;
using Students.Luca.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHightScoreUI : MonoBehaviour
{
    public GameObject scoreEntryPrefab;
    public GameObject highScoreTitleBox;
    public int maxEntries = 5;
    
    private void Awake()
    {
        CreateHighScoreList();
    }

    public void CreateHighScoreList()
    {
        highScoreTitleBox.SetActive(false);
        if(scoreEntryPrefab == null) return;
        DeleteChildren();
        /*
        DeleteChildren();
        var entryCount = 0;
        for (var i = 1; i <= 5; i++)
        {
            var sName = PlayerPrefs.GetString("ScoreName" + i, null);
            var sPoints = PlayerPrefs.GetInt("ScorePoints" + i, 0);
            if(sName == null || sPoints <= 0)
                break;
            entryCount++;
            var entryObj = Instantiate(scoreEntryPrefab, transform);
            var entry = entryObj?.GetComponentInChildren<PlayerHighScoreEntryUI>();
            if (entry == null) continue;
            entry.txtRank.text = i+".";
            entry.txtName.text = sName;
            entry.txtScore.text = sPoints.ToString();
        }*/

        var hsl = HighScoreList.LoadFromPlayerPrefs(GameModeManager.highscorePlayerPrefsKey);
        if (hsl.Equals(default) || hsl.scoreEntries?.Count <= 0) return;
        var entryCount = 0;
        foreach (var hsEntRy in hsl.GetSortedHighscoreList())
        {
            if(entryCount >= maxEntries)
                break;
            
            var entryObj = Instantiate(scoreEntryPrefab, transform);
            var entry = entryObj?.GetComponentInChildren<PlayerHighScoreEntryUI>();
            if (entry == null) continue;
            entry.txtRank.text = (entryCount+1)+".";
            entry.txtName.text = hsEntRy.name;
            entry.txtScore.text = hsEntRy.points.ToString();
            entryCount++;
        }
        
        highScoreTitleBox.SetActive(true);

    }

    private void DeleteChildren()
    {
        var childCount = transform.childCount;
        if (childCount <= 0) return;
        for (var i = 0; i < childCount; i++)
        {
            Destroy(transform.GetChild(i));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
