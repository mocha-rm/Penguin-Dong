using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScene;
using VContainer.Unity;
using VContainer;
using MessagePipe;



namespace TestScene
{
    public class TestSceneLifeScope : LifetimeScope
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
            builder.RegisterEntryPoint<RoguelikeController>(Lifetime.Singleton).AsSelf();
        }

        private void RegisterFacade(IContainerBuilder builder)
        {
            builder.RegisterByHierarchy<RoguelikeFacade>(null, Hierarchy.RogueFacade);
        }

        private void RegisterMessage(IContainerBuilder builder)
        {
            var option = builder.RegisterMessagePipe();

            builder.RegisterMessageBroker<GameScene.Message.RoguelikePayEvent>(option);
            builder.RegisterMessageBroker<GameScene.Message.RoguelikeRefreshEvent>(option);
        }


        public static class Hierarchy
        {
            public static readonly string RogueFacade = "Canvas/RoguelikeFacade";
        }
    }
}
