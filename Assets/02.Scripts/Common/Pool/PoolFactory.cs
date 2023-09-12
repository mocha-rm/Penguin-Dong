using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

using UniRx;


    public class PoolFactory : IInitializable, IDisposable
    {
        [Inject] IObjectResolver _container;
        Dictionary<string, MiniPool> _poolDic = new Dictionary<string, MiniPool>();

        public void Initialize()
        {
            Debug.Log("Initialize Factory!!");
            var selfEnds = _container.Resolve<IEnumerable<ISelfEndPoolable>>();

            foreach(var self in selfEnds)
            {
                var selfId = self.PoolId;
                var selfObj = self.gameObject;

                self.OnEndAsObservable().Subscribe(_ =>
                {
                    PushObject(selfId, selfObj);
                });
            }    
        }



        public void Dispose()
        {
            foreach (var pool in _poolDic.Values)
            {
                pool.Dispose();
            }

            _poolDic.Clear();
        }

        public MiniPool GetPool(string id)
        {
            if (_poolDic.TryGetValue(id, out var pool))
            {
                return pool;
            }
            else
            {
                Debug.LogError($"{id} is Not Regist");
                return null;
            }
        }

        public bool PushObject(string id, GameObject obj)
        {
            if(_poolDic.TryGetValue(id, out var pool))
            {
                pool.push(obj);
                return true;
            }

            return false;
        }

        public MiniPool AddPool(string poolId, GameObject original, int poolSize)
        {
            if(_poolDic.ContainsKey(poolId))
            {
                Debug.LogWarning($"{poolId} is Already Added");
                return null;
            }

            var pool = new MiniPool();
            pool.Init(original, poolSize);

            _poolDic.Add(poolId, pool);
            return pool;
        }

        public class PoolModel
        {
            public string poolId;
            public string resourceId;
            public int poolSize;
        }
    }

    //BaseFacade ���� (Interface�� Resolve �ϴ� �� ������ BaseFacade�� �������� �Ѵ�)
    public interface ISelfEndPoolable
    {
        public GameObject gameObject { get; }
        public string PoolId { get; }
        public IObservable<Unit> OnEndAsObservable();
    }


    public static partial class RegisterExtension
    {

        public static PoolFactory RegisterPoolFactory(this IContainerBuilder builder)
        {
            var poolFactory_ = new PoolFactory();

            builder.Register(Container =>
            {
                Container.Inject(poolFactory_);
                return poolFactory_;
            }, Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.RegisterFactory<string, MiniPool>(container => container.Resolve<PoolFactory>().GetPool, Lifetime.Singleton);
            return poolFactory_;
        }

        public static void RegisterPool<T>(this IContainerBuilder builder, BaseResourceContainer resources, PoolFactory factory, PoolFactory.PoolModel poolInfo) where T : UnityEngine.Component
        {
            var originalObj = resources.GetGameObject(poolInfo.resourceId);
            var miniPool = factory.AddPool(poolInfo.poolId, originalObj, poolInfo.poolSize);

            var originalType = originalObj.GetComponent<T>();
            var totalPoolObject = miniPool.AllObjects;

            if (originalType is BaseFacade)
            {
                foreach (var objInPool in totalPoolObject)
                {
                    builder.Register<T>(Container =>
                    {
                        var component = objInPool.GetComponent<T>();
                        Container.Inject(component);
                        return component;
                    }, Lifetime.Scoped).AsImplementedInterfaces();
                }
            }
            else
            {
                foreach (var objInPool in totalPoolObject)
                {
                    builder.Register<T>(Container => objInPool.GetComponent<T>(), Lifetime.Scoped);
                }
            }

            if (originalType is IRegistMonobehavior)
            {
                foreach (var objInPool in totalPoolObject)
                {
                    (objInPool.GetComponent<T>() as IRegistMonobehavior).RegistBehavior(builder);
                }
            }
        }

        public static MiniPool ResolveMiniPool(this IObjectResolver container, string poolId)
        {
            var getPoolFunc = container.Resolve<Func<string, MiniPool>>();

            if(getPoolFunc != null)
            {
               return getPoolFunc(poolId);
            }

            Debug.LogError("There are no PoolFactory");
            return null;
        }
    }

