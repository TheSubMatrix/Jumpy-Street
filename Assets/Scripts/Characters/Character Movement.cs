using System;
using MatrixUtils.Attributes;
using MatrixUtils.Extensions;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [RequiredField, SerializeReference, ClassSelector] ICharacterController m_characterController;
    [SerializeField] float m_moveTime;
    [SerializeField] float m_moveDistance;
    [SerializeField] float m_jumpHeight;
    Rigidbody m_rigidbody;
    Coroutine m_currentMoveRoutine;

    void Awake()
    {
        m_characterController.Initialize();
        m_characterController.RequestMovement += OnMoveRequested;
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void OnDestroy()
    {
        m_characterController.RequestMovement -= OnMoveRequested;
        m_characterController.DeInitialize();
    }

    void OnMoveRequested(Vector2 movementVector)
    {
        if (m_currentMoveRoutine is not null) return;
        m_currentMoveRoutine = StartCoroutine(MoveCharacter(movementVector));
    }

    IEnumerator MoveCharacter(Vector2 movementVector)
    {
        float elapsedTime = 0;
        Vector3 startPosition = m_rigidbody.position;
        Vector3 endPosition = startPosition;
        endPosition.x += movementVector.normalized.x * m_moveDistance;
        endPosition.z += movementVector.normalized.y * m_moveDistance;
        while(elapsedTime <= m_moveTime)
        {

            float additionalHeight = Mathf.Sin((elapsedTime / m_moveTime).Remap(0, 1, 0, Mathf.PI)) * m_jumpHeight;
            Vector3 updatedPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / m_moveTime);
            updatedPosition.y += additionalHeight;
            m_rigidbody.MovePosition(updatedPosition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        m_rigidbody.MovePosition(endPosition);
        m_currentMoveRoutine = null;
    }
}
