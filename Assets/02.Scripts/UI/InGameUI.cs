using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MessagePipe;
using VContainer;
using VContainer.Unity;


public class InGameUI : MonoBehaviour, IInitializable, System.IDisposable
{
    [Inject] IObjectResolver _container;

    [Header("PlayerControl")]
    [SerializeField] private Button directionControlBtn;

    [Header("CountDown")]
    private int count; //TODO : Make CountSystem;


    IPublisher<DirectionButtonClick> _playerTurnPub;

    public void Dispose()
    {
        directionControlBtn.onClick.RemoveAllListeners();
    }

    public void Initialize()
    {
        _playerTurnPub = _container.Resolve <IPublisher<DirectionButtonClick>>();


        directionControlBtn.OnClickAsObservable()
             .Subscribe(_ =>
             {
                 //Debug.Log("Click");
                 _playerTurnPub.Publish(new DirectionButtonClick() 
                 { 
                    
                 });
                 
             }).AddTo(this.gameObject);
    }

    
}
