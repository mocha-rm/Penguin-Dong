using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using MessagePipe;
using VContainer.Unity;



public class CrashTestScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        RegisterRule(builder);
        RegisterController(builder);
        RegisterFacade(builder);
        RegisterMessage(builder);
    }

    void RegisterRule(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<CrashSceneRule>(Lifetime.Singleton).AsSelf();
    }

    void RegisterController(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<PL_Controller>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<OJ_Controller>(Lifetime.Singleton).AsSelf();
    }

    void RegisterFacade(IContainerBuilder builder)
    {
        builder.RegisterByHierarchy<PL_Facade>(null, Hierarchy.PL_Facade);
        builder.RegisterByHierarchy<OJ_Facade>(null, Hierarchy.OJ_Facade);
    }

    void RegisterMessage(IContainerBuilder builder)
    {
        var option = builder.RegisterMessagePipe();

        builder.RegisterMessageBroker<CrashEvent>(option);
    }



    public static class Hierarchy
    {
        public static readonly string PL_Facade = "Player";
        public static readonly string OJ_Facade = "FallingObject";
    }
}
