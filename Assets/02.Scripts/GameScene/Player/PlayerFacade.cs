using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using GameScene.Rule;
using GameScene.Message;




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

        Coroutine _routine = null;



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
            _pBehaviour.Move();
        }

        public void DirectionControlAction()
        {
            _pBehaviour.SetDirection();
        }

        public void IsInvulnerable() //1
        {
            if(_model._isInvul.Value == false)
            {
                _model._isInvul.Value = true;

                if (_routine != null)
                {
                    StopCoroutine(_routine);
                }

                _routine = StartCoroutine(InvulnerableEnd());
            }
        }

        private IEnumerator InvulnerableEnd() //2
        {
            _pBehaviour.Crashed();

            yield return new WaitForSeconds(Constants.InvulnerableTime);

            _pBehaviour.BodyColorSwitch();

            _model._isInvul.Value = false;
        }



        public static class Constants
        {
            public static readonly int life = 3;
            public static readonly float maxSpeed = 5;
            public static readonly Vector3 originPos = Vector3.zero;
            public static readonly float InvulnerableTime = 2.5f;
        }


        private static FacadeModel CreateModel()
        {
            return new FacadeModel()
            {
                _life = new ReactiveProperty<int>(Constants.life),
                _isInvul = new ReactiveProperty<bool>(false),
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

            public ReactiveProperty<bool> _isInvul;

            public IReadOnlyReactiveProperty<int> Life { get => _life; }

            public IReadOnlyReactiveProperty<bool> Isinvul { get => _isInvul; }

            public void Dispose()
            {
                _life?.Dispose();
                _isInvul?.Dispose();
            }
        }
    }

    public interface IFacadeModelObservable
    {
        public IReadOnlyReactiveProperty<int> Life { get; }

        public IReadOnlyReactiveProperty<bool> Isinvul { get; }
    }
}


