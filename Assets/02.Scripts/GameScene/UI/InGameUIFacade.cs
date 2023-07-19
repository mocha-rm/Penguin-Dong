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
    public class InGameUIFacade : MonoBehaviour, IInitializable, System.IDisposable
    {
        [Inject] IObjectResolver _container;


        //부품들
        [Header("PlayerControl")]
        [SerializeField] Button _directionControlBtn;
        IPublisher<DirectionButtonClick> _playerTurnPub;


        [Header("CountDown")]
        [SerializeField] TextMeshProUGUI _count;
        private int _countNum;
        IPublisher<CountDownComplete> _countdownPub;

        [Header("Info Print")]
        private TextMeshProUGUI _score;
        private TextMeshProUGUI _coin;
        private TextMeshProUGUI _life;

        [Header("Game Control")] //다른 Facade로 분리예정
        private GameObject _gameOverPanel;
       /* [SerializeField] Button _home;
        [SerializeField] Button _retry;*/




        public void Dispose()
        {
            _directionControlBtn.onClick.RemoveAllListeners();
        }

        public void Initialize()
        {
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

        public static class Constants
        {
            public static readonly int countNumber = 3;
        }

        public static class Hierarchy
        {

        }

        public class FacadeModel
        {

        }
    }
}

