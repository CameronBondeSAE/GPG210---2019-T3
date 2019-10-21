using Sirenix.OdinInspector;
using UnityEngine;

namespace Students.Luca
{
    public class ObjectPoolItem : MonoBehaviour
    {
        public ObjectPool masterPool;
        
        private string objectPoolTag;
        public string ObjectPoolTag { get => objectPoolTag; set => objectPoolTag = value; }

        public void ReleaseObject()
        {
            ReleaseObject(masterPool);
        }

        public void ReleaseObject(ObjectPool toOwnerPool)
        {
            if (toOwnerPool != null)
            {
                toOwnerPool.ReturnObject(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}