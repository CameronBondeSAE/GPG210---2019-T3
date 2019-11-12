

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

        public abstract override void LeftStickAxis(Vector2 value);

        public abstract override void RightStickAxis(Vector2 value);

        protected Vector2 CalculateLSAValue(Vector2 value)
        {
            return new Vector2(Mathf.Clamp(value.x,LSA_X_ValueBounds.x,LSA_X_ValueBounds.y)*LSA_X_ValueMultiplier,Mathf.Clamp(value.y,LSA_Y_ValueBounds.x,LSA_Y_ValueBounds.y)*LSA_Y_ValueMultiplier);
        }
    }
}