using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace Students.Luca.Scripts
{
    public class FlyWheel : Students.Luca.Scripts.Archive.Wheel
    {
        public float floorAngularDrag = 0.05f;
        public float flyAngularDrag = 10;

        public override void RandomUpdateHackFunction()
        {
            //base.RandomUpdateHackFunction();
            master.rb.angularDrag = IsReallyGrounded() ? floorAngularDrag : flyAngularDrag;
        }

        protected override bool IsGrounded()
        {
            return true;
            
            RaycastHit hit;
            Debug.DrawRay(transform.position, currentDistanceToGround*-transform.up/*-Vector3.up*/, Color.blue);
            if (Physics.Raycast(transform.position, -transform.up/*-Vector3.up*/, out hit, 100)) // TODO do raycast from bottom/exhaust
            {
                currentDistanceToGround = hit.distance;
            }
            else
            {
                currentDistanceToGround = float.PositiveInfinity; //zeroForceHeight;
            }
            
            return currentDistanceToGround <= distanceToGround;
        }

        // TREMP HACK DELETE
        protected virtual bool IsReallyGrounded()
        {
            RaycastHit hit;
            Debug.DrawRay(transform.position, currentDistanceToGround*-transform.up/*-Vector3.up*/, Color.blue);
            if (Physics.Raycast(transform.position, -transform.up/*-Vector3.up*/, out hit, 100)) // TODO do raycast from bottom/exhaust
            {
                currentDistanceToGround = hit.distance;
            }
            else
            {
                currentDistanceToGround = float.PositiveInfinity; //zeroForceHeight;
            }
            
            return currentDistanceToGround <= distanceToGround;
        }
    }
}
