using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;


    public sealed class DefaultResourceContainer : BaseResourceContainer
    {
        public override async UniTask LoadResourcesAsync(BaseLoadingScreen sc)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), DelayType.DeltaTime);
        }

        public override void ReleaseResources()
        {
        }
    }
