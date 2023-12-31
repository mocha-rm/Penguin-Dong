using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine;


    public sealed class PreloadResouceContainer : BaseResourceContainer
    {
        List<string> resourcesTag = new List<string>()
        {
            "Preload",
        };

        Dictionary<string, GameObject> _gameObjContainer = new Dictionary<string, GameObject>();

        public override async UniTask LoadResourcesAsync(BaseLoadingScreen sc)
        {
            {
                sc.SetLoadingText("GameObjectLoading");
                var handler = Addressables.LoadAssetsAsync<GameObject>(resourcesTag, (obj) =>
                {
                    _gameObjContainer.Add(obj.name, obj);
                }, Addressables.MergeMode.Intersection);
                await handler.ToUniTask(sc);

                if (handler.IsValid())
                {
                    Addressables.Release(handler);
                }
            }
        }

        public override void ReleaseResources()
        {
            Debug.Log("Release Resouce Container");
            _gameObjContainer.Clear();
        }

        public override GameObject GetGameObject(string str)
        {
            if (_gameObjContainer.ContainsKey(str) == false)
            {
                Debug.LogError($"{str} is not Contain in ResourceContainer");
                return null;
            }

            return _gameObjContainer[str];
        }

    }