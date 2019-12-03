using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.HID;


public class Possessable : SerializedMonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform exitPosition;
    public Controller CurrentController;
    
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

    public virtual void Activate(Controller c)
    {
      //  Debug.Log(gameObject.name + " activated");
        CurrentController = c;
    }

    public virtual void Deactivate()
    {
       // Debug.Log(gameObject.name + " deactivated");
        //CurrentController = null;
       
    }

    public void Eject()
    {
        if (CurrentController?.possessable != null)
        {
            CurrentController.GetComponent<PlayerVehicleInteraction>().ExitVehicle();
        }
    }
}
