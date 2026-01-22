using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

[UsedImplicitly]
public class RemoveDirection : InputProcessor<Vector2>
{
#if UNITY_EDITOR
    static RemoveDirection()
    {
        Initialize();
    }
#endif
    
    [Tooltip("The X component of the direction to remove from the input"), UsedImplicitly]
    public float XInputVector;
    
    [Tooltip("The Y component of the direction to remove from the input"), UsedImplicitly]
    public float YInputVector;
    
    public override Vector2 Process(Vector2 value, InputControl control)
    {
        Vector2 directionToRemove = new Vector2(XInputVector, YInputVector).normalized;
        float projection = Vector2.Dot(value, directionToRemove);
        return value - Mathf.Max(0, projection) * directionToRemove;
    }
    
    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<RemoveDirection>();
    }
}