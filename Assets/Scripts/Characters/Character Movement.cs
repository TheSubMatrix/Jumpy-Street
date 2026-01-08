using MatrixUtils.Attributes;
using MatrixUtils.Extensions;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [RequiredField, SerializeReference, ClassSelector] ICharacterController m_characterController;
    [SerializeField] float m_moveTime;
    [SerializeField] float m_moveDistance;
    [SerializeField] float m_jumpHeight;
    
    Rigidbody m_rigidbody;
    Rigidbody m_connectedMovingGround;
    bool m_isGrounded;
    bool m_isJumping;
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

    void OnCollisionEnter(Collision collision)
    {
        if (m_isJumping || !IsGroundCollision(collision)) return;
        m_isGrounded = true;
        m_connectedMovingGround = collision.rigidbody;
    }

    void OnCollisionStay(Collision collision)
    {
        if (m_isJumping || !IsGroundCollision(collision)) return;
        m_isGrounded = true;
        m_connectedMovingGround = collision.rigidbody;
    }

    void OnCollisionExit(Collision collision)
    {
        if (IsGroundCollision(collision)) m_isGrounded = false;
        if (collision.rigidbody == m_connectedMovingGround) m_connectedMovingGround = null;
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

    IEnumerator MoveCharacter(Vector2 movementVector)
    {
        m_isGrounded = false;
        m_isJumping = true;
        
        float elapsedTime = 0;
        Vector3 startPosition = m_rigidbody.position;
        Vector3 movementOffset = new(movementVector.normalized.x * m_moveDistance, 0, movementVector.normalized.y * m_moveDistance);
        
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
        
        m_isJumping = false;
        m_currentMoveRoutine = null;
    }
}