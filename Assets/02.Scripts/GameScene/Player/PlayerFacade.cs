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
        enum Tag
        {
            Player,
            Invulnerable
        }


        public IPlayerModel Model
        {
            get
            {
                return _model;
            }
        }


        BLOC _bloc; //Through BLOC can Access GameState of GameRule.GameModel

        //Component (��ǰ)
        PlayerBehaviour _pBehaviour;

        //Model
        FacadeModel _model;

        Coroutine _routine = null;



        public void RegistBehavior(IContainerBuilder builder)
        {
            try
            {
                _pBehaviour = transform.GetChild(0).GetComponent<PlayerBehaviour>();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message + "PlayerFacade RegisterBehavior error");
            }
        }


        public override void Initialize()
        {
            _bloc = _container.Resolve<BLOC>();

            if (_model == null)
            {
                _model = CreateModel();
            }

            _pBehaviour.Init(Constants.maxSpeed, Constants.originPos);

            _model._isInvul.AsObservable()
                .Subscribe(_ =>
                {
                    if (_model._isInvul.Value == true)
                    {
                        gameObject.layer = 8;
                        _pBehaviour.gameObject.layer = 8;

                        gameObject.tag = Tag.Invulnerable.ToString();
                        _pBehaviour.gameObject.tag = Tag.Invulnerable.ToString();
                    }
                    else
                    {
                        gameObject.layer = 6;
                        _pBehaviour.gameObject.layer = 6;

                        gameObject.tag = Tag.Player.ToString();
                        _pBehaviour.gameObject.tag = Tag.Player.ToString();
                    }
                }).AddTo(this.gameObject);
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
            if (_model._isInvul.Value == false)
            {
                if (_bloc.GameModel.GameStateProperty.Value == GameState.GameOver)
                {
                    _pBehaviour.GameOverAnimate();
                }
                else
                {
                    _model._isInvul.Value = true;

                    if (_routine != null)
                    {
                        StopCoroutine(_routine);
                    }

                    _routine = StartCoroutine(InvulnerableEnd());
                }
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
            public static readonly float maxSpeed = 5;
            public static readonly Vector3 originPos = Vector3.zero;
            public static readonly float InvulnerableTime = 2.5f;
        }


        private static FacadeModel CreateModel()
        {
            return new FacadeModel()
            {
                _isInvul = new ReactiveProperty<bool>(false),
            };
        }

        public void ToWarp(Vector3 position)
        {
            var facadePos = this.transform.position;
            this.transform.position = new Vector3(position.x, facadePos.y, facadePos.z);
        }

        public class FacadeModel : IPlayerModel
        {
            public ReactiveProperty<bool> _isInvul;

            public IReadOnlyReactiveProperty<bool> Isinvul { get => _isInvul; }

            public void Dispose()
            {
                _isInvul?.Dispose();
            }
        }
    }

    public interface IPlayerModel
    {
        public IReadOnlyReactiveProperty<bool> Isinvul { get; }
    }
}


