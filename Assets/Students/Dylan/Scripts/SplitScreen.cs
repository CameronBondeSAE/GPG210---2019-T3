using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreen : MonoBehaviour
{
    //remember to make a new layer for each camera and cull the opposite camera from each view, eg cull cam2 from cam1's viewport, and cam1 from cam2's viewport
    public Camera cam1; 
    public Camera cam2;

    public bool Horizontal;

    void Start() 
    {
        SetUpCameraHorizontal();
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
    
    public void SetUpCameraHorizontal()
    {
        cam1.rect = new Rect(0,0,1,0.5f);
        cam2.rect = new Rect(0,0.5f,1,0.5f);
    }
    
    public void SetUpCameraVertical()
    {
        cam1.rect = new Rect(0,0,0.5f,1);
        cam2.rect = new Rect(0.5f,0,0.5f,1);    
    }

    

}
