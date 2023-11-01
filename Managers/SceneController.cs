using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviourSingletonPersistent<SceneController>
{    
    //SceneField, you can download from GoodScripts/Helpers in github

    [SerializeField] SceneField gameScene;
    [SerializeField] SceneField mainMenuScene;
    [SerializeField] SceneField loadingScene;
    [SerializeField] SceneField miniGameScene;

    string targetSceneName;

    public SceneField currentScene { get; private set; }

    public Action onLoadedGameScene;
    public Action onLoadedMainScene;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += LoadedLoadingScene;
    }
    public void LoadScene(string sceneName)
    {
        targetSceneName = sceneName;

        //SceneManager.sceneLoaded += LoadedLoadingScene;

        SceneManager.LoadScene(loadingScene);
    }
    public void LoadScene(int index)
    {
        targetSceneName = SceneManager.GetSceneAt(index).name;

        //SceneManager.sceneLoaded += LoadedLoadingScene;

        SceneManager.LoadScene(loadingScene);
    }            
    public void LoadGameScene()
    {
        LoadScene(gameScene);
    }
    public void LoadMainMenuScene()
    {
        LoadScene(mainMenuScene);
    }
    public void LoadMiniGameScene()
    {
        LoadScene(miniGameScene);
    }
    public void RestartScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }    
    private void LoadedLoadingScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == loadingScene)
        {
            //SceneManager.sceneLoaded -= LoadedLoadingScene;

            LoadingScreen manager = FindFirstObjectByType<LoadingScreen>();
            manager.LoadScene(targetSceneName);
        }
        else if (scene.name == gameScene.SceneName)
        {
            onLoadedGameScene?.Invoke();
        }
        else if (scene.name == mainMenuScene.SceneName)
        {
            onLoadedMainScene?.Invoke();
        }
    }
}