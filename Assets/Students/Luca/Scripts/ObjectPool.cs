using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Students.Luca;
using UnityEngine;

public class ObjectPool : SerializedMonoBehaviour
{
    
    public Dictionary<string, ObjectPoolItemSetting> objectSettings;
    private Dictionary<string, Stack<ObjectPoolItem>> availableObjects;
    
    // Start is called before the first frame update
    void Start()
    {
        if (objectSettings == null)
        {
            objectSettings = new Dictionary<string, ObjectPoolItemSetting>();
        }
        
        availableObjects = new Dictionary<string, Stack<ObjectPoolItem>>();
        StartCoroutine(InitPool());
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Info: Added this code in a IEnumerator (To be called in a coroutine) so Waits can be added in the future in case needed for performance @ initial spawning
    IEnumerator InitPool()
    {
        foreach (var itemSetting in objectSettings.Values)
        {
            if(!availableObjects.ContainsKey(itemSetting.tag))
                availableObjects.Add(itemSetting.tag, new Stack<ObjectPoolItem>());

            if (itemSetting.minPooledObjects > 0)
            {
                for (int i = 0; i < itemSetting.minPooledObjects; i++)
                {
                    ObjectPoolItem opi = InstantiateItem(itemSetting.tag);
                    if(opi != null)
                        availableObjects[itemSetting.tag].Push(opi);
                }
            }
        }

        yield return 0;
    }

    public GameObject SpawnObject(string objectTag)
    {
        if (!objectSettings.ContainsKey(objectTag))
            return null;

        ObjectPoolItem objPoolItem = null;
        if (availableObjects.ContainsKey(objectTag) && availableObjects[objectTag].Count > 0)
        {
            objPoolItem = availableObjects[objectTag].Pop();
        }
        else
        {
            objPoolItem = InstantiateItem(objectTag);
        }
        
        objPoolItem?.gameObject.SetActive(true);
        return objPoolItem?.gameObject;
    }

    private ObjectPoolItem InstantiateItem(string objectTag)
    {
        if (!objectSettings.ContainsKey(objectTag) || objectSettings[objectTag].prefab == null)
            return null;
        
        GameObject newObj = Instantiate(objectSettings[objectTag].prefab);
        newObj.SetActive(false);
        ObjectPoolItem objPoolItem = newObj.GetComponent<ObjectPoolItem>();

        if (objPoolItem == null)
        {
            objPoolItem = newObj.AddComponent<ObjectPoolItem>();
        }
        
        objPoolItem.ObjectPoolTag = objectTag;
        objPoolItem.masterPool = this;
        
        return objPoolItem;
    }
    public void ReturnObject(ObjectPoolItem obj)
    {
        if (obj == null)
        {
            return;
        }
        else if (!objectSettings.ContainsKey(obj.ObjectPoolTag))
        {
            Destroy(obj.gameObject);
            return;
        }

        if (availableObjects.ContainsKey(obj.ObjectPoolTag) && availableObjects[obj.ObjectPoolTag].Count <
            objectSettings[obj.ObjectPoolTag].maxPooledObjects)
        {
            obj.gameObject.SetActive(false);
            availableObjects[obj.ObjectPoolTag].Push(obj);
        }
        else
        {
            Destroy(obj.gameObject);
        }
    }

    public static void ReturnObject(GameObject obj)
    {
        ObjectPoolItem opi = obj?.GetComponent<ObjectPoolItem>();

        if (opi != null && opi.masterPool != null)
        {
            opi.masterPool.ReturnObject(opi);
        }
        else
        {
            Destroy(obj);
        }
    }
}
