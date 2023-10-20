using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using VContainer;


public class T_AddressablesLoader : IInitializable
{
    [Inject] IObjectResolver _container;

    public string addressableKey = "GameScene";

    public void Initialize()
    {
        LoadRemoteAddressable();
    }

    private void LoadRemoteAddressable()
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
        handle.Completed += OnLoadCompleted;
    }

    private void OnLoadCompleted(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject loadedAsset = handle.Result;
            // Use the loaded asset as needed
        }
        else
        {
            Debug.LogError("Failed to load the remote addressable asset: " + handle.OperationException);
        }
    }

}
