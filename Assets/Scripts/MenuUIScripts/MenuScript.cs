using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class MenuScript : MonoBehaviour
{
    public GameObject menuScreen;
    public GameObject optionsScreen;
    public GameObject controlsScreen;
    public GameObject creditsScreen;
    public GameObject playerUI;

    public Scene currentScene;
    void Start()
    {
        menuScreen.SetActive(true);
        optionsScreen.SetActive(false);
        controlsScreen.SetActive(false);
        creditsScreen.SetActive(false);
        playerUI.SetActive(false);
    }


    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickStart()
    {
        playerUI.SetActive(true);
    }

    public void OnClickOptions()
    {
        menuScreen.SetActive(false);
        optionsScreen.SetActive(true);
        controlsScreen.SetActive(false);
        creditsScreen.SetActive(false);
        playerUI.SetActive(false);
    }

    public void OnClickCredits()
    {
        menuScreen.SetActive(false);
        optionsScreen.SetActive(false);
        controlsScreen.SetActive(true);
        creditsScreen.SetActive(false);
        playerUI.SetActive(false);
    }

    public void OnClickControls()
    {
        menuScreen.SetActive(false);
        optionsScreen.SetActive(false);
        controlsScreen.SetActive(true);
        creditsScreen.SetActive(false);
        playerUI.SetActive(false);
    }
}
