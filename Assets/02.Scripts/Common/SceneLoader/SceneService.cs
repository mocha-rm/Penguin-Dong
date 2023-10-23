using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;


public class SceneService : IInitializable, IDisposable
{

    [Inject] IObjectResolver _container;

    SceneName _currentScene = SceneName.None;
    PreloadResouceContainer _preloadResouceContainer;
    BaseResourceContainer _resourceContainer;

    IDisposable _disposable;


    public void Initialize()
    {
        _currentScene = SceneName.None;
        _resourceContainer = null;
    }

    public void Dispose()
    {
        if (_resourceContainer != null)
        {
            _resourceContainer.ReleaseResources();
            _resourceContainer = null;
        }

        _disposable?.Dispose();
    }

    public async UniTask LoadPreload()
    {
        _preloadResouceContainer = new PreloadResouceContainer();

        var loadSc = await _preloadResouceContainer.InstatiateLoadingScreenAsync();
        loadSc.SetLoadingText("LoadingPreResources");

        await _preloadResouceContainer.LoadResourcesAsync(loadSc);

        _preloadResouceContainer.ReleaseLoadingScreen();
    }

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
            if (_resourceContainer != null)
            {
                _resourceContainer.ReleaseResources();
                _resourceContainer = null;
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
            case SceneName.GameScene:
                return new GameScene.GameResourceContainer();
        }

        return new DefaultResourceContainer();
    }

    public T GetCurrentResouceContainer<T>() where T : BaseResourceContainer
    {
        if (typeof(T) == typeof(PreloadResouceContainer))
        {
            return _preloadResouceContainer as T;
        }

        var container = _resourceContainer as T;
        if (container == null)
        {
            Debug.LogError($"Container is Null {nameof(T)}");
            return null;
        }
        return container;
    }

}
