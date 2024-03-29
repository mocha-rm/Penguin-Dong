using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;
using UniRx.Triggers;
using MessagePipe;
using GameScene.Message;

using GameScene.Player;

namespace GameScene.Obstacle
{
    public class ObstacleFacade : BaseFacade, IRegistMonobehavior
    {
        public Guid ID { get => _id; }

        public IObstacleModel Model
        {
            get
            {
                if (_model == null)
                {
                    _model = CreateModel();
                }
                return _model;
            }
        }
        BLOC _bloc;
        AudioService _audioService;

        Guid _id;
        FacadeModel _model = null;
        float _endOfY;

        Rigidbody2D _rigid;
        CircleCollider2D _collider;
        Animator _ani;
        Coroutine _aniRoutine = null;

        CompositeDisposable _disposables;

        IPublisher<ScoreUpEvent> _scoreUpPub;


        public void RegistBehavior(IContainerBuilder builder)
        {
            _ani = GetComponent<Animator>();
            _rigid = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CircleCollider2D>();
        }

        public override void Initialize()
        {
            Init(Guid.Empty, Vector3.zero, 0f);
            Debug.Log("Initialize!");
        }

        public void Init(Guid id, Vector3 pos, float endOfY)
        {
            _bloc = _container.Resolve<BLOC>();
            _audioService = _container.Resolve<AudioService>();
            _scoreUpPub = _container.Resolve<IPublisher<ScoreUpEvent>>();

            if (_model == null)
            {
                _model = CreateModel();
            }

            _id = id;
            transform.position = pos;
            _rigid.constraints = RigidbodyConstraints2D.None;
            _rigid.gravityScale = GetFeeze();
            _collider.enabled = true;
            _endOfY = endOfY;
            _disposables?.Dispose();
            _disposables?.Clear();
            _disposables = new CompositeDisposable();

            Observable.EveryUpdate()
                .Where(_ => this.gameObject.activeInHierarchy)
                .Where(_ => _model.IsAlive.Value == true)
                .Subscribe(_ =>
                {
                    if (transform.position.y < _endOfY && _bloc.GameModel.GameStateProperty.Value == Rule.GameState.Playing
                    && Time.timeScale != 0)
                    {
                        _scoreUpPub.Publish(new ScoreUpEvent()
                        {

                        });

                        ExplosionAnim();
                    }
                }).AddTo(_disposables);
        }

        public override void Dispose()
        {
            _model?.Dispose();
            _model = null;

            _disposables?.Dispose();
            _disposables = null;
        }

        public void ExplosionAnim()
        {
            _rigid.constraints = RigidbodyConstraints2D.FreezePositionY;

            _collider.enabled = false;

            _ani.Play("Explosion");

            if (_aniRoutine != null)
            {
                StopCoroutine(_aniRoutine);
            }
            _aniRoutine = StartCoroutine(ExplosionAnimRoutine());
            //_audioService.Play(AudioService.AudioResources.Fire_Impact, AudioService.SoundType.SFX);
        }

        private IEnumerator ExplosionAnimRoutine()
        {
            var state = _ani.GetCurrentAnimatorStateInfo(0);

            yield return new WaitUntil(() => state.IsName("Explosion") == true && state.normalizedTime >= 1.0f);

            _model._isAlilve.Value = false;
        }

        private float GetFeeze()
        {
            if (_bloc.GameModel.AbilitiesProperty[AbilityNames.Freeze.ToString()] != 0)
            {
                return 0.3f;
            }

            return 1.0f;
        }




        FacadeModel CreateModel()
        {
            return new FacadeModel()
            {
                _isAlilve = new ReactiveProperty<bool>(true),
            };
        }


        public static class Constants
        {
            public static readonly string AddressableId = "ObstacleFacade";
            public static readonly string PoolPath = "ObstacleFacade";
            public static readonly string PoolId = "ObstacleFacade";
            public static readonly int PoolSize = 64;


            public static readonly float DamageAmount = 60f;
        }

        public static class Hierarchy
        {

        }

        public class FacadeModel : IObstacleModel
        {
            public IReadOnlyReactiveProperty<bool> IsAlive { get => _isAlilve; }

            public ReactiveProperty<bool> _isAlilve;

            public void Dispose()
            {
                _isAlilve?.Dispose();
            }
        }

        public interface IObstacleModel
        {
            public IReadOnlyReactiveProperty<bool> IsAlive { get; }
        }
    }
}
