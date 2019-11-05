using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTimer : MonoBehaviour
{
    private Rigidbody rb;
    public float timer;
    public float currentvalue;
    // Start is called before the first frame update
    void Start()
    {
        currentvalue = timer;
        rb = this.GetComponent<Rigidbody>();
        rb.isKinematic = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (currentvalue > 0)
        {
            currentvalue -= Time.deltaTime;
        }
        else
        {
            currentvalue = 0;
            rb.isKinematic = false;
        }

    }
}
