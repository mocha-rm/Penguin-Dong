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
        ShieldBehaviour _shield;
        //ReactiveProperty<int> _shieldCount;

        //Model
        FacadeModel _model;

        Coroutine _routine = null;

        IPublisher<ObstacleCrashEvent> _colPub;



        public void RegistBehavior(IContainerBuilder builder)
        {
            try
            {
                _pBehaviour = transform.GetChild(0).GetComponent<PlayerBehaviour>();
                _shield = _pBehaviour.transform.GetChild(0).GetComponent<ShieldBehaviour>();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message + "PlayerFacade RegisterBehavior error");
            }
        }


        public override void Initialize()
        {
            _bloc = _container.Resolve<BLOC>();

            _colPub = _container.Resolve<IPublisher<ObstacleCrashEvent>>();

            if (_model == null)
            {
                _model = CreateModel();
            }

            _pBehaviour.Init(Constants.maxSpeed, Constants.originPos);

            _shield.Init();

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


            Observable.EveryFixedUpdate().Where(_ => _pBehaviour.IsCrashed == true)
                .Subscribe(_ =>
                {
                    _pBehaviour.IsCrashed = false;

                    _colPub.Publish(new ObstacleCrashEvent()
                    {

                    });
                });

            _model._shieldCount.AsObservable().Where(_ => _model._isShieldHave.Value == true).
                Subscribe(_ =>
            {
                if(_model._shieldCount.Value <= 0)
                {
                    _shield.ExplosionAction();
                    _model._isShieldHave.Value = false;
                }
            });
        }


        public override void Dispose()
        {
            _pBehaviour.Clear();
            _model?.Dispose();
            _model = null;
        }

        public void IdleAction()
        {
            transform.position = Constants.originPos;
            _pBehaviour.Idle();
        }

        public void MoveAction()
        {
            _pBehaviour.Move();
        }

        public void DirectionControlAction()
        {
            float icyValue = _bloc.GameModel.AbilitiesProperty[AbilityNames.Shoes.ToString()];
            _pBehaviour.SetDirection(icyValue);
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

        public void InvulStatus_Roguelike(bool isActivate)
        {
            _model._isInvul.Value = isActivate;
        }


        public void ShieldActivate(int count)
        {
            _shield.gameObject.SetActive(true);
            _model._shieldCount.Value = count;
            _model._isShieldHave.Value = true;
        }

        public void ShieldDamage()
        {
            if(_model._isShieldHave.Value == true)
            {
                _model._shieldCount.Value -= 1;
                _shield.HittedParticleAction();
            }
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
                _shieldCount = new ReactiveProperty<int>(0),
                _isInvul = new ReactiveProperty<bool>(false),
                _isShieldHave = new ReactiveProperty<bool>(false)
            };
        }

        public void ToWarp(Vector3 position)
        {
            var facadePos = this.transform.position;
            this.transform.position = new Vector3(position.x, facadePos.y, facadePos.z);
        }

        public class FacadeModel : IPlayerModel
        {
            public ReactiveProperty<int> _shieldCount;

            public ReactiveProperty<bool> _isInvul;

            public ReactiveProperty<bool> _isShieldHave;

            public IReadOnlyReactiveProperty<int> ShieldCount { get => _shieldCount; }

            public IReadOnlyReactiveProperty<bool> Isinvul { get => _isInvul; }

            public IReadOnlyReactiveProperty<bool> IsShieldHave { get => _isShieldHave; }

            public void Dispose()
            {
                _shieldCount?.Dispose();
                _isInvul?.Dispose();
                _isShieldHave?.Dispose();
            }
        }
    }

    public interface IPlayerModel
    {
        public IReadOnlyReactiveProperty<int> ShieldCount { get; }
        public IReadOnlyReactiveProperty<bool> Isinvul { get; }
        public IReadOnlyReactiveProperty<bool> IsShieldHave { get; }
    }
}


