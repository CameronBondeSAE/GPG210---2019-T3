﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreen : MonoBehaviour
{
    public Camera cam1; 
    public Camera cam2;

    public bool Horizontal;

    void Start() 
    {
        ChangeSplitScreen() ;
    } 

    public void ChangeSplitScreen() 
    {
        Horizontal = !Horizontal;
        if(Horizontal) 
        {
            cam1.rect = new Rect(0,0,1,0.5f);
            cam2.rect = new Rect(0,0.5f,1,0.5f);
        } 
        else
        {
            cam1.rect = new Rect(0,0,0.5f,1);
            cam2.rect = new Rect(0.5f,0,0.5f,1);
        } 
    } 

}
