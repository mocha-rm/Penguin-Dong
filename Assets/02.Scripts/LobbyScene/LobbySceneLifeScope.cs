using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using LobbyScene.UI;



namespace LobbyScene
{
    public class LobbySceneLifeScope : LifetimeScope
    {
        [SerializeField] RectTransform _lobbyUI;


        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterComponent(_lobbyUI);
            builder.RegisterEntryPoint<LobbyUIFacade>(Lifetime.Singleton);
        }
    }
}
