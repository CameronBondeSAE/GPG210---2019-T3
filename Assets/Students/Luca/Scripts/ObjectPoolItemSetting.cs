using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ObjectPoolItemSetting
{
    public string tag;
    public GameObject prefab;
    public int maxPooledObjects = 100;
    public int minPooledObjects = 0;
}
