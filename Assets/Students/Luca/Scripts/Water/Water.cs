using UnityEngine;

namespace Students.Luca.Scripts
{
    
    public class Water : MonoBehaviour
    {
        public float density = 1025;

        public virtual float GetSurfaceLevelAtPos(float x, float z)
        {
            return transform.position.y;
        }

    }
}