using System;
using UnityEngine;

namespace Students.Luca.Scripts.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public GameMaster gameMaster;
        private void Start()
        {
            gameMaster = FindObjectOfType<GameMaster>();
        }

        public void OnBtnClickStartGame()
        {
            gameMaster?.LoadScreen(GameMaster.ScreenId.Level1);
        }
    }
}