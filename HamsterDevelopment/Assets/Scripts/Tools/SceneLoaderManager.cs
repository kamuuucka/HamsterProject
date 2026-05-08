using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventBus;
using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : Singleton<SceneLoaderManager>
{
    [SerializeField,Scene] private string loadingScene;

    private void Awake()
    {
        InitializeSingleton();
    }

    public void LoadScene(Scene scene)
    {
        
    }
    
    public void LoadScenes(SceneSetting sceneSettingData)
    {
        StartCoroutine(LoadScenesAsync(sceneSettingData));
    }

    public void FadeThenLoadScenes(SceneSetting sceneSettingData)
    {
        EventBusManager.Instance.Publish("Fader: Fade To Black");
        StartCoroutine(LoadScenesAsync(sceneSettingData, true));
    }
    
    private IEnumerator LoadScenesAsync(SceneSetting sceneSettingData, bool fadeOutAfterLoad = false)
    {
        if (fadeOutAfterLoad)
        {
            float delay = GetComponentInChildren<ScreenFader>().Duraction;
            yield return new WaitForSeconds(delay);
        }
        SceneManager.LoadScene(loadingScene);
        foreach (var scene in sceneSettingData.Scenes)
        {
            var operaton = SceneManager.LoadSceneAsync(scene.Scene, LoadSceneMode.Additive);
            while (!operaton.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        yield return SceneManager.UnloadSceneAsync(loadingScene);
        var activeScene = sceneSettingData.Scenes.FirstOrDefault(s => s.IsActiveScene);
        if(activeScene != null)
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(activeScene.Scene));
        if (fadeOutAfterLoad)
        {
            EventBusManager.Instance.Publish("Fader: Fade To Transparent");
        }
    }
}
