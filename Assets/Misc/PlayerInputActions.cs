//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Misc/PlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""ac936f85-8aaf-4a00-96fc-b0ac09050f42"",
            ""actions"": [
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""cdf29bbb-466e-4042-8744-8a49ebde5a0c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""TurnInputs"",
                    ""type"": ""Value"",
                    ""id"": ""2e586c3a-3704-41e1-b99f-dc3c8e52ca7d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""VertInputs"",
                    ""type"": ""Button"",
                    ""id"": ""7541731e-aca1-44bd-8e92-f2ecedaa7d57"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Vacuum"",
                    ""type"": ""Button"",
                    ""id"": ""d5664c60-226a-4f78-a0bd-a3f65d6cd959"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Canon"",
                    ""type"": ""Button"",
                    ""id"": ""be2569c7-1106-4074-902a-f9247744bdcd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SlowTime"",
                    ""type"": ""Button"",
                    ""id"": ""78591756-4c43-4343-8e8b-4da602ba4dc3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""_ToggleTP"",
                    ""type"": ""Button"",
                    ""id"": ""b3044324-7a2a-4982-81cc-75e007b91973"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""_AddFuel"",
                    ""type"": ""Button"",
                    ""id"": ""b56b262c-fc67-4979-b99f-a7145e542950"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""_CycleCrosshair"",
                    ""type"": ""Button"",
                    ""id"": ""6707b499-9332-4655-9d7e-59a5dde129bf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""_ToggleMirror"",
                    ""type"": ""Button"",
                    ""id"": ""622a0427-1d76-47cc-977f-e5a7b8b11e5f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4c812bc2-fb3f-4991-b7f3-f2c809d39503"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Canon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de3de582-4be2-4698-8552-f782219e2367"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Canon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c35a8bda-2cd9-4578-96d0-f7a6b4d9e694"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Canon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""90a434b2-829e-4b78-a97c-e41626c5db6d"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a17998b5-c473-4ec6-a4db-1a7c6a42452c"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone,ScaleVector2(x=5,y=5)"",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""ArrowKeys"",
                    ""id"": ""0a47293f-d8cd-4274-a5ea-1570e50e7b81"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=6)"",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""90084984-0047-41ad-94c9-0b4e897f9041"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3bff8a59-9e0b-4229-bbe4-1e31308d1568"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""6e685e5f-7ba8-4ad6-b291-947c5e89b4ac"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""aa70286c-98b6-47bf-a786-4d9826a277de"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8ae0bc02-181d-4e83-803e-1cade52989e7"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SlowTime"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7828ed62-44ed-4851-9912-8a3fab4b71a4"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SlowTime"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3c08076c-b1bc-4628-a991-68648ccc410e"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""_AddFuel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a2db3675-2a8d-4043-818f-78ae3ee0fc6d"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""_AddFuel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""d7107e90-c60d-4181-a1cf-9f2210a31a9e"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnInputs"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""94951541-1ad1-4cfa-a34a-95589b46af0a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnInputs"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f153bcb9-ad19-42fb-b58a-91d615bb259f"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnInputs"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""fcc8291c-e3ee-4a3e-bd5c-8c70decd7e5b"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnInputs"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""22a006ca-2e2e-45c8-88a7-093ed85f74cc"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnInputs"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Controller"",
                    ""id"": ""d3e99cb5-1c69-427b-9dae-c410c51e6807"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnInputs"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""2d09cb45-c4b1-45a1-baff-aacf0d1137be"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnInputs"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ccffdb6c-2a97-4962-bc04-0b0b609f5689"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnInputs"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""194e4b1f-81e6-4c1c-890b-760ba28fe4a3"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnInputs"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""00e42221-77a7-4425-b415-ea278eacfdba"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnInputs"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""b053536c-634e-4d4d-9848-9e90bd5f6121"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VertInputs"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""7515689d-67ec-49c2-98e6-60a92c7ee9ae"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VertInputs"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""f4c7e5b0-9098-47dd-8d66-a3d992b4e694"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VertInputs"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""400c6a4f-54f4-48e3-82b9-7ad0b72ae65c"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VertInputs"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""c41be424-56e0-4ec6-9882-359adcebf492"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VertInputs"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""6abe495c-a94c-497a-bac4-a86ad79818ca"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VertInputs"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""39f40fb8-7425-4e86-af83-40fbecff3eac"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vacuum"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""52b27a46-5976-42a6-a234-627f40b2f5e6"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vacuum"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""60a3ac19-c774-4b65-b4f5-240b704cec95"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vacuum"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""5bfb5ca2-5c93-433d-874f-b4ae8449e39c"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""_CycleCrosshair"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""cc4049de-7cb8-4504-ba5a-5e3cd52259ad"",
                    ""path"": ""<Keyboard>/comma"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""_CycleCrosshair"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""e07a042b-7587-48d1-9fa0-adea5a635598"",
                    ""path"": ""<Keyboard>/period"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""_CycleCrosshair"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""518834d8-f541-4e50-80e3-5e15690e526e"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""_ToggleMirror"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""abbafb76-1f16-4df5-bda9-38be448cff86"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""_ToggleTP"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PauseMenu"",
            ""id"": ""96fecc3c-1941-4204-8ce9-180390c599f1"",
            ""actions"": [
                {
                    ""name"": ""Escape"",
                    ""type"": ""Button"",
                    ""id"": ""d15aebfc-5b76-40eb-a751-1b68cad3f22c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""1a5ef6d2-1e77-4756-a406-31ad54c64962"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Escape"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
        m_Player_TurnInputs = m_Player.FindAction("TurnInputs", throwIfNotFound: true);
        m_Player_VertInputs = m_Player.FindAction("VertInputs", throwIfNotFound: true);
        m_Player_Vacuum = m_Player.FindAction("Vacuum", throwIfNotFound: true);
        m_Player_Canon = m_Player.FindAction("Canon", throwIfNotFound: true);
        m_Player_SlowTime = m_Player.FindAction("SlowTime", throwIfNotFound: true);
        m_Player__ToggleTP = m_Player.FindAction("_ToggleTP", throwIfNotFound: true);
        m_Player__AddFuel = m_Player.FindAction("_AddFuel", throwIfNotFound: true);
        m_Player__CycleCrosshair = m_Player.FindAction("_CycleCrosshair", throwIfNotFound: true);
        m_Player__ToggleMirror = m_Player.FindAction("_ToggleMirror", throwIfNotFound: true);
        // PauseMenu
        m_PauseMenu = asset.FindActionMap("PauseMenu", throwIfNotFound: true);
        m_PauseMenu_Escape = m_PauseMenu.FindAction("Escape", throwIfNotFound: true);
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
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Look;
    private readonly InputAction m_Player_TurnInputs;
    private readonly InputAction m_Player_VertInputs;
    private readonly InputAction m_Player_Vacuum;
    private readonly InputAction m_Player_Canon;
    private readonly InputAction m_Player_SlowTime;
    private readonly InputAction m_Player__ToggleTP;
    private readonly InputAction m_Player__AddFuel;
    private readonly InputAction m_Player__CycleCrosshair;
    private readonly InputAction m_Player__ToggleMirror;
    public struct PlayerActions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Look => m_Wrapper.m_Player_Look;
        public InputAction @TurnInputs => m_Wrapper.m_Player_TurnInputs;
        public InputAction @VertInputs => m_Wrapper.m_Player_VertInputs;
        public InputAction @Vacuum => m_Wrapper.m_Player_Vacuum;
        public InputAction @Canon => m_Wrapper.m_Player_Canon;
        public InputAction @SlowTime => m_Wrapper.m_Player_SlowTime;
        public InputAction @_ToggleTP => m_Wrapper.m_Player__ToggleTP;
        public InputAction @_AddFuel => m_Wrapper.m_Player__AddFuel;
        public InputAction @_CycleCrosshair => m_Wrapper.m_Player__CycleCrosshair;
        public InputAction @_ToggleMirror => m_Wrapper.m_Player__ToggleMirror;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Look.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @TurnInputs.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnInputs;
                @TurnInputs.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnInputs;
                @TurnInputs.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnInputs;
                @VertInputs.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVertInputs;
                @VertInputs.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVertInputs;
                @VertInputs.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVertInputs;
                @Vacuum.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVacuum;
                @Vacuum.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVacuum;
                @Vacuum.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVacuum;
                @Canon.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCanon;
                @Canon.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCanon;
                @Canon.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCanon;
                @SlowTime.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSlowTime;
                @SlowTime.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSlowTime;
                @SlowTime.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSlowTime;
                @_ToggleTP.started -= m_Wrapper.m_PlayerActionsCallbackInterface.On_ToggleTP;
                @_ToggleTP.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.On_ToggleTP;
                @_ToggleTP.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.On_ToggleTP;
                @_AddFuel.started -= m_Wrapper.m_PlayerActionsCallbackInterface.On_AddFuel;
                @_AddFuel.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.On_AddFuel;
                @_AddFuel.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.On_AddFuel;
                @_CycleCrosshair.started -= m_Wrapper.m_PlayerActionsCallbackInterface.On_CycleCrosshair;
                @_CycleCrosshair.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.On_CycleCrosshair;
                @_CycleCrosshair.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.On_CycleCrosshair;
                @_ToggleMirror.started -= m_Wrapper.m_PlayerActionsCallbackInterface.On_ToggleMirror;
                @_ToggleMirror.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.On_ToggleMirror;
                @_ToggleMirror.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.On_ToggleMirror;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @TurnInputs.started += instance.OnTurnInputs;
                @TurnInputs.performed += instance.OnTurnInputs;
                @TurnInputs.canceled += instance.OnTurnInputs;
                @VertInputs.started += instance.OnVertInputs;
                @VertInputs.performed += instance.OnVertInputs;
                @VertInputs.canceled += instance.OnVertInputs;
                @Vacuum.started += instance.OnVacuum;
                @Vacuum.performed += instance.OnVacuum;
                @Vacuum.canceled += instance.OnVacuum;
                @Canon.started += instance.OnCanon;
                @Canon.performed += instance.OnCanon;
                @Canon.canceled += instance.OnCanon;
                @SlowTime.started += instance.OnSlowTime;
                @SlowTime.performed += instance.OnSlowTime;
                @SlowTime.canceled += instance.OnSlowTime;
                @_ToggleTP.started += instance.On_ToggleTP;
                @_ToggleTP.performed += instance.On_ToggleTP;
                @_ToggleTP.canceled += instance.On_ToggleTP;
                @_AddFuel.started += instance.On_AddFuel;
                @_AddFuel.performed += instance.On_AddFuel;
                @_AddFuel.canceled += instance.On_AddFuel;
                @_CycleCrosshair.started += instance.On_CycleCrosshair;
                @_CycleCrosshair.performed += instance.On_CycleCrosshair;
                @_CycleCrosshair.canceled += instance.On_CycleCrosshair;
                @_ToggleMirror.started += instance.On_ToggleMirror;
                @_ToggleMirror.performed += instance.On_ToggleMirror;
                @_ToggleMirror.canceled += instance.On_ToggleMirror;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // PauseMenu
    private readonly InputActionMap m_PauseMenu;
    private IPauseMenuActions m_PauseMenuActionsCallbackInterface;
    private readonly InputAction m_PauseMenu_Escape;
    public struct PauseMenuActions
    {
        private @PlayerInputActions m_Wrapper;
        public PauseMenuActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Escape => m_Wrapper.m_PauseMenu_Escape;
        public InputActionMap Get() { return m_Wrapper.m_PauseMenu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PauseMenuActions set) { return set.Get(); }
        public void SetCallbacks(IPauseMenuActions instance)
        {
            if (m_Wrapper.m_PauseMenuActionsCallbackInterface != null)
            {
                @Escape.started -= m_Wrapper.m_PauseMenuActionsCallbackInterface.OnEscape;
                @Escape.performed -= m_Wrapper.m_PauseMenuActionsCallbackInterface.OnEscape;
                @Escape.canceled -= m_Wrapper.m_PauseMenuActionsCallbackInterface.OnEscape;
            }
            m_Wrapper.m_PauseMenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Escape.started += instance.OnEscape;
                @Escape.performed += instance.OnEscape;
                @Escape.canceled += instance.OnEscape;
            }
        }
    }
    public PauseMenuActions @PauseMenu => new PauseMenuActions(this);
    public interface IPlayerActions
    {
        void OnLook(InputAction.CallbackContext context);
        void OnTurnInputs(InputAction.CallbackContext context);
        void OnVertInputs(InputAction.CallbackContext context);
        void OnVacuum(InputAction.CallbackContext context);
        void OnCanon(InputAction.CallbackContext context);
        void OnSlowTime(InputAction.CallbackContext context);
        void On_ToggleTP(InputAction.CallbackContext context);
        void On_AddFuel(InputAction.CallbackContext context);
        void On_CycleCrosshair(InputAction.CallbackContext context);
        void On_ToggleMirror(InputAction.CallbackContext context);
    }
    public interface IPauseMenuActions
    {
        void OnEscape(InputAction.CallbackContext context);
    }
}
