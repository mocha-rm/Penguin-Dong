using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;


public class SceneLoader
{
    public enum SceneName
    {
        LobbyScene,
        GameScene,
        None,
    }

    [Inject] IObjectResolver _container;

    SceneName _currentScene = SceneName.None;
    BaseResourceContainer _resourceContainer;

    public async UniTaskVoid LoadScene(SceneName Scene)
    {
        if (_currentScene != SceneName.None && Scene == _currentScene)
        {
            var loadSc = await _resourceContainer.InstatiateLoadingScreenAsync();
            var sceneLoad = await Addressables.LoadSceneAsync(Scene.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single, false).ToUniTask(loadSc);
            await sceneLoad.ActivateAsync();
            _resourceContainer.ReleaseLoadingScreen();
        }
        else
        {
            if (_resourceContainer != null && _resourceContainer is DefaultResourceContainer == false)
            {
                _resourceContainer.ReleaseResources();
            }

            _resourceContainer = GetCurrentContainer(Scene);

            var loadSc = await _resourceContainer.InstatiateLoadingScreenAsync();

            loadSc.SetLoadingText("LoadingResource");
            await _resourceContainer.LoadResourcesAsync(loadSc);

            loadSc.SetLoadingText("LoadingScene");

            _currentScene = Scene;
            var sceneLoad = await Addressables.LoadSceneAsync(Scene.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single, false).ToUniTask(loadSc);
            await sceneLoad.ActivateAsync();


            _resourceContainer.ReleaseLoadingScreen();

        }
    }

    BaseResourceContainer GetCurrentContainer(SceneName scene)
    {
        switch (scene)
        {
            /*case SceneName.LobbyScene:
                return new LobbyScene.*/

            case SceneName.GameScene:
                return new GameScene.GameResourceContainer();   
        }

        return new DefaultResourceContainer();
    }

    public T GetCurrentResouceContainer<T>() where T : BaseResourceContainer
    {
        var container = _resourceContainer as T;
        if (container == null)
        {
            Debug.LogError("Container is Null");
            return null;
        }
        return container;
    }
}

