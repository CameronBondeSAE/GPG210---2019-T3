using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuel : MonoBehaviour
{
    public float currentFuel;
    public float maxFuel = 100f;

    //not each vehicle just references this bool and if its true then take no input
    public bool outOfFuel;

    public event Action OnOutOfFuel;

    public bool OutOfFuel
    {
        get
        {
            return currentFuel <= 0;
        }
    }
    
    //each vehicle references this function and calls it everytime there is input from player
    public void DrainFuel(float fuel)
    {
        currentFuel -= fuel;
        
        if(currentFuel <= 0 && outOfFuel != true)
        {
            outOfFuel = true;
            OnOutOfFuel?.Invoke();
        }
        else
        {
            outOfFuel = false;
        }
    }

    /*public bool OutOfFuel()
    {
        return currentFuel <= 0;
    }*/
}
