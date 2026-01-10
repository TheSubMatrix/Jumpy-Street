using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] string m_jumpTrigger = "Jump";
    [SerializeField] string m_jumpStateName = "Jump";
    [SerializeField] int m_animationLayer = 0;
    
    Animator m_animator;
    Coroutine m_currentAnimationRoutine;
    
    void Awake()
    {
        m_animator = GetComponent<Animator>();    
    }
    
    public void PlayJumpAnimation(Vector2 jumpDirection, float jumpTime)
    {
        if (m_currentAnimationRoutine != null)
        {
            StopCoroutine(m_currentAnimationRoutine);
        }

        m_currentAnimationRoutine = StartCoroutine(PlayJumpAnimationRoutine(jumpTime));
    }
    
    IEnumerator PlayJumpAnimationRoutine(float jumpTime)
    {
        m_animator.speed = 1f;

        m_animator.ResetTrigger(m_jumpTrigger);
        m_animator.SetTrigger(m_jumpTrigger);
        
        float startTime = Time.time;
        float endTime = startTime + jumpTime;
        yield return null;
        AnimatorStateInfo stateInfo = m_animator.GetNextAnimatorStateInfo(m_animationLayer);
        
        if (!stateInfo.IsName(m_jumpStateName))
        {
            stateInfo = m_animator.GetCurrentAnimatorStateInfo(m_animationLayer);
        }
        if (stateInfo.IsName(m_jumpStateName))
        {
            float requiredSpeed = stateInfo.length / jumpTime;
            m_animator.speed = requiredSpeed;
        }

        while (Time.time < endTime)
        {
            yield return null;
        }
        m_animator.speed = 1f;
        m_currentAnimationRoutine = null;
    }
}