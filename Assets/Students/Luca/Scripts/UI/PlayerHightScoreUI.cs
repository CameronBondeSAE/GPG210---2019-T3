using System;
using System.Collections;
using System.Collections.Generic;
using Students.Luca.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHightScoreUI : MonoBehaviour
{
    public GameObject scoreEntryPrefab;
    public GameObject highScoreTitleBox;
    
    private void Awake()
    {
        CreateHighScoreList();
    }

    public void CreateHighScoreList()
    {
        highScoreTitleBox.SetActive(false);
        if(scoreEntryPrefab == null) return;
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
        }

        if (entryCount > 0)
        {
            highScoreTitleBox.SetActive(true);
        }
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
