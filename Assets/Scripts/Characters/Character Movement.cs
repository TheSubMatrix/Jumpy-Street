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
    [SerializeField] UnityEvent<Vector2, float> m_onJump = new();
    [SerializeField] UnityEvent m_onJumpComplete = new();
    [SerializeField] UnityEvent<float> m_onBounceBack = new();
    
    Rigidbody m_rigidbody;
    Rigidbody m_connectedMovingGround;
    int m_groundContactCount;
    bool m_isMoving;
    bool m_hitObstacle;
    Coroutine m_currentMoveRoutine;
    bool m_canMove = true;
    
    bool IsGrounded => m_groundContactCount > 0;

    public void DisableMovement()
    {
        m_characterController.Value.RequestMovement -= OnMoveRequested;
        m_characterController.Value.DeInitialize();
        m_canMove = false;
    }

    public void EnableMovement()
    {
        m_characterController.Value.Initialize();
        m_characterController.Value.RequestMovement += OnMoveRequested;
        m_canMove = true;
    }

    void OnEnable()
    {
        EnableMovement();
        m_rigidbody = GetComponent<Rigidbody>();
    }
    
    void OnDisable()
    {
        DisableMovement();
    }

    void OnMoveRequested(Vector2 movementVector)
    {
        if (m_currentMoveRoutine != null || !IsGrounded || !m_canMove) return;
        m_currentMoveRoutine = StartCoroutine(MoveCharacter(movementVector));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (IsGroundCollision(collision))
        {
            m_groundContactCount++;
            m_connectedMovingGround = collision.rigidbody;
        }
        else if (m_isMoving) m_hitObstacle = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (!IsGroundCollision(collision)) return;
        m_groundContactCount--;
        if (collision.rigidbody == m_connectedMovingGround) m_connectedMovingGround = null;
    }

    static bool IsGroundCollision(Collision collision) => 
        collision.contacts.Any(contact => Vector3.Dot(contact.normal, Vector3.up) > 0.7f);

    IEnumerator MoveCharacter(Vector2 movementVector)
    {
        m_groundContactCount = 0;
        m_isMoving = true;
        m_hitObstacle = false;
    
        Vector3 offset = new(movementVector.x * m_moveDistance, 0, movementVector.y * m_moveDistance);
        
        if (offset.sqrMagnitude > 0.01f)
            transform.rotation = m_rigidbody.rotation = Quaternion.LookRotation(offset);
    
        m_onJump?.Invoke(movementVector.normalized, m_moveTime);
    
        Vector3 start = m_rigidbody.position;
        Rigidbody ground = m_connectedMovingGround;
        Vector3 groundStart = ground ? ground.position : Vector3.zero;
    
        yield return AnimateMove(start, offset, ground, groundStart);

        if (m_hitObstacle)
        {
            Vector3 collision = m_rigidbody.position;
            Vector3 groundDelta = ground ? ground.position - groundStart : Vector3.zero;
            float progress = Vector3.Distance(start, collision) / m_moveDistance;
            float bounceTime = progress * m_moveTime;
            m_onBounceBack?.Invoke(bounceTime);
            yield return AnimateBounceBack(collision, start + groundDelta - collision, bounceTime, progress);
        }
        else
        {
            Vector3 groundDelta = ground ? ground.position - groundStart : Vector3.zero;
            m_rigidbody.MovePosition(start + offset + groundDelta);
        }

        m_isMoving = false;
        m_currentMoveRoutine = null;
        m_onJumpComplete.Invoke();
    }

    IEnumerator AnimateMove(Vector3 start, Vector3 offset, Rigidbody ground, Vector3 groundStart)
    {
        float elapsedTime = 0;
        
        while (elapsedTime <= m_moveTime && !m_hitObstacle)
        {
            float normalizedProgress = elapsedTime / m_moveTime;
            Vector3 groundDelta = ground ? ground.position - groundStart : Vector3.zero;
            Vector3 pos = start + offset * normalizedProgress + groundDelta;
            pos.y += Mathf.Sin(normalizedProgress.Remap(0, 1, 0, Mathf.PI)) * m_jumpHeight;

            m_rigidbody.MovePosition(pos);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator AnimateBounceBack(Vector3 start, Vector3 offset, float duration, float startProgress)
    {
        float elapsed = 0;
        
        while (elapsed <= duration)
        {
            float normalizedProgress = elapsed / duration;
            float wave = Mathf.Lerp(startProgress, 0, normalizedProgress);
            Vector3 pos = start + offset * normalizedProgress;
            pos.y += Mathf.Sin(wave.Remap(0, 1, 0, Mathf.PI)) * m_jumpHeight;

            m_rigidbody.MovePosition(pos);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}