using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour, PlayerControls.IPlayerActions
{
    public PlayerControls PlayerControls { get; private set; }
    public Vector2 MovementInput { get; private set; }

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
        print(MovementInput);
    }
}
