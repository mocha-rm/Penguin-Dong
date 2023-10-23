using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;


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

            return _gameObjContainer[str].gameObject;
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
