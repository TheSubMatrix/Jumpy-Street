using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneTransitioner
{
    void RequestTransitionTo(string sceneName);

    void ReloadScene()
    {
        RequestTransitionTo(SceneManager.GetActiveScene().name);
    }
}
