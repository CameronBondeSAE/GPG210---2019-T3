
// ===============================
// AUTHOR           : Luca Taennler
// CREATE DATE      : 23.07.2019
// SPECIAL NOTES    : -
// ===============================
// Change History:
//
//==================================

using UnityEngine;

namespace Students.Luca.Scripts.Helper
{
    public static class GameHelper
    {
        /** <summary>Method <c>AddLayerToMask</c> Adds a layer (int) to a layer mask.</summary>
         * <param name="mask">The layer mask to which the layer needs to be added.</param>
         * <param name="layer">The layer to be added to the provided mask.</param>*/
        public static int AddLayerToMask(int mask, int layer)
        {
            var newMask = mask | (1 << layer);

            return newMask;
        }

        /** <summary>Method <c>RemoveLayerFromMask</c> Removes a layer (int) from a layer mask.</summary>
         * <param name="mask">The layer mask to which the layer needs to be added.</param>
         * <param name="layer">The layer to be added to the provided mask.</param>*/
        public static int RemoveLayerFromMask(int mask, int layer)
        {
            var newMask = mask & ~(1 << layer);

            return newMask;
        }

        /** <summary>Method <c>SetLayerRecursively</c> Sets the layer of a gameobject and all its children recursively.</summary>
         * <param name="gameObject">The root gameobject to which the layer needs to be applied to.</param>
         * <param name="layer">The layer to be added to the provided mask.</param>*/
        public static void SetLayerRecursively(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;

            for(int i = 0; i < gameObject.transform.childCount; i++)
            {
                SetLayerRecursively(gameObject.transform.GetChild(i).gameObject, layer);
            }
        }
        
        /** AngleDir: Determines if a target is on the left or right of an object.
         * @return: -1 if target is on the left, 0 if infron/behind, 1 if on the right
         * Copied from: https://forum.unity.com/threads/left-right-test-function.31420/
         */
        public static int AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
            Vector3 perp = Vector3.Cross(fwd, targetDir);
            float dir = Vector3.Dot(perp, up);
		
            if (dir > 0f) {
                return 1;
            } else if (dir < 0f) {
                return -1;
            } else {
                return 0;
            }
        }
    }
}
