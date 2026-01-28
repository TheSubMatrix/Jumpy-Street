using System;
using AudioSystem;
using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class ScorePickup : MonoBehaviour
{
    [SerializeField] float m_bobSpeed;
    [SerializeField] float m_bobStrength;
    [SerializeField] float m_rotationSpeed;
    [SerializeField] uint m_extraPoints = 5;
    [SerializeField] SoundData m_soundData;
    Rigidbody m_rb;
    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        this.RequestInjection();
    }

    [Inject, UsedImplicitly]
    IScoreReaderWriter m_scoreReaderWriter;
    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        m_scoreReaderWriter.UpdateExtraPoints(m_scoreReaderWriter.GetCurrentScore().ExtraPoints + m_extraPoints);
        SoundManager.Instance?.CreateSound().WithPosition(transform.position).WithSoundData(m_soundData).WithRandomPitch().Play();
        gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        m_rb.MovePosition(new(transform.position.x, transform.position.y + Mathf.Sin(Time.time * m_bobSpeed) * m_bobStrength * Time.deltaTime, transform.position.z));
        m_rb.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles. y + m_rotationSpeed * Time.deltaTime, transform.rotation.eulerAngles.z));
    }
}
