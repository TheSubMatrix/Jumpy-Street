using MatrixUtils.Attributes;
using MatrixUtils.Extensions;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, ICharacterMovement
{
    [SerializeField] float m_moveTime;
    [SerializeField] float m_moveDistance;
    [SerializeField] float m_jumpHeight;

    Coroutine m_currentMoveRoutine;
    public void OnMoveRequested(Vector2 movementVector)
    {
        if (m_currentMoveRoutine is not null) return;
        m_currentMoveRoutine = StartCoroutine(MoveCharacter(movementVector));
    }

    IEnumerator MoveCharacter(Vector2 movementVector)
    {
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition;
        endPosition.x += movementVector.normalized.x * m_moveDistance;
        endPosition.z += movementVector.normalized.y * m_moveDistance;
        Vector3 updatedPosition = startPosition;
        Debug.Log(startPosition);
        Debug.Log(endPosition);
        while(elapsedTime < m_moveTime)
        {

            float additionalHeight = Mathf.Sin((elapsedTime / m_moveTime).Remap(0, 1, 0, Mathf.PI)) * m_jumpHeight;
            Debug.Log(additionalHeight);
            updatedPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / m_moveTime);
            updatedPosition.y += additionalHeight;
            transform.position = updatedPosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        m_currentMoveRoutine = null;
       
    }
}
