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



public class ProjectLifeScope : LifetimeScope
{
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

    private void Start()
    {
        Container.Resolve<SceneLoader>().LoadScene(SceneLoader.SceneName.LobbyScene).Forget();
    }
    #endregion


    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);

        builder.Register<SceneLoader>(Lifetime.Singleton);
        //기본적으로 게임에 들어가는 Manager들 Sound , DB , etc...
    }
}
