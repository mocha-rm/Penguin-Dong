using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameScene
{
    public class GameResourceContainer : BaseResourceContainer
    {
        List<string> resourcesTag = new List<string>()
        {
            "GameScene",
        };

        Dictionary<string, GameObject> _gameObjContainer = new Dictionary<string, GameObject>();
        //Dictionary<string, AudioClip> _audioClipContainer = new Dictionary<string, AudioClip>();

        public override async UniTask LoadResourcesAsync(BaseLoadingScreen sc)
        {
            {
                //sc.SetLoadingText("GameObjectLoading");
                /*var handler = Addressables.LoadAssetsAsync<GameObject>(resourcesTag, (obj) =>
                {
                    _gameObjContainer.Add(obj.name, obj);
                }, Addressables.MergeMode.Intersection);*/
                //SetMergeMode(Addressables.MergeMode.Intersection, resourcesTag[0]);

                SetMergeMode(Addressables.MergeMode.Intersection, resourcesTag[0]);

                var handler = Addressables.LoadAssetAsync<GameObject>(resourcesTag[0]);
                handler.Completed += OnLoadCompleted;
                await handler.ToUniTask(sc);

                if (handler.IsValid())
                {
                    Debug.Log(handler.PercentComplete * 100);
                }
            }

            { 
                /*sc.SetLoadingText("AudioLoading");
                var handler = Addressables.LoadAssetsAsync<AudioClip>(resourcesTag, (audio) =>
                {
                    _audioClipContainer.Add(audio.name, audio);
                }, Addressables.MergeMode.Intersection);
                await handler.ToUniTask(sc);
                
                if(handler.IsValid())
                {
                    Addressables.Release(handler);
                }*/
            }
        }

        private void OnLoadCompleted(AsyncOperationHandle<GameObject> handle)
        {
            if(handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject loadedAsset = handle.Result;
                _gameObjContainer.Add(loadedAsset.name, loadedAsset);
            }
            else
            {
                Debug.LogError($"Failed to load the remote addressable asset");
            }
        }

        private void SetMergeMode(Addressables.MergeMode mode, string key)
        {
            Addressables.MergeMode _mode = mode;
            Addressables.ClearDependencyCacheAsync(key, true);
        }


        public override void ReleaseResources()
        {
            Debug.Log("Release Resouce Container");
            _gameObjContainer.Clear();
            //_audioClipContainer.Clear();
        }

        public override GameObject GetGameObject(string str)
        {
            if (_gameObjContainer.ContainsKey(str) == false)
            {
                Debug.LogError($"{str} is not Contain in ResourceContainer");
                return null;
            }
            else
            {
                if (_gameObjContainer[str] == null)
                {
                    Debug.Log($"Target is NULL");
                }
            }

            return _gameObjContainer[str];
        }

        /*public override AudioClip GetAudioClip(string addressableId)
        {
            if(_audioClipContainer.ContainsKey(addressableId) == false)
            {
                return null;
            }

            return _audioClipContainer[addressableId];
        }*/
    }
}
