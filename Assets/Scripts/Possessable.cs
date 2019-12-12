using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.HID;


public class Possessable : SerializedMonoBehaviour
{
    public float spawnHeightOffset;
    public bool isFrozen;
    public Possessable masterPossessable;
    public CinemachineVirtualCamera virtualCamera;
    public Transform exitPosition;
    public Controller currentController;
    
    public virtual void LeftStickAxis(Vector2 value)
    {}
    public virtual void RightStickAxis(Vector2 value)
    {}
    public virtual void LeftTrigger(float value)
    {}
    public virtual void RightTrigger(float value)
    {}
    /*public virtual void Button(HID.Button button)
    {}*/
    public virtual void OnActionButton1()
    {}
    public virtual void OnEnterExitButton()
    {}

    public virtual void OnSpawn()
    {
        isFrozen = true;
        // Freeze on spawn
    }

    public virtual void Activate(Controller c)
    {
        if (currentController != null)
        {
            Eject();
        }
        //  Debug.Log(gameObject.name + " activated");
        currentController = c;
        isFrozen = false;
        //Unfreeze on here and in OnCollisionEnter
        //
    }

    public virtual void Deactivate()
    {
       // Debug.Log(gameObject.name + " deactivated");
        currentController = null;
    }

    public void Eject()
    {
        if (currentController?.possessable != null)
        {
            currentController.GetComponent<PlayerVehicleInteraction>().ExitVehicle();
        }
    }
}
