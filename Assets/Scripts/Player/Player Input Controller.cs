using MatrixUtils.Attributes;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
[Serializable]
public class PlayerInputController : ICharacterController
{
    [SerializeField, RequiredField] InputActionReference m_moveAction;

    public event Action<Vector2> RequestMovement;

    public void Initialize()
    {
        m_moveAction.action.Enable();
        m_moveAction.action.started += OnMoveAction;
        m_moveAction.action.canceled += OnMoveAction;
    }
    public void DeInitialize()
    {
        m_moveAction.action.started -= OnMoveAction;
        m_moveAction.action.canceled -= OnMoveAction;
        m_moveAction.action.Disable();
    }
    void OnMoveAction(InputAction.CallbackContext context)
    {
        RequestMovement?.Invoke(context.ReadValue<Vector2>());
    }
}
