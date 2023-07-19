using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;



public abstract class BaseResourceContainer
{
    public virtual string ScreenKey { get => "DefaultLoadingScreen"; }

    protected GameObject _screenObj;

    public virtual async UniTask<BaseLoadingScreen> InstatiateLoadingScreenAsync()
    {
        var screenObj = await Addressables.GetDownloadSizeAsync(ScreenKey);
        if (screenObj > 0)
        {
            await Addressables.DownloadDependenciesAsync(ScreenKey, true);
        }

        _screenObj = await Addressables.InstantiateAsync(ScreenKey);
        var loadSc = _screenObj.GetComponent<BaseLoadingScreen>();
        if (loadSc == null)
        {
            Debug.LogError("loadingScreen is Null");
        }
        return loadSc;
    }

    public virtual void ReleaseLoadingScreen()
    {
        if (_screenObj != null)
        {
            Addressables.ReleaseInstance(_screenObj);
        }
    }

    public abstract UniTask LoadResourcesAsync(BaseLoadingScreen sc);

    public abstract void ReleaseResources();
}