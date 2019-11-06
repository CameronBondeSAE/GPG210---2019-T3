using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Possessable possessable;

    private void Start()
    {
        possessable = FindObjectOfType<Possessable>();
    }

    private void Update()
    {
        possessable.LeftStickAxis(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        
//        float steering = Input.GetAxis("Horizontal") * 30f;
        //		float driving = Input.GetAxis("Vertical");
    }
}
