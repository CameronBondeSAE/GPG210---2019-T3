using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public BoxCollider boundingBox;

    public GameObject[] prefabs;
    public int number;
    public float offsetFromGround = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 randomPointInBounds = RandomPointInBounds(boundingBox.bounds);

        for (int i = 0; i < number; i++)
        {
            Vector3 pointInBounds = RandomPointInBounds(boundingBox.bounds);

            Ray ray = new Ray(new Vector3(pointInBounds.x, boundingBox.bounds.max.y, pointInBounds.z), Vector3.down);
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, boundingBox.bounds.size.y);
            pointInBounds.y = hitInfo.point.y + offsetFromGround; // TODO: Should read normal or do corner checks
           
            Instantiate(prefabs[Random.Range(0, prefabs.Length)], pointInBounds,
                Quaternion.Euler(0, Random.Range(0, 360), 0));
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


