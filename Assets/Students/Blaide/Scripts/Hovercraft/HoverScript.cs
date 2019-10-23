using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Blaide
{
    public class HoverScript : MonoBehaviour
    {
        public AnimationCurve hoverCurve;
        public List<Transform> HoverPadTransforms;
        public float forceMultiplier;
        public float maxDistance;
        private Rigidbody rB;
    
        // Start is called before the first frame update
        void Start()
        {
            rB = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            foreach (Transform hoverPadTransform in HoverPadTransforms)
            {
                RaycastHit hit;
            
                Physics.Raycast(hoverPadTransform.position, hoverPadTransform.TransformDirection(Vector3.down), out hit,maxDistance);
                    
                if (hit.collider != null && hit.collider.gameObject != this)
                { 
                    rB.AddForceAtPosition(hoverPadTransform.up * forceMultiplier * hoverCurve.Evaluate(hit.distance), hoverPadTransform.position);
                }
            }
        }
    }

}

