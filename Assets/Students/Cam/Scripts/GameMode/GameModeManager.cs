using System;
using UnityEngine;

namespace Cam
{
    public class GameModeManager : MonoBehaviour
    {
//    public LevelManager LevelManager;
        public GameModeBase GameMode;

        public event Action OnAnother;

        public delegate void DoThingDelegate();
        public event DoThingDelegate OnDoThing;


        public event Action<int> OnAnotherWithInt;

        public delegate void DoAnotherWithInt(int stuff);
        public event DoAnotherWithInt OnDoAnotherWithInt;


        // Start is called before the first frame update
        void Start()
        {
//        LevelManager.LevelStart();
            GameMode.Activate();
            OnDoThing?.Invoke();
            OnAnotherWithInt?.Invoke(5);
        }
    }
}