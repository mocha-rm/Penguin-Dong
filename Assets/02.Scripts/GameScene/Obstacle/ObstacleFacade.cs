using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;
using UniRx.Triggers;
using MessagePipe;
using GameScene.Message;
using UnityEditor.VersionControl;
using UnityEngine.TextCore.Text;
using GameScene.Player;

namespace GameScene.Obstacle
{
    public class ObstacleFacade : BaseFacade, IRegistMonobehavior
    {
        public Guid ID { get => _id; }

        public IFacadeModelObservable Model
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


        Guid _id;
        FacadeModel _model = null;
        float _endOfY;

        CompositeDisposable _disposables;
        IPublisher<ObstacleCrashEvent> _colPub;


        public void RegistBehavior(IContainerBuilder builder)
        {

        }

        public override void Initialize()
        {
            Debug.Log("Initialize!");
            Init(Guid.Empty, Vector3.zero, 0f);
        }

        public void Init(Guid id, Vector3 pos, float endOfY)
        {
            if (_model == null)
            {
                _model = CreateModel();
            }

            _id = id;
            transform.position = pos;
            _endOfY = endOfY;
            _disposables?.Dispose();
            _disposables?.Clear();
            _disposables = new CompositeDisposable();
            _colPub = _container.Resolve<IPublisher<ObstacleCrashEvent>>();

            Observable.EveryUpdate()
                .Where(_ => this.gameObject.activeInHierarchy)
                .Where(_ => _model.IsAlive.Value == true)
                .Subscribe(_ =>
                {
                    if (transform.position.y < _endOfY)
                    {
                        _colPub.Publish(new ObstacleCrashEvent()
                        {
                            Character = null,
                            Obstacle = this,
                        });

                        _model._isAlilve.Value = false;
                    }
                }).AddTo(_disposables);

            this.gameObject.OnTriggerEnter2DAsObservable()
                .Where(_ => this.gameObject.activeInHierarchy)
                .Subscribe(collision =>
                {
                    if (collision.attachedRigidbody != null)
                    {
                        if (collision.attachedRigidbody.TryGetComponent<PlayerFacade>(out var character))
                        {
                            _colPub.Publish(new ObstacleCrashEvent()
                            {
                                Character = character,
                                Obstacle = this,
                            });

                            _model._isAlilve.Value = false;
                        }
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

        public class FacadeModel : IFacadeModelObservable
        {
            public IReadOnlyReactiveProperty<bool> IsAlive { get => _isAlilve; }

            public ReactiveProperty<bool> _isAlilve;

            public void Dispose()
            {
                _isAlilve?.Dispose();
            }
        }

        public interface IFacadeModelObservable
        {
            public IReadOnlyReactiveProperty<bool> IsAlive { get; }
        }
    }
}
