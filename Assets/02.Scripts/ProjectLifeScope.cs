using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VContainer.Diagnostics;

using MessagePipe;
using MessagePipe.VContainer;
using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;


public enum SceneName
{
    LobbyScene,
    GameScene,
    None,
}

public class ProjectLifeScope : LifetimeScope
{
    readonly SceneName StartScene = SceneName.LobbyScene;
    #region SingleScope


    static ProjectLifeScope _instance;
    protected override void Awake()
    {
        base.Awake();

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private async void Start()
    {
        var sceneService = Container.Resolve<SceneService>();

        await sceneService.LoadPreload();

        Container.Resolve<SceneService>().LoadScene(StartScene).Forget();
    }

    public static IObjectResolver GetProjectContainer()
    {
        if (_instance == null)
        {
            return null;
        }
        return _instance.Container;
    }
    #endregion


    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);

        //Proejct Services
        builder.Register<SceneService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<AudioService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
    }
}
