using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameUI : MonoBehaviour
{
    public string playerName;
    public GameObject inputField;
    public GameObject textDisplay;

    public void StoreName()
    {
        playerName = inputField.GetComponent<Text>().text;
        textDisplay.GetComponent<Text>().text = "Welcome " + playerName;
        //return playerName;
    }

    public void GiveNameToPlayerInfos(PlayerInfo playerInfo)
    {
        //playerInfo.name = playerName;
    }
}
