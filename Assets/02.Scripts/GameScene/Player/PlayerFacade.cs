using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;
using System;
using MessagePipe;
using GameScene.Rule;
using GameScene.Message;

namespace GameScene.Player
{
    public class PlayerFacade : MonoBehaviour, IInitializable, IDisposable
    {
        BloC _bloc;

        //플레이어

        [Inject] IObjectResolver _container;

        //Component (부품)
        [SerializeField] PlayerBehaviour _pBehaviour;

        //Model
        private FacadeModel _model;


        public void Initialize()
        {
            _bloc = _container.Resolve<BloC>();

            if (_model == null)
            {
                _model = CreateModel();
            }

            _pBehaviour.Init(Constants.maxSpeed, Constants.originPos);
        }


        public void Dispose()
        {
            _pBehaviour.Clear();

            _model?.Dispose();
            _model = null;
        }

        public void MoveAction()
        {
            _bloc.GameModelObservable.GameStateProperty
                .Where(state => state == GameState.Playing)
                .Subscribe(action => _pBehaviour.Move());
        }

        public void DirectionControlAction()
        {
            _pBehaviour.SetDirection();
        }


        public static class Hierachy
        {
            //Hierachy InspectorView 에 나타낼 항목들
        }

        public static class Constants
        {
            public static readonly int life = 3;
            public static readonly float maxSpeed = 5;
            public static readonly Vector3 originPos = Vector3.zero;
        }


        private static FacadeModel CreateModel()
        {
            return new FacadeModel()
            {
                _life = new ReactiveProperty<int>(Constants.life)
            };
        }

        public class FacadeModel : IFacadeModelObservable
        {
            public ReactiveProperty<int> _life;

            public IReadOnlyReactiveProperty<int> Life { get => _life; }

            public void Dispose()
            {
                _life?.Dispose();
            }
        }
    }

    public interface IFacadeModelObservable
    {
        public IReadOnlyReactiveProperty<int> Life { get; }
    }
}


