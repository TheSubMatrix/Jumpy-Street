using System;
using System.Collections.Generic;
using AudioSystem;
using UnityEngine;

public class CharacterSoundPlayer : MonoBehaviour
{
    [SerializeField] SoundData m_deathSound;
    [SerializeField] SoundData m_jumpSound;
    public void PlayDeathSound()
    {
        SoundManager.Instance?.CreateSound().WithSoundData(m_deathSound).WithPosition(transform.position).WithRandomPitch().Play();
    }
    public void PlayJumpSound()
    {
        SoundManager.Instance?.CreateSound().WithSoundData(m_jumpSound).WithPosition(transform.position).WithRandomPitch().Play();
    }
}