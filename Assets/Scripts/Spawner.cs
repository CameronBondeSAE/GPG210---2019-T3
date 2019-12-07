using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

public class Spawner : MonoBehaviour
{
    public LayerMask layerMask;
    public BoxCollider boundingBox;
    public GameObject[] prefabs;
    public int number;
    public float offsetFromGround = 5f;
    public float maxRandomRotation = 360;
    public enum SpawnCondition
    {
        SpawnOnLand,
        SpawnOnWater,
        SpawnUnderWater
    }
    public List<SpawnCondition> spawnConditions;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 randomPointInBounds = RandomPointInBounds(boundingBox.bounds);

        for (int i = 0; i < number; i++)
        {
            Vector3 pointInBounds = RandomPointInBounds(boundingBox.bounds);

            Ray ray = new Ray(new Vector3(pointInBounds.x, boundingBox.bounds.max.y, pointInBounds.z), Vector3.down);
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, boundingBox.bounds.size.y,~layerMask, ( !spawnConditions.Contains(SpawnCondition.SpawnUnderWater) ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore ));
            pointInBounds.y = hitInfo.point.y + offsetFromGround; // DONE: Should read normal or do corner checks 
            
            /*Instantiate(prefabs[Random.Range(0, prefabs.Length)], pointInBounds,
                Quaternion.Euler(0, Random.Range(0, maxRandomRotation), 0));*/

            if (!(!spawnConditions.Contains(SpawnCondition.SpawnOnLand) &&
                  hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("terrain"))
                && !(!spawnConditions.Contains(SpawnCondition.SpawnOnWater) &&
                     (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Water"))))
            {
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                Instantiate(prefabs[Random.Range(0, prefabs.Length)], pointInBounds,rot);
            }
            else
            {
                i--;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static Vector3 RandomPointInBounds(Bounds bounds) 
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}


