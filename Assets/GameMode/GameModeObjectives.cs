using Sirenix.OdinInspector;
using UnityEngine;

namespace GameMode
{
 /// <summary>
 ///  UNUSED
 /// 
 /// </summary>
 [CreateAssetMenu(menuName = "Objectives")]
 public class GameModeObjectives : ScriptableObject
 {
  
      [ToggleGroup("useObjTimeLimit", ToggleGroupTitle = "Time Limit"), SerializeField, ShowInInspector]
      public bool useObjTimeLimit;

      [ButtonGroup("useObjTimeLimit/TimeLimitButtons", -1), Button(ButtonSizes.Small), LabelText("5 min")]
      private void SetTimeLimit5M() => objTimeLimit = 300;

      [ButtonGroup("useObjTimeLimit/TimeLimitButtons"), LabelText("10 min")]
      private void SetTimeLimit10M() => objTimeLimit = 600;

      [ButtonGroup("useObjTimeLimit/TimeLimitButtons"), LabelText("30 min")]
      private void SetTimeLimit30M() => objTimeLimit = 1800;

      [ButtonGroup("useObjTimeLimit/TimeLimitButtons"), LabelText("45 min")]
      private void SetTimeLimit45M() => objTimeLimit = 2700;

      [ButtonGroup("useObjTimeLimit/TimeLimitButtons"), LabelText("1 hour")]
      private void SetTimeLimit1H() => objTimeLimit = 3600;

      [ToggleGroup("useObjTimeLimit"), SerializeField, ShowInInspector, MinValue(0), SuffixLabel("Seconds"),
       LabelText("Time Limit")]
      public float objTimeLimit; // In Seconds

      [ToggleGroup("useObjTimeLimit"), SerializeField, ShowInInspector, MinValue(1),
       Tooltip("The frequency how often to check if the timer reached 0."), LabelText("Time Check Interval")]
      public float objCheckerInterval = 1;


      [ToggleGroup("useObjScoreLimit", ToggleGroupTitle = "Score Limit"), SerializeField, ShowInInspector]
      public bool useObjScoreLimit;

      [ToggleGroup("useObjScoreLimit"), SerializeField, ShowInInspector, MinValue(0), LabelText("Score Cap")]
      public int objScoreLimit;

      [ToggleGroup("useObjCheckpointLimit", ToggleGroupTitle = "Checkpoint Limit"), SerializeField,
       ShowInInspector]
      public bool useObjCheckpointLimit;

      [ToggleGroup("useObjCheckpointLimit"), SerializeField, ShowInInspector, MinValue(0),
       LabelText("# Checkpoint Target")]
      public int objCheckpointLimit;

      public bool CheckObjectivesReached(float startTime = 0f, int points = 0, int reachedCheckpoints = 0)
      {
          var timeLeft = (objTimeLimit - Time.time + startTime);
          if (useObjTimeLimit && timeLeft <= 0)
           return true;
          
          if (useObjScoreLimit && points >= objScoreLimit)
           return true;
          
          if (useObjCheckpointLimit && reachedCheckpoints >= objCheckpointLimit)
           return true;
          
          return false;
      }
 }
}
