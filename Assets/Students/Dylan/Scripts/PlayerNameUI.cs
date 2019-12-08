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
        //takes the input and sets that as the player name

        playerName = inputField.GetComponent<Text>().text;   
        textDisplay.GetComponent<Text>().text = "Welcome " + playerName;
        
        //TODO: link to player info
        //GiveNameToPlayerInfos(playerName);
        
    }

    public void GiveNameToPlayerInfos(PlayerInfo playerInfo)
    {
        
        //playerInfo.name = playerName;
    }
}
