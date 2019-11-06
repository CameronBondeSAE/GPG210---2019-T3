using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanController : MonoBehaviour
{
    public Possessable possessionTarget;

    private void Update()
    {
        if (possessionTarget != null)
        {
            possessionTarget.LeftStickAxis(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
            
        }
    }



}
