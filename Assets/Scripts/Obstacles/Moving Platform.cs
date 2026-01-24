using UnityEngine;
using System.Collections.Generic;
using MatrixUtils.Attributes;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] List<Vector3> m_waypoints;
    [SerializeField] float m_moveSpeed = 2f;
    [ClassSelector, SerializeReference] WaypointNavigator m_navigator;
    
    Rigidbody m_rigidbody;
    float m_journeyProgress;
    float m_journeyLength;

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        List<Vector3> worldWaypoints = new();
        if (m_waypoints.Count == 0)
        {
            worldWaypoints.Add(transform.position);
        }
        else
        {
            Vector3 startPosition = transform.position;
            foreach (Vector3 localWaypoint in m_waypoints)
            {
                worldWaypoints.Add(startPosition + localWaypoint);
            }
        }
        
        m_navigator.Initialize(worldWaypoints);
        m_journeyProgress = 0f;
        m_journeyLength = Vector3.Distance(m_navigator.CurrentWaypoint, m_navigator.NextWaypoint);
    }
    
    void FixedUpdate()
    {
        if (m_waypoints.Count < 2 || !m_navigator.ShouldContinue()) return;
        m_journeyProgress += m_moveSpeed * Time.fixedDeltaTime;
        float t = Mathf.Clamp01(m_journeyProgress / m_journeyLength);
        //t = Mathf.SmoothStep(0f, 1f, t);
        Vector3 targetPosition = Vector3.Lerp(m_navigator.CurrentWaypoint, m_navigator.NextWaypoint, t);
        m_rigidbody.MovePosition(targetPosition);
        if (!(t >= 1f)) return;
        m_journeyProgress = 0f;
        m_navigator.Advance();
        m_journeyLength = Vector3.Distance(m_navigator.CurrentWaypoint, m_navigator.NextWaypoint);
    }
}