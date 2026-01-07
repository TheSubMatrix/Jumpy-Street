using MatrixUtils.Attributes;
using System;
using UnityEngine;
[Serializable]
public class CharacterContext
{
    [SerializeReference, ClassSelector] ICharacterController controller;
    [SerializeField] InterfaceReference<ICharacterMovement> m_characterMovement;
    public void Initialize()
    {
        controller.Initialize();
        controller.RequestMovement += m_characterMovement.Value.OnMoveRequested;
    }
    public void DeInitialize()
    {
        controller.RequestMovement -= m_characterMovement.Value.OnMoveRequested;
        controller.DeInitialize();
    }
}
