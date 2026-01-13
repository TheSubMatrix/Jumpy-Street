using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using System.Collections;

public class CharacterAnimationController : MonoBehaviour
{
    [System.Serializable]
    class AnimationLayer
    {
        public AnimationClip Clip;
        public AnimationClipPlayable Playable;
        [HideInInspector] public int Index;
    }

    [SerializeField] AnimationLayer m_jump;
    [SerializeField] AnimationLayer m_death;
    [SerializeField] AnimationLayer m_idle;
    [SerializeField] float m_blendDuration = 0.2f;
    
    PlayableGraph m_graph;
    AnimationMixerPlayable m_mixer;
    float m_currentJumpSpeed;

    void Awake()
    {
        m_graph = PlayableGraph.Create("CharacterAnimations");
        m_graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
    
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(m_graph, "Animation", GetComponent<Animator>());
        m_mixer = AnimationMixerPlayable.Create(m_graph, 3);
        output.SetSourcePlayable(m_mixer);
    
        SetupLayer(m_jump, 0);
        SetupLayer(m_death, 1);
        SetupLayer(m_idle, 2);
        SetWeight(m_idle, 1);
        m_graph.Play();
    }

    void SetupLayer(AnimationLayer layer, int index)
    {
        layer.Index = index;
        layer.Playable = AnimationClipPlayable.Create(m_graph, layer.Clip);
        m_graph.Connect(layer.Playable, 0, m_mixer, index);
    }

    public void PlayJumpAnimation(Vector2 jumpDirection, float jumpTime)
    {
        StopAllCoroutines();
        m_currentJumpSpeed = 1f;
        m_jump.Playable.SetTime(0);
        m_jump.Playable.SetSpeed(m_currentJumpSpeed);
        SetWeight(m_jump, 1);
        StartCoroutine(FadeToIdleAfter(jumpTime - m_blendDuration));
    }

    public void PlayBounceBackAnimation(float bounceTime)
    {
        StopAllCoroutines();
        m_jump.Playable.SetSpeed(-m_currentJumpSpeed);
        StartCoroutine(BounceBackSequence(bounceTime - m_blendDuration));
    }

    public void PlayDeathAnimation()
    {
        StopAllCoroutines();
        m_death.Playable.SetTime(0);
        SetWeight(m_death, 1);
    }

    IEnumerator FadeToIdleAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return BlendTo(m_idle, m_blendDuration);
    }

    IEnumerator BounceBackSequence(float animTime)
    {
        yield return BlendTo(m_jump, m_blendDuration);
        yield return new WaitForSeconds(animTime);
        yield return BlendTo(m_idle, m_blendDuration);
    }

    IEnumerator BlendTo(AnimationLayer target, float duration)
    {
        float[] startWeights = new float[3];
        for (int i = 0; i < 3; i++)
            startWeights[i] = m_mixer.GetInputWeight(i);
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            for (int i = 0; i < 3; i++)
            {
                float weight = i == target.Index ? 1 : 0;
                m_mixer.SetInputWeight(i, Mathf.Lerp(startWeights[i], weight, t));
            }
            yield return null;
        }
        
        SetWeight(target, 1);
    }

    void SetWeight(AnimationLayer layer, float weight)
    {
        for (int i = 0; i < 3; i++)
            m_mixer.SetInputWeight(i, i == layer.Index ? weight : 0);
    }

    void OnDestroy()
    {
        if (m_graph.IsValid()) m_graph.Destroy();
    }
}