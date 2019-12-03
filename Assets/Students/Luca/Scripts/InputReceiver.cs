

using UnityEngine;
using UnityEngine.UIElements;

namespace Students.Luca.Scripts
{
    public abstract  class InputReceiver : Possessable
    {
        public bool useLSA = false;
        public Vector2 LSA_X_ValueBounds = new Vector2(-1,1); // min,max for Left Stick Axis X
        public float LSA_X_ValueMultiplier = 1;
        public Vector2 LSA_Y_ValueBounds = new Vector2(-1,1); // min,max for Left Stick Axis Y
        public float LSA_Y_ValueMultiplier = 1;

        
        public bool useRSA = false;
        public Vector2 RSA_X_ValueBounds = new Vector2(-1,1); // min,max for Left Stick Axis X
        public float RSA_X_ValueMultiplier = 1;
        public Vector2 RSA_Y_ValueBounds = new Vector2(-1,1); // min,max for Left Stick Axis Y
        public float RSA_Y_ValueMultiplier = 1;

        public bool useLT = false;
        public float LT_ValueMultiplier = 1;
        public bool useRT = false;
        public float RT_ValueMultiplier = 1;
        
        public abstract override void LeftStickAxis(Vector2 value);

        public abstract override void RightStickAxis(Vector2 value);
        
        public abstract override void LeftTrigger(float value);
        
        public abstract override void RightTrigger(float value);

        protected Vector2 CalculateLSAValue(Vector2 value)
        {
            return new Vector2(Mathf.Clamp(value.x,LSA_X_ValueBounds.x,LSA_X_ValueBounds.y)*LSA_X_ValueMultiplier,Mathf.Clamp(value.y,LSA_Y_ValueBounds.x,LSA_Y_ValueBounds.y)*LSA_Y_ValueMultiplier);
        }
        protected Vector2 CalculateRSAValue(Vector2 value)
        {
            return new Vector2(Mathf.Clamp(value.x,RSA_X_ValueBounds.x,RSA_X_ValueBounds.y)*RSA_X_ValueMultiplier,Mathf.Clamp(value.y,RSA_Y_ValueBounds.x,RSA_Y_ValueBounds.y)*RSA_Y_ValueMultiplier);
        }
        
        protected float CalculateLTValue(float value)
        {
            return value * LT_ValueMultiplier;
        }
        
        protected float CalculateRTValue(float value)
        {
            return value * RT_ValueMultiplier;
        }

        public abstract float GetCurrentForceSecondValue(); // Hacky
    }
}