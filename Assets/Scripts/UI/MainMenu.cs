using System.Collections;
using MatrixUtils.Attributes;
using MatrixUtils.DependencyInjection;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Inject] IScoreManager m_scoreManager;
    [Inject] ISceneTransitioner m_sceneTransitioner;
    [SerializeField, RequiredField] CanvasGroup m_instructionPanelGroup;
    [SerializeField, RequiredField] CanvasGroup m_controlsPanelGroup;
    [SerializeField, RequiredField] CanvasGroup m_creditsPanelGroup;
    [SerializeField, RequiredField] TMP_Text m_highScoreText;
    CanvasGroup m_activePanelGroup;
    CanvasGroup m_desiredPanelGroup;
    Coroutine m_swapRoutine;
    string m_highScorePrefix;
    void Start()
    {
        m_activePanelGroup = m_instructionPanelGroup;
        m_desiredPanelGroup = m_instructionPanelGroup;
        SetPanelState(m_instructionPanelGroup, true);
        SetPanelState(m_controlsPanelGroup, false);
        SetPanelState(m_creditsPanelGroup, false);
        m_highScorePrefix = m_highScoreText.text;
        UpdateHighScoreText(m_scoreManager.HighScoreDataObserver.Value);
        m_scoreManager.HighScoreDataObserver.AddListener(UpdateHighScoreText);
    }

    void OnDestroy()
    {
        m_scoreManager.HighScoreDataObserver.RemoveListener(UpdateHighScoreText);
    }
    
    void UpdateHighScoreText(ScoreData scoreData)
    {
        m_highScoreText.text = m_highScorePrefix + scoreData.Score;
    }
    
    static void SetPanelState(CanvasGroup panel, bool active)
    {
        panel.alpha = active ? 1 : 0;
        panel.interactable = active;
        panel.blocksRaycasts = active;
    }

    public void OnPlayButtonPressed()
    {
        m_sceneTransitioner.RequestTransitionTo("SampleScene");
    }
    public void OnInstructionButtonPressed()
    {
        SwapToPanel(m_instructionPanelGroup);
    }
    public void OnControlsButtonPressed()
    {
        SwapToPanel(m_controlsPanelGroup);
    }
    public void OnCreditsButtonPressed()
    {
        SwapToPanel(m_creditsPanelGroup);
    }

    void SwapToPanel(CanvasGroup panel)
    {
        if (m_desiredPanelGroup == panel) return;
        if (m_swapRoutine != null) StopCoroutine(m_swapRoutine);
        m_swapRoutine = StartCoroutine(SwapPanelGroup(panel));
    }
    
    IEnumerator SwapPanelGroup(CanvasGroup newPanelGroup)
    {
        m_desiredPanelGroup = newPanelGroup;
        if (m_activePanelGroup) yield return FadeCanvasGroup(m_activePanelGroup, 0);
        m_activePanelGroup = newPanelGroup;
        yield return FadeCanvasGroup(newPanelGroup, 1);
        m_swapRoutine = null;
    }

    static IEnumerator FadeCanvasGroup(CanvasGroup groupToFade, float desiredAlpha, float duration = 0.5f)
    {
        float startAlpha = groupToFade.alpha;
        float elapsedTime = 0;
        while (elapsedTime <= duration)
        {
            groupToFade.alpha = Mathf.Lerp(startAlpha, desiredAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        groupToFade.alpha = desiredAlpha;
        groupToFade.interactable = groupToFade.alpha > 0;
        groupToFade.blocksRaycasts = groupToFade.alpha > 0;
    }
}
