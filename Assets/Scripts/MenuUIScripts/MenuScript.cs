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

    public Scene currentScene;
    void Start()
    {
       
    }


    void OnClickQuit()
    {
        Application.Quit();
    }

    void OnClickStart()
    {
        
    }

    void OnClickOptions()
    {

    }

    void OnClickCredits()
    {

    }

    void OnClickControls()
    {

    }
}
