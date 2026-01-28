using AudioSystem;
using UnityEngine;

public class SoundRequest : MonoBehaviour
{
    [SerializeField] SoundData m_soundData;

    public void RequestSound()
    {
        SoundManager.Instance?.CreateSound().WithPosition(transform.position).WithSoundData(m_soundData).WithRandomPitch().Play();
    }
}
