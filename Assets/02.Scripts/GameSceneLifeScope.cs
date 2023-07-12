using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;

public class GameSceneLifeScope : LifetimeScope
{
    [Header("Facades")]
    [SerializeField] PlayerFacade _player;
    [SerializeField] InGameUI _ingameUI;


    //UI
    //+ UI Facades


    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        RegisterRule(builder);
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

    private void RegisterController(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<PlayerController>().AsSelf();
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

    }
}
