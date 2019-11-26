using System.Collections;
using System.Collections.Generic;
using Students.Luca.Scripts;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab;
    public Water currentWater;
    public KeyCode spawnKey;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKeyUp(spawnKey) || objectPrefab == null) return;
        GameObject go = Instantiate(objectPrefab, transform.position, Quaternion.identity);

        BuoyantBody bb = go.GetComponent<BuoyantBody>();
        if (bb != null && currentWater != null)
            bb.CurrentWater = currentWater;
    }
}
