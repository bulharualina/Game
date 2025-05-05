using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonInput : MonoBehaviour, PlayerControls.IPlayer3RDActions
{
    #region Class Variables
    public Vector2 ScrollInput { get; private set; }

    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private float _cameraZoomSpeed = 0.1f;
    [SerializeField] private float _cameraMinZoom = 1f;
    [SerializeField] private float _cameraMaxZoom = 5f;

    private Cinemachine3rdPersonFollow _thirdPersonFollow;
    #endregion

    #region StartUp
    private void Awake()
    {
        _thirdPersonFollow = _virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }
    private void OnEnable()
    {
        if (PlayerInputManager.Instance?.PlayerControls == null)
        {
            Debug.LogError("Player controls is not initialized - cannot enable");
            return;
        }

        PlayerInputManager.Instance.PlayerControls.Player3RD.Enable();
        PlayerInputManager.Instance.PlayerControls.Player3RD.SetCallbacks(this);
    }

    private void OnDisable()
    {
        if (PlayerInputManager.Instance?.PlayerControls == null)
        {
            Debug.LogError("Player controls is not initialized - cannot disable");
            return;
        }

        PlayerInputManager.Instance.PlayerControls.Player3RD.Disable();
        PlayerInputManager.Instance.PlayerControls.Player3RD.RemoveCallbacks(this);
    }


    #endregion

    #region Update 
    private void Update()
    {
        _thirdPersonFollow.CameraDistance = Mathf.Clamp(_thirdPersonFollow.CameraDistance + ScrollInput.y, _cameraMinZoom, _cameraMaxZoom);
    }

    private void LateUpdate()
    {
        ScrollInput = Vector2.zero;
    }


    #endregion

    #region Input Callbacks
    public void OnScrollCamera(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Vector2 scrollInput = context.ReadValue<Vector2>();
        ScrollInput = -1f * scrollInput.normalized * _cameraZoomSpeed;
    }
    #endregion
}
