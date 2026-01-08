using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Vector3 m_targetOffset = new(5f, 0f, 0f);
    [SerializeField] float m_moveTime = 3f;

    Rigidbody m_rigidbody;
    Vector3 m_startPos;
    Vector3 m_endPos;
    float m_elapsedTime;

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.isKinematic = true;
        m_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        m_startPos = transform.position;
        m_endPos = m_startPos + m_targetOffset;
    }

    void FixedUpdate()
    {
        m_elapsedTime += Time.fixedDeltaTime;
        float t = Mathf.PingPong(m_elapsedTime / m_moveTime, 1f);
        t = Mathf.SmoothStep(0f, 1f, t);
        Vector3 targetPosition = Vector3.Lerp(m_startPos, m_endPos, t);
        m_rigidbody.MovePosition(targetPosition);
    }
}