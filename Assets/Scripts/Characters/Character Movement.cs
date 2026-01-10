using MatrixUtils.Attributes;
using MatrixUtils.Extensions;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [RequiredField, SerializeField] InterfaceReference<ICharacterController> m_characterController;
    [SerializeField] float m_moveTime;
    [SerializeField] float m_moveDistance;
    [SerializeField] float m_jumpHeight;
    
    [Header("Events")]
    [SerializeField] UnityEvent<Vector2, float> m_onJump;
    
    Rigidbody m_rigidbody;
    Rigidbody m_connectedMovingGround;
    bool m_isGrounded;
    bool m_isJumping;
    Coroutine m_currentMoveRoutine;

    void OnEnable()
    {
        m_characterController.Value.Initialize();
        m_characterController.Value.RequestMovement += OnMoveRequested;
        m_rigidbody = GetComponent<Rigidbody>();
    }
    
    void OnDisable()
    {
        m_characterController.Value.RequestMovement -= OnMoveRequested;
        m_characterController.Value.DeInitialize();
    }

    void OnCollisionExit(Collision collision)
    {
        if (!IsGroundCollision(collision)) return;
        m_isGrounded = false;
        if (collision.rigidbody == m_connectedMovingGround)
        {
            m_connectedMovingGround = null;
        }
    }

    static bool IsGroundCollision(Collision collision)
    {
        return collision.contacts.Any(contact => Vector3.Dot(contact.normal, Vector3.up) > 0.7f);
    }

    void OnMoveRequested(Vector2 movementVector)
    {
        if (m_currentMoveRoutine is not null || !m_isGrounded) return;
        m_currentMoveRoutine = StartCoroutine(MoveCharacter(movementVector));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!IsGroundCollision(collision)) return;
        m_isGrounded = true;
        m_connectedMovingGround = collision.rigidbody;
    }

    IEnumerator MoveCharacter(Vector2 movementVector)
    {
        m_isGrounded = false;
    
        Vector2 normalizedMovement = movementVector.normalized;
        Vector3 movementOffset = new(normalizedMovement.x * m_moveDistance, 0, normalizedMovement.y * m_moveDistance);
        if (movementOffset.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementOffset);
            transform.rotation = targetRotation;
            m_rigidbody.rotation = targetRotation;
        }
    
        m_onJump?.Invoke(normalizedMovement, m_moveTime);
    
        float elapsedTime = 0;
        Vector3 startPosition = m_rigidbody.position;
        Rigidbody movingGround = m_connectedMovingGround;
        Vector3 platformStartPos = movingGround ? movingGround.position : Vector3.zero;
    
        while(elapsedTime <= m_moveTime)
        {
            float t = elapsedTime / m_moveTime;
            Vector3 movingGroundDelta = movingGround ? movingGround.position - platformStartPos : Vector3.zero;
            Vector3 targetPosition = startPosition + movementOffset * t + movingGroundDelta;
            targetPosition.y += Mathf.Sin(t.Remap(0, 1, 0, Mathf.PI)) * m_jumpHeight;

            m_rigidbody.MovePosition(targetPosition);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Vector3 finalPlatformDelta = movingGround ? movingGround.position - platformStartPos : Vector3.zero;
        m_rigidbody.MovePosition(startPosition + movementOffset + finalPlatformDelta);

        m_currentMoveRoutine = null;
    }
}