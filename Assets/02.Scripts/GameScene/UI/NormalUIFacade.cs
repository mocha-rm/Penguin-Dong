using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MessagePipe;
using VContainer;
using VContainer.Unity;
using GameScene.Rule;
using TMPro;
using Cysharp.Threading.Tasks;
using GameScene.Message;


namespace GameScene.UI
{
    public class NormalUIFacade : MonoBehaviour, IInitializable, IDisposable
    {
        [Inject] IObjectResolver _container;


        [Header("PlayerControl")]
        [SerializeField] Button _directionControlBtn;
        IPublisher<DirectionButtonClick> _playerTurnPub;

        [Header("CountDown")]
        [SerializeField] TextMeshProUGUI _count;
        private int _countNum;
        IPublisher<CountDownComplete> _countdownPub;

        [Header("Info Output")]
        private TextMeshProUGUI _score;
        private TextMeshProUGUI _life;

        FacadeModel _model;


        public void Initialize()
        {
            if (_model == null)
            {
                _model = CreateModel();
            }

            _playerTurnPub = _container.Resolve<IPublisher<DirectionButtonClick>>();

            _countdownPub = _container.Resolve<IPublisher<CountDownComplete>>();

            _directionControlBtn.OnClickAsObservable()
                 .Subscribe(_ =>
                 {
                     _playerTurnPub.Publish(new DirectionButtonClick()
                     {

                     });

                 }).AddTo(this.gameObject);


            StartCount().Forget();


            _model.life.Subscribe(life =>
            {
                _life.text = $"잔여 목숨 : {life}";
            }).AddTo(this);

            _model.score.Subscribe(score =>
            {
                _score.text = $"Score : {score}";
            }).AddTo(this);
        }

        private async UniTaskVoid StartCount()
        {
            _countNum = Constants.countNumber;

            while (_countNum > 0)
            {
                --_countNum;
                _count.text = _countNum.ToString();
                await UniTask.Delay(System.TimeSpan.FromMilliseconds(1000));
            }

            _countdownPub.Publish(new CountDownComplete() //카운트컴플리트 이벤트 퍼블리싱
            {

            });

            Debug.Log($"{_countNum} Count Ended");

            _count.gameObject.SetActive(false);
        }

        private FacadeModel CreateModel()
        {
            return new FacadeModel()
            {
                score = new ReactiveProperty<int>(0),
                life = new ReactiveProperty<int>(Constants.lifeCount),
            };
        }


        public void Dispose()
        {
            _directionControlBtn.onClick.RemoveAllListeners();
            _model?.Dispose();
            _model = null;
        }



        public static class Constants
        {
            public static readonly int countNumber = 3;
            public static readonly int lifeCount = 3;
        }



        public class FacadeModel : IFacadeModelObservable
        {
            public ReactiveProperty<int> score;
            public ReactiveProperty<int> life;

            public IReadOnlyReactiveProperty<int> Life { get { return life; } }

            public IReadOnlyReactiveProperty<int> Score { get { return score; } }



            public void Dispose()
            {
                score?.Dispose();
                life?.Dispose();
            }
        }
    }


    public interface IFacadeModelObservable
    {
        public IReadOnlyReactiveProperty<int> Life { get; }

        public IReadOnlyReactiveProperty<int> Score { get; }
    }
}

