using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using GameScene.Obstacle;
using GameScene.Environment;
using GameScene.Player;
using GameScene.UI;
using GameScene.Rule;
using GameScene.Message;



namespace GameScene
{
    public class GameSceneLifeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterEntryPoint<BLOC>(Lifetime.Singleton).AsSelf();

            RegisterRule(builder);
            RegisterPool(builder);
            RegisterFacade(builder);
            RegisterController(builder);
            RegisterMessage(builder);
        }


        private void RegisterRule(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameRule>(Lifetime.Singleton).AsSelf();
        }

        private void RegisterPool(IContainerBuilder builder)
        {
            /*if (Parent == null)
            {
                Debug.LogError($"There Have No ParentScopoe in {nameof(GameSceneLifeScope)}");
                return;
            }*/

            var resouceContainer = Parent.Container.Resolve<SceneService>().GetCurrentResouceContainer<GameResourceContainer>();

            builder.Register(_ => resouceContainer, Lifetime.Singleton).As<BaseResourceContainer>();

            var poolFactory = builder.RegisterPoolFactory();

            PoolFactory.PoolModel poolinfo = new PoolFactory.PoolModel()
            {
                poolId = "ObstacleFacade",
                poolSize = 64,
                resourceId = "ObstacleFacade",
            };

            builder.RegisterPool<ObstacleFacade>(resouceContainer, poolFactory, poolinfo);
        }

        private void RegisterController(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<PlayerController>(Lifetime.Singleton).AsSelf();
            builder.RegisterEntryPoint<ObstacleController>(Lifetime.Singleton).AsSelf();
            builder.RegisterEntryPoint<InGameUIController>(Lifetime.Singleton).AsSelf();
            builder.RegisterEntryPoint<RoguelikeController>(Lifetime.Singleton).AsSelf();
        }

        private void RegisterFacade(IContainerBuilder builder)
        {
            builder.RegisterByHierarchy<PlayerFacade>(null, Hierarchy.PlayerFacade);
            
            builder.RegisterByHierarchy<EnvironmentFacade>(null, Hierarchy.EnvironmentFacade);
            builder.RegisterByHierarchy<InfoIndicateFacade>(null, Hierarchy.InfoIndicateFacade);
            builder.RegisterByHierarchy<InteractionFacade>(null, Hierarchy.InteractionFacade);
            builder.RegisterByHierarchy<RoguelikeFacade>(null, Hierarchy.RoguelikeFacade);
        }

        private void RegisterMessage(IContainerBuilder builder)
        {
            var option = builder.RegisterMessagePipe();

            builder.RegisterMessageBroker<DirectionButtonClick>(option);
            builder.RegisterMessageBroker<CountDownComplete>(option);
            builder.RegisterMessageBroker<GameOverEvent>(option);
            builder.RegisterMessageBroker<SceneLoadEvent>(option);
            builder.RegisterMessageBroker<ObstacleCrashEvent>(option);
            builder.RegisterMessageBroker<ScoreUpEvent>(option);
            builder.RegisterMessageBroker<RoguelikePayEvent>(option);
            builder.RegisterMessageBroker<RoguelikeRefreshEvent>(option);
            builder.RegisterMessageBroker<RoguelikeSkipEvent>(option);
        }


        public static class Hierarchy
        {
            public static readonly string PlayerFacade = "PlayerFacade";
            
            public static readonly string EnvironmentFacade = "EnvironmentFacade";


            #region UI
            public static readonly string InfoIndicateFacade = "UI/InfoIndicate";
            public static readonly string InteractionFacade = "UI/Interaction";
            public static readonly string RoguelikeFacade = "UI/RoguelikeFacade";
            #endregion
        }
    }
}