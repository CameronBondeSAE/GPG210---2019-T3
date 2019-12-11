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
                    ""name"": ""LeftTrigger"",
                    ""type"": ""Value"",
                    ""id"": ""12b0f818-6bdb-430b-8336-c7ebecb50314"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightTrigger"",
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
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""EnterExitButton"",
                    ""type"": ""Button"",
                    ""id"": ""fe86fcda-d471-476c-b218-d87dcf98cb36"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Respawn"",
                    ""type"": ""Button"",
                    ""id"": ""a7469b1c-1cb9-4182-96bb-6f34a1016ab3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Leave"",
                    ""type"": ""Button"",
                    ""id"": ""f038c42a-2903-4bbe-ab4e-c1d07959fb86"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=0.6)""
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
                    ""action"": ""LeftTrigger"",
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
                    ""action"": ""LeftTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""830522f8-61e3-4516-ab7c-c476216573cc"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""LeftTrigger"",
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
                    ""id"": ""5ad24cd3-4eb5-4b86-a943-f1244f119879"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=0.02,y=0.02)"",
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
                    ""action"": ""RightTrigger"",
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
                    ""action"": ""RightTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0af4e101-640c-4c35-bb3c-66b319b05bc7"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""RightTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a1fa07e1-9706-4bd3-9050-2818d027dc97"",
                    ""path"": ""<XInputController>/buttonSouth"",
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
                    ""id"": ""59773381-1d71-429e-8704-6df5c7424d39"",
                    ""path"": ""<Mouse>/rightButton"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""d412e523-e41a-4bbb-8bc2-3bbbf0705832"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Respawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e9750ed2-c5c1-4362-9df6-0136489ab40c"",
                    ""path"": ""<DualShockGamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4"",
                    ""action"": ""Respawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""234501e6-4fb8-484a-bea8-0a86c7b0a129"",
                    ""path"": ""<XInputController>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBOX"",
                    ""action"": ""Respawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d1fe0159-12a2-4a64-ac4e-7ecbf8fad700"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Leave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4d7e349a-987a-43ae-83b9-570cccc366e4"",
                    ""path"": ""<DualShockGamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4"",
                    ""action"": ""Leave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e29b2ccb-84b9-4518-8743-425de19c0f3c"",
                    ""path"": ""<XInputController>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBOX"",
                    ""action"": ""Leave"",
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
        m_vehicleControls_LeftTrigger = m_vehicleControls.FindAction("LeftTrigger", throwIfNotFound: true);
        m_vehicleControls_RightTrigger = m_vehicleControls.FindAction("RightTrigger", throwIfNotFound: true);
        m_vehicleControls_ActionButton1 = m_vehicleControls.FindAction("ActionButton1", throwIfNotFound: true);
        m_vehicleControls_EnterExitButton = m_vehicleControls.FindAction("EnterExitButton", throwIfNotFound: true);
        m_vehicleControls_Respawn = m_vehicleControls.FindAction("Respawn", throwIfNotFound: true);
        m_vehicleControls_Leave = m_vehicleControls.FindAction("Leave", throwIfNotFound: true);
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
    private readonly InputAction m_vehicleControls_Respawn;
    private readonly InputAction m_vehicleControls_Leave;
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
        public InputAction @Respawn => m_Wrapper.m_vehicleControls_Respawn;
        public InputAction @Leave => m_Wrapper.m_vehicleControls_Leave;
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
                Respawn.started -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnRespawn;
                Respawn.performed -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnRespawn;
                Respawn.canceled -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnRespawn;
                Leave.started -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnLeave;
                Leave.performed -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnLeave;
                Leave.canceled -= m_Wrapper.m_VehicleControlsActionsCallbackInterface.OnLeave;
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
                Respawn.started += instance.OnRespawn;
                Respawn.performed += instance.OnRespawn;
                Respawn.canceled += instance.OnRespawn;
                Leave.started += instance.OnLeave;
                Leave.performed += instance.OnLeave;
                Leave.canceled += instance.OnLeave;
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
        void OnRespawn(InputAction.CallbackContext context);
        void OnLeave(InputAction.CallbackContext context);
    }
}
