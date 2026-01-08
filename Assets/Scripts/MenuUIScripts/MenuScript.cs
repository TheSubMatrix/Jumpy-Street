using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class MenuScript : MonoBehaviour
{
    public Button startButton;
    public Button optionsButton;
    public Button controlsButton;
    public Button creditsButton;
    public Button quitButton;

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


    void OnClickQuit()
    {
        Application.Quit();
    }

    void OnClickStart()
    {
        playerUI.SetActive(true);
    }

    void OnClickOptions()
    {
        menuScreen.SetActive(false);
        optionsScreen.SetActive(true);
        controlsScreen.SetActive(false);
        creditsScreen.SetActive(false);
        playerUI.SetActive(false);
    }

    void OnClickCredits()
    {
        menuScreen.SetActive(false);
        optionsScreen.SetActive(false);
        controlsScreen.SetActive(true);
        creditsScreen.SetActive(false);
        playerUI.SetActive(false);
    }

    void OnClickControls()
    {
        menuScreen.SetActive(false);
        optionsScreen.SetActive(false);
        controlsScreen.SetActive(true);
        creditsScreen.SetActive(false);
        playerUI.SetActive(false);
    }
}
