// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class Controls : IInputActionCollection, IDisposable
{
    private InputActionAsset asset;
    public Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""vehicleControls"",
            ""id"": ""bc405661-20e6-443b-8e28-3050a30a1efc"",
            ""actions"": [
                {
                    ""name"": ""leftStick"",
                    ""type"": ""Value"",
                    ""id"": ""2db30244-6f9c-420f-8a14-7fa381e30ce7"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""rightStick"",
                    ""type"": ""Value"",
                    ""id"": ""10e988b6-23d5-4779-a08a-5f20f7f8e1d6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Left Trigger"",
                    ""type"": ""Value"",
                    ""id"": ""12b0f818-6bdb-430b-8336-c7ebecb50314"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Right Trigger"",
                    ""type"": ""Value"",
                    ""id"": ""4c1a5099-4593-4ce5-8cdc-1dcf3c5a2f79"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ActionButton1"",
                    ""type"": ""Button"",
                    ""id"": ""b302cce1-49a2-448a-bb3b-86ca72836f54"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EnterExitButton"",
                    ""type"": ""Button"",
                    ""id"": ""fe86fcda-d471-476c-b218-d87dcf98cb36"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3af3ca1b-14a4-4a45-951b-9e53adfb356b"",
                    ""path"": ""<XInputController>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBOX"",
                    ""action"": ""leftStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8db187f1-0c72-46e1-b8aa-0cdd8a114059"",
                    ""path"": ""<DualShockGamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4"",
                    ""action"": ""leftStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dd3cbbad-5e17-48de-98c3-afd93fb2deba"",
                    ""path"": ""<XInputController>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBOX"",
                    ""action"": ""rightStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c2262ac6-6acd-462e-b552-2c13a1b7f4aa"",
                    ""path"": ""<DualShockGamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4"",
                    ""action"": ""rightStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""83c6f720-7747-4f9c-9323-56785c0bcaaa"",
                    ""path"": ""<XInputController>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBOX"",
                    ""action"": ""Left Trigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""79eb480a-8b96-423d-9019-d3461e4122b3"",
                    ""path"": ""<DualShockGamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4"",
                    ""action"": ""Left Trigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""830522f8-61e3-4516-ab7c-c476216573cc"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Left Trigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""db5e4b50-b801-4019-8519-0921e8eada86"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""leftStick"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""4caeacfc-0bb1-4da7-bb6d-8d4922d6132d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""leftStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""e328f4d4-ecc5-4db3-8b68-ce60ecde00f6"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""leftStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""15a5e010-3759-4743-84db-3fda55fe87c6"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""leftStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""73fdde5a-6f46-4cb2-872a-e637ca658c1b"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""leftStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""ArrowKeys"",
                    ""id"": ""59ba2fe2-d00d-4aac-a442-dff20275cf27"",
                    ""path"": ""2DVector(normalize=false)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""rightStick"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""bc203774-659d-49d2-bf37-371464a368e0"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""rightStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""215a68a7-2095-4435-95ec-302e597a920f"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""rightStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""5cf43ba8-601b-4a64-8f7a-22b079b4ab2f"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": ""Keyboard"",
                    ""action"": ""rightStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ce3b91a3-daf0-4e71-8e28-c8a3355beb5d"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": ""Keyboard"",
                    ""action"": ""rightStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""ff0dcab3-1438-42d6-ba99-c093e27c40e2"",
                    ""path"": ""*/{Primary2DMotion}"",
                    ""interactions"": """",
                    ""processors"": ""NormalizeVector2"",
                    ""groups"": ""Keyboard"",
                    ""action"": ""rightStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""12fa16e3-7ecd-40d5-925b-de947fd156b1"",
                    ""path"": ""<XInputController>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBOX"",
                    ""action"": ""Right Trigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""770b5204-e805-4ad5-81df-1d1885965042"",
                    ""path"": ""<DualShockGamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4"",
                    ""action"": ""Right Trigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0af4e101-640c-4c35-bb3c-66b319b05bc7"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Right Trigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a1fa07e1-9706-4bd3-9050-2818d027dc97"",
                    ""path"": ""<XInputController>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBOX"",
                    ""action"": ""ActionButton1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e3b0d0da-1ce2-4d92-8464-18dc949dc156"",
                    ""path"": ""<DualShockGamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4"",
                    ""action"": ""ActionButton1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a3805d51-fb5c-4463-b5e1-891b2ab88cc9"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ActionButton1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ea7839b5-623c-4a18-8c61-31741a3f03fb"",
                    ""path"": ""<XInputController>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBOX"",
                    ""action"": ""EnterExitButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""98d72726-3738-481c-ade0-9f5cd2029ab8"",
                    ""path"": ""<DualShockGamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4"",
                    ""action"": ""EnterExitButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bfed45f5-edf3-4d39-a806-8839b4e5cc2d"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""EnterExitButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""PS4"",
            ""bindingGroup"": ""PS4"",
            ""devices"": [
                {
                    ""devicePath"": ""<DualShockGamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""XBOX"",
            ""bindingGroup"": ""XBOX"",
            ""devices"": [
                {
                    ""devicePath"": ""<XInputController>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // vehicleControls
        m_vehicleControls = asset.FindActionMap("vehicleControls", throwIfNotFound: true);
        m_vehicleControls_leftStick = m_vehicleControls.FindAction("leftStick", throwIfNotFound: true);
        m_vehicleControls_rightStick = m_vehicleControls.FindAction("rightStick", throwIfNotFound: true);
        m_vehicleControls_LeftTrigger = m_vehicleControls.FindAction("Left Trigger", throwIfNotFound: true);
        m_vehicleControls_RightTrigger = m_vehicleControls.FindAction("Right Trigger", throwIfNotFound: true);
        m_vehicleControls_ActionButton1 = m_vehicleControls.FindAction("ActionButton1", throwIfNotFound: true);
        m_vehicleControls_EnterExitButton = m_vehicleControls.FindAction("EnterExitButton", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // vehicleControls
    private readonly InputActionMap m_vehicleControls;
    private IVehicleControlsActions m_VehicleControlsActionsCallbackInterface;
    private readonly InputAction m_vehicleControls_leftStick;
    private readonly InputAction m_vehicleControls_rightStick;
    private readonly InputAction m_vehicleControls_LeftTrigger;
    private readonly InputAction m_vehicleControls_RightTrigger;
    private readonly InputAction m_vehicleControls_ActionButton1;
    private readonly InputAction m_vehicleControls_EnterExitButton;
    public struct VehicleControlsActions
    {
        private Controls m_Wrapper;
        public VehicleControlsActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @leftStick => m_Wrapper.m_vehicleControls_leftStick;
        public InputAction @rightStick => m_Wrapper.m_vehicleControls_rightStick;
        public InputAction @LeftTrigger => m_Wrapper.m_vehicleControls_LeftTrigger;
        public InputAction @RightTrigger => m_Wrapper.m_vehicleControls_RightTrigger;
        public InputAction @ActionButton1 => m_Wrapper.m_vehicleControls_ActionButton1;
        public InputAction @EnterExitButton => m_Wrapper.m_vehicleControls_EnterExitButton;
        public InputActionMap Get() { return m_Wrapper.m_vehicleControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(VehicleControlsActions set) { return set.Get(); }
        public void SetCallbacks(IVehicleControlsActions instance)
        {
            if (m_Wrapper.m_VehicleControlsActionsCallbackInterface != null)
            {
                leftStick.started -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnLeftStick;
                leftStick.performed -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnLeftStick;
                leftStick.canceled -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnLeftStick;
                rightStick.started -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnRightStick;
                rightStick.performed -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnRightStick;
                rightStick.canceled -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnRightStick;
                LeftTrigger.started -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnLeftTrigger;
                LeftTrigger.performed -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnLeftTrigger;
                LeftTrigger.canceled -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnLeftTrigger;
                RightTrigger.started -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnRightTrigger;
                RightTrigger.performed -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnRightTrigger;
                RightTrigger.canceled -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnRightTrigger;
                ActionButton1.started -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnActionButton1;
                ActionButton1.performed -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnActionButton1;
                ActionButton1.canceled -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnActionButton1;
                EnterExitButton.started -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnEnterExitButton;
                EnterExitButton.performed -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnEnterExitButton;
                EnterExitButton.canceled -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnEnterExitButton;
            }
            m_Wrapper.m_VehicleControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                leftStick.started += instance.OnLeftStick;
                leftStick.performed += instance.OnLeftStick;
                leftStick.canceled += instance.OnLeftStick;
                rightStick.started += instance.OnRightStick;
                rightStick.performed += instance.OnRightStick;
                rightStick.canceled += instance.OnRightStick;
                LeftTrigger.started += instance.OnLeftTrigger;
                LeftTrigger.performed += instance.OnLeftTrigger;
                LeftTrigger.canceled += instance.OnLeftTrigger;
                RightTrigger.started += instance.OnRightTrigger;
                RightTrigger.performed += instance.OnRightTrigger;
                RightTrigger.canceled += instance.OnRightTrigger;
                ActionButton1.started += instance.OnActionButton1;
                ActionButton1.performed += instance.OnActionButton1;
                ActionButton1.canceled += instance.OnActionButton1;
                EnterExitButton.started += instance.OnEnterExitButton;
                EnterExitButton.performed += instance.OnEnterExitButton;
                EnterExitButton.canceled += instance.OnEnterExitButton;
            }
        }
    }
    public VehicleControlsActions @vehicleControls => new VehicleControlsActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    private int m_PS4SchemeIndex = -1;
    public InputControlScheme PS4Scheme
    {
        get
        {
            if (m_PS4SchemeIndex == -1) m_PS4SchemeIndex = asset.FindControlSchemeIndex("PS4");
            return asset.controlSchemes[m_PS4SchemeIndex];
        }
    }
    private int m_XBOXSchemeIndex = -1;
    public InputControlScheme XBOXScheme
    {
        get
        {
            if (m_XBOXSchemeIndex == -1) m_XBOXSchemeIndex = asset.FindControlSchemeIndex("XBOX");
            return asset.controlSchemes[m_XBOXSchemeIndex];
        }
    }
    public interface IVehicleControlsActions
    {
        void OnLeftStick(InputAction.CallbackContext context);
        void OnRightStick(InputAction.CallbackContext context);
        void OnLeftTrigger(InputAction.CallbackContext context);
        void OnRightTrigger(InputAction.CallbackContext context);
        void OnActionButton1(InputAction.CallbackContext context);
        void OnEnterExitButton(InputAction.CallbackContext context);
    }
}