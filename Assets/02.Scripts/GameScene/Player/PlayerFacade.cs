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
using UnityEngine.EventSystems;

namespace GameScene.Player
{
    public class PlayerFacade : BaseFacade, IRegistMonobehavior, Environment.IWarpAble
    {
        BloC _bloc; //Through BLOC can Access GameState of GameRule.GameModel

        //Component (ºÎÇ°)
        PlayerBehaviour _pBehaviour;
        Rigidbody2D _rigid;

        //Model
        FacadeModel _model;



        public void RegistBehavior(IContainerBuilder builder)
        {
            _pBehaviour = transform.GetChild(0).GetComponent<PlayerBehaviour>();
            _rigid = GetComponent<Rigidbody2D>();
        }


        public override void Initialize()
        {
            _bloc = _container.Resolve<BloC>();

            if (_model == null)
            {
                _model = CreateModel();
            }

            _pBehaviour.Init(Constants.maxSpeed, Constants.originPos);
        }


        public override void Dispose()
        {
            _pBehaviour.Clear();

            _model?.Dispose();
            _model = null;
        }

        public void MoveAction()
        {
            _bloc.GameModel.GameStateProperty
                .Where(state => state == GameState.Playing)
                .Subscribe(action => _pBehaviour.Move());
        }

        public void DirectionControlAction()
        {
            _pBehaviour.SetDirection();
        }


        public static class Hierachy
        {
            
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

        public void ToWarp(Vector3 position)
        {
            var facadePos = this.transform.position;
            this.transform.position = new Vector3(position.x, facadePos.y, facadePos.z);
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


