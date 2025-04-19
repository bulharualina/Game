using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class PlayerInput : MonoBehaviour, PlayerControls.IPlayerActions
{
    [SerializeField] private bool holdToSprint = true;

    public bool SwitchSprintOn { get; private set; }

    public PlayerControls PlayerControls { get; private set; }
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }


    private void OnEnable()
    {
        PlayerControls = new PlayerControls();
        PlayerControls.Enable();

        PlayerControls.Player.Enable();
        PlayerControls.Player.SetCallbacks(this);
    }

    private void OnDisable()
    {
        PlayerControls.Player.Disable();
        PlayerControls.Player.RemoveCallbacks(this);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
        //print(MovementInput);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnSprintSwitch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwitchSprintOn = holdToSprint || !SwitchSprintOn;
        }
        else if (context.canceled) 
        {
            SwitchSprintOn = !holdToSprint && SwitchSprintOn;
        }
       
    }
}
