using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using GameScene.Icicle;
using GameScene.Player;
using GameScene.UI;
using GameScene.Rule;
using GameScene.Message;



namespace GameScene
{
    public class GameSceneLifeScope : LifetimeScope
    {
        [Header("Facades")]
        [SerializeField] PlayerFacade _player;
        [SerializeField] InGameUIFacade _ingameUI;


        //UI
        //+ UI Facades


        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            RegisterRule(builder);
            RegisterPool(builder);
            RegisterController(builder);
            RegisterFacade(builder);
            RegisterMessage(builder);
        }


        private void RegisterRule(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameRule>().AsSelf();
            builder.Register<BloC>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
        }

        private void RegisterPool(IContainerBuilder builder)
        {
            var resouceContainer = Parent.Container.Resolve<SceneLoader>().GetCurrentResouceContainer<GameResourceContainer>();

            Dictionary<string, MiniPool> poolDic = new Dictionary<string, MiniPool>();
            {
                for (int i = 0; i < IcicleFacade.Constants.TYPE.Length; i++)
                {
                    var poolObj = resouceContainer.GetGameObject(IcicleFacade.Constants.TYPE[i]);
                    var pool = new MiniPool();
                    pool.Init(poolObj, IcicleFacade.Constants.POOLING_SIZE);
                    poolDic.Add(IcicleFacade.Constants.TYPE[i], pool);
                }
            }

            builder.RegisterComponent(poolDic);
        }

        private void RegisterController(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<PlayerController>().AsSelf();
            builder.RegisterEntryPoint<IcicleController>().AsSelf();
            builder.RegisterEntryPoint<InGameUIController>().AsSelf();
        }

        private void RegisterFacade(IContainerBuilder builder)
        {
            builder.RegisterComponent(_player).AsImplementedInterfaces().AsSelf();
            builder.RegisterComponent(_ingameUI).AsImplementedInterfaces().AsSelf();
        }

        private void RegisterMessage(IContainerBuilder builder)
        {
            var option = builder.RegisterMessagePipe();

            builder.RegisterMessageBroker<DirectionButtonClick>(option);
            builder.RegisterMessageBroker<CountDownComplete>(option);
        }
    }
}