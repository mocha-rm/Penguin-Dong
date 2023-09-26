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
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            RegisterController(builder);
            RegisterFacade(builder);
            RegisterMessage(builder);
        }

        private void RegisterController(IContainerBuilder builder)
        {

        }

        private void RegisterFacade(IContainerBuilder builder)
        {
            builder.RegisterByHierarchy<LobbyUIFacade>(null, Hierarchy.LobbyUIFacade);
        }

        private void RegisterMessage(IContainerBuilder builder)
        {

        }


        public static class Hierarchy
        {
            public static readonly string LobbyUIFacade = "UI";
        }
    }
}
