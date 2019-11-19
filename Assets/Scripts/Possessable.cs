using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Possessable : SerializedMonoBehaviour
{
    public virtual void LeftStickAxis(Vector2 value)
    {
    }

    public virtual void RightStickAxis(Vector2 value)
    {
    }
    
    public virtual void LeftTrigger(float value)
    {}
    
    public virtual void RightTrigger(float value)
    {}
    
    public virtual void Button(HID.Button button)
    {}

    public virtual void OnActionButton1()
    {
        
    }

    public virtual void OnEnterExitButton()
    {
        
    }

}
