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
    
    public void PlayJumpAnimation(Vector3 jumpDirection, float jumpTime)
    {
        if (m_currentAnimationRoutine != null)
        {
            StopCoroutine(m_currentAnimationRoutine);
        }
        
        m_currentAnimationRoutine = StartCoroutine(PlayJumpAnimationRoutine(jumpDirection, jumpTime));
    }
    
    IEnumerator PlayJumpAnimationRoutine(Vector3 jumpDirection, float jumpTime)
    {
        float originalSpeed = m_animator.speed;
        m_animator.SetTrigger(m_jumpTrigger);
        yield return null;
        AnimatorStateInfo nextState = m_animator.GetNextAnimatorStateInfo(m_animationLayer);
        if (!nextState.IsName(m_jumpStateName))
        {
            m_currentAnimationRoutine = null;
            yield break;
        }
        while (m_animator.IsInTransition(m_animationLayer))
        {
            yield return null;
        }
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(m_animationLayer);
        float requiredSpeed = stateInfo.length / jumpTime;
        m_animator.speed = requiredSpeed;
        yield return new WaitForSeconds(jumpTime);
        m_animator.speed = originalSpeed;
        
        m_currentAnimationRoutine = null;
    }
}