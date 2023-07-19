using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;

using MessagePipe;



namespace GameScene.Icicle
{
    public enum IcicleType { Basic, Super, Legend }

    public class IcicleController : IInitializable, IDisposable
    {
        [Inject] IObjectResolver _container;

        public IFactoryModelObservable Model { get { return _model; } }

        MiniPool[] _obstaclePools;
        FactoryModel _model = new FactoryModel();
        int rand;


        public void Initialize()
        {
            var poolDic = _container.Resolve<Dictionary<string, MiniPool>>();

            _obstaclePools = new MiniPool[]
            {
                poolDic[IcicleFacade.Constants.TYPE[(int)IcicleType.Basic]],
                poolDic[IcicleFacade.Constants.TYPE[(int)IcicleType.Super]],
                poolDic[IcicleFacade.Constants.TYPE[(int)IcicleType.Legend]],
            };
        }

        public void Dispose()
        {
            _model.Disposable();
            _model = null;
        }

        public void Spawn()
        {
            rand = UnityEngine.Random.Range(0, 3); //TODO : Ȯ������
            SetIcicle();
        }


        private void SetIcicle()
        {
            var facade = _obstaclePools[rand].Pop<IcicleFacade>();
            var id = Guid.NewGuid();
            facade.transform.position = facade.GetRandomPosition();
            facade.LinkContainer(_container);
            facade.Initialize(id);

            facade.Model.IsAlive.Subscribe(isAlive =>
            {
                if (isAlive == false)
                {
                    DespawnObstacle(facade.ID);
                }
            });

            _model._facadeDic.Add(id, facade);
        }

        public void DespawnObstacle(Guid id)
        {
            if (_model._facadeDic.TryGetValue(id, out var facade))
            {
                facade.Dispose();
                _obstaclePools[rand].push(facade.gameObject);
                _model._facadeDic.Remove(id);
            }
            else
            {
                Debug.LogWarning($"{id} isnt Here");
            }
        }

        public class FactoryModel : IFactoryModelObservable
        {
            public IObservable<Guid> GetAddedFacadeObservable()
            {
                return _facadeDic.ObserveAdd().Select(item => item.Key);
            }

            public IObservable<Guid> GetRemoveFacadeModelId()
            {
                return _facadeDic.ObserveRemove().Select(item => item.Key);
            }

            public IObservable<int> ObserveCount()
            {
                return _facadeDic.ObserveCountChanged();
            }

            public IcicleFacade.IFacadeModelObservable GetFacadeObservable(Guid id)
            {
                return _facadeDic[id].Model;
            }

            public void Disposable()
            {
                _facadeDic.Dispose();
            }

            public ReactiveDictionary<Guid, IcicleFacade> _facadeDic = new ReactiveDictionary<Guid, IcicleFacade>();
        }
    }

    public interface IFactoryModelObservable
    {
        public IObservable<Guid> GetAddedFacadeObservable();
        public IObservable<Guid> GetRemoveFacadeModelId();
        public IObservable<int> ObserveCount();
        public IcicleFacade.IFacadeModelObservable GetFacadeObservable(Guid id);
    }
}

