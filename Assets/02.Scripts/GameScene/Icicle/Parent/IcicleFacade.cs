using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;
using UniRx.Triggers;



namespace GameScene.Icicle
{
    public abstract class IcicleFacade : MonoBehaviour, IDisposable
    {
        public Guid ID { get { return _id; } }

        public IFacadeModelObservable Model { get { return _model; } }

        IObjectResolver _container;

        Guid _id;

        FacadeModel _model;

        protected Animator _ani;

        protected Rigidbody2D _rigid;



        public void LinkContainer(IObjectResolver container) => _container = container;

        public void Initialize(Guid id)
        {
            if (_model == null)
            {
                _model = CreateModel();
            }

            _id = id;

            _ani = GetComponent<Animator>();

            _rigid = GetComponent<Rigidbody2D>();
            _rigid.gravityScale = _model._gravity.Value;

            this.gameObject.OnTriggerEnter2DAsObservable().Where(_ => gameObject.activeInHierarchy)
                .Subscribe(_ =>
                {
                    if (_.tag == "Player")
                    {
                        this.gameObject.SetActive(false);
                    }

                    if (_.tag == "Ground")
                    {
                        this.gameObject.SetActive(false);
                    }
                }).AddTo(this);
        }

        public void Dispose()
        {
            _model.Dispose();
            _model = null;
        }

        protected virtual FacadeModel CreateModel(float gr = 1.0f)
        {
            return new FacadeModel()
            {
                _isAlive = new ReactiveProperty<bool>(true),
                _gravity = new ReactiveProperty<float>(gr),
            };
        }

        public Vector3 GetRandomPosition()
        {
            Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0.0f, 1.0f), 1.1f, 0f));
            pos.z = 0.0f;

            return pos;
        }

        public static class Constants
        {
            public static readonly string[] TYPE = new string[]
            {
                "BasicIcicle",
                "SuperIcicle",
                "LegendIcicle",
            };
            public static readonly int POOLING_SIZE = 50;
        }

        public class FacadeModel : IFacadeModelObservable
        {
            public ReactiveProperty<bool> _isAlive;
            public ReactiveProperty<float> _gravity;

            public IReadOnlyReactiveProperty<bool> IsAlive { get => _isAlive; }
            public IReadOnlyReactiveProperty<float> Gravity { get => _gravity; }



            public void Dispose()
            {
                _isAlive?.Dispose();
                _gravity?.Dispose();
            }
        }

        public interface IFacadeModelObservable
        {
            public IReadOnlyReactiveProperty<bool> IsAlive { get; }
            public IReadOnlyReactiveProperty<float> Gravity { get; }
        }
    }
}

