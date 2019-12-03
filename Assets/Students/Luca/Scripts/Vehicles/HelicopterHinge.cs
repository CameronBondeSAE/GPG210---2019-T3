using UnityEngine;

namespace Students.Luca.Scripts
{
    public class HelicopterHinge : InputReceiver
    {
        public ConstantForce constantForce;

        public Vector3 maxRelativeForce = Vector3.zero;
        
        private Vector3 currentRelativeForce = Vector3.zero;
        
        
        // Start is called before the first frame update
        void Start()
        {
            if (constantForce == null)
                constantForce = GetComponent<ConstantForce>();
            
            
        }

        // Update is called once per frame
        void Update()
        {
            if(constantForce == null)
                return;
            constantForce.relativeForce = currentRelativeForce;
        }

        public override void LeftStickAxis(Vector2 value)
        {
            if (!useLSA)
                return;

            value = CalculateLSAValue(value);

            if (!Mathf.Approximately(LSA_X_ValueMultiplier,0))
            {
                currentRelativeForce = maxRelativeForce * value.x;
            }
            if (!Mathf.Approximately(LSA_Y_ValueMultiplier,0))
            {
                currentRelativeForce = maxRelativeForce * value.y;
            }
        }

        public override void RightStickAxis(Vector2 value)
        {
            if (!useRSA)
                return;

            value = CalculateRSAValue(value);

            if (!Mathf.Approximately(RSA_X_ValueMultiplier,0))
            {
                currentRelativeForce = maxRelativeForce * value.x;
            }
            if (!Mathf.Approximately(RSA_Y_ValueMultiplier,0))
            {
                currentRelativeForce = maxRelativeForce * value.y;
            }
        }

        public override void LeftTrigger(float value)
        {
            
        }

        public override void RightTrigger(float value)
        {
            
        }

        public override void Stop()
        {
            currentRelativeForce = Vector3.zero;
        }

        public override float GetCurrentForceSecondValue()
        {
            return currentRelativeForce.magnitude;
        }
    }
}
