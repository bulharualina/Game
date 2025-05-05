using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class PlayerInput : MonoBehaviour, PlayerControls.IPlayerActions
{
    #region Class Variables
    [SerializeField] private bool holdToSprint = true;
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool SwitchSprintOn { get; private set; }
    public bool SwitchWalkOn { get; private set; }
    public bool JumpPressed { get; private set; }

    #endregion

    #region StartUp
    private void OnEnable()
    {
        if (PlayerInputManager.Instance?.PlayerControls == null)
        {
            Debug.LogError("Player controls is not initialized - cannot enable");
            return;
        }

        PlayerInputManager.Instance.PlayerControls.Player.Enable();
        PlayerInputManager.Instance.PlayerControls.Player.SetCallbacks(this);
    }

    private void OnDisable()
    {
        if (PlayerInputManager.Instance?.PlayerControls == null)
        {
            Debug.LogError("Player controls is not initialized - cannot disable");
            return;
        }

        PlayerInputManager.Instance.PlayerControls.Player.Disable();
        PlayerInputManager.Instance.PlayerControls.Player.RemoveCallbacks(this);
    }
    #endregion

    #region Late Update Logic
    private void LateUpdate()
    {
        JumpPressed = false;
    }

    #endregion

    #region Input Callbacks
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

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) 
        {
            return;
        }

        JumpPressed = true;
    }

    public void OnWalkSwitch(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        SwitchWalkOn = !SwitchWalkOn;
    }
    #endregion
}
