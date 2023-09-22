
using MessagePipe;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;



public class OJ_Facade : BaseFacade, IRegistMonobehavior
{
    IPublisher<CrashEvent> _crashPub;
    CompositeDisposable _disposable;

    public void RegistBehavior(IContainerBuilder builder)
    {
        
    }

    public override void Initialize()
    {
        _crashPub = _container.Resolve<IPublisher<CrashEvent>>();

        _disposable = new CompositeDisposable();


        this.gameObject.OnTriggerEnter2DAsObservable()
            .Where(_ => this.gameObject.activeInHierarchy)
            .Subscribe(collision =>
            {
                if(collision.attachedRigidbody != null)
                {
                    if(collision.tag == "Player")
                    {
                        _crashPub.Publish(new CrashEvent()
                        {
                           
                        });
                    }
                    else
                    {
                        //call private method for nonactive
                        Debug.Log("Just Play Animation");
                        Debug.Log("Add Round Score here?");
                    }
                }
            }).AddTo(_disposable);
    }
    public override void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }

    public void CrashedAction()
    {
        gameObject.SetActive(false);
        Debug.Log("Object Crash Action");
    }

}
