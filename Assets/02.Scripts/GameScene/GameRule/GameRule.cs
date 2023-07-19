using System;
using UnityEngine;

using VContainer;
using VContainer.Unity;

using MessagePipe;
using UniRx;
using Cysharp.Threading.Tasks;
using GameScene.Player;
using GameScene.UI;
using GameScene.Icicle;

namespace GameScene.Rule
{
    public enum GameState { Waiting, Playing, GameOver }

    public partial class GameRule : IInitializable, IDisposable
    {
        public IGameModelObserver Model { get { return _model; } }

        GameModel _model;

        [Inject] IObjectResolver _container;


        //Controller
        PlayerController _playerController;
        IcicleController _icicleController;
        InGameUIController _uiController;

        //Sub
        IDisposable _disposable;



        public void Initialize()
        {
            _playerController = _container.Resolve<PlayerController>();
            _icicleController = _container.Resolve<IcicleController>();
            _uiController = _container.Resolve<InGameUIController>();

            if (_model == null)
            {
                _model = CreateModel();

            }

            GameRunning().Forget();

            //ISubScriber
            var bag = DisposableBag.CreateBuilder();

            SubscribePlayerTurnEvent().AddTo(bag);

            SubscribeCountDownCompleteEvent().AddTo(bag);

            _disposable = bag.Build();
        }

        private async UniTaskVoid GameRunning()
        {
            int score = 0;

            await UniTask.WaitUntil(() => _model.GameState.Value == GameState.Playing);

            do
            {
                _icicleController.Spawn();
                score++;

                await UniTask.Delay(TimeSpan.FromMilliseconds(Constants.Difficulty));

                if (_model.GameState.Value == GameState.GameOver)
                {
                    Debug.Log(score.ToString());
                    Debug.Log($"Game is Over");
                    break;
                }

                //������Ʈ ����(������Ʈ ������ �Ҷ� ���� �������� Ȯ���� ����)
                //���� ���ھ� �̻� �޼��ϸ� ����� ������Ʈ�� ���� Ȯ�� ���

            } while (true);
        }



        public void Dispose()
        {
            _disposable?.Dispose();
            _model?.Dispose();
            _model = null;
        }


        private GameModel CreateModel()
        {
            return new GameModel()
            {
                Score = new ReactiveProperty<int>(0),
                GameState = new ReactiveProperty<GameState>(GameState.Waiting),
            };
        }

        public static class Constants
        {
            public static readonly double Difficulty = 500;
        }
    }
}

