using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [field: SerializeField] public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idling;

    public enum PlayerMovementState 
    {
        Idling =0,
        Walking = 1,
        Running = 2,
        Strafing = 3,
    
    }
}
