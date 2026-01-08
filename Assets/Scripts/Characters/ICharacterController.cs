using System;
using UnityEngine;

public interface ICharacterController
{
    public void Initialize();
    public void DeInitialize();
    public event Action<Vector2> RequestMovement;
}
