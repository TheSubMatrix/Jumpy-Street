using MatrixUtils.Attributes;
using System;
using MatrixUtils;
using MatrixUtils.GenericDatatypes;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class PlayerInputController : ICharacterController
{
    [SerializeReference, RequiredField] SerializableAssetReference<InputActionReference> m_moveAction = new();

    public event Action<Vector2> RequestMovement;

    public void Initialize()
    {
        InputAction action = m_moveAction.Asset.action;
        if (action == null)
        {
            Debug.LogError("Move action not configured!");
            return;
        }
        
        action.Enable();
        action.started += OnMoveAction;
    }
    
    public void DeInitialize()
    {
        InputAction action = m_moveAction.Asset.action;
        if (action == null) return;
        
        action.started -= OnMoveAction;
        action.Disable();
    }
    
    void OnMoveAction(InputAction.CallbackContext context)
    {
        RequestMovement?.Invoke(context.ReadValue<Vector2>());
    }
}