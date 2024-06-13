using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using UniRx;
using GameScene.Environment;


namespace GameScene.Obstacle
{
    public class ObstacleController : IInitializable, IDisposable
    {
        Color[] _colors = new Color[]
        {
            new Color (1f, 1f, 1f, 1f),
            new Color (1f,0f,1f,1f),
            new Color (0f, 0.2f, 1f, 1f),
            new Color (0f, 1f, 0.9f, 1f),
            new Color (0f, 1f, 0f, 1f),
            new Color (0f, 0f, 0f, 1f),
        };

        public Color ObjectColor { get; set; } = new Color(1f, 1f, 1f, 1f);



        [Inject] IObjectResolver _container;

        public IObstacleControllerModel Model
        {
            get
            {
                return _model;
            }
        }

        EnvironmentFacade _environment;
        AudioService _audioService;
        

        MiniPool _obastaclePool;
        FactoryModel _model = new FactoryModel();

        public void Initialize()
        {
            Debug.Log("Initialize ObstacleController");

            _audioService = _container.Resolve<AudioService>();

            var poolGet = _container.Resolve<Func<string, MiniPool>>();
            _obastaclePool = poolGet(ObstacleFacade.Constants.PoolId);
            
            _environment = _container.Resolve<EnvironmentFacade>();

            var totalObstacles = _container.Resolve<IEnumerable<ObstacleFacade>>();

            Debug.Log(totalObstacles.Count());
            foreach (var obj in totalObstacles)
            {
                Debug.Log(obj.Model.IsAlive);
            }
        }
        public void Dispose()
        {
            Debug.Log("Dispose ObstacleController");
            _model.Disposable();
            _model = null;
        }

        public void SpawnObstacles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnObstacle();
            }
        }

        public void GetRandomColor() //Call when Level up // Init = Original Color
        {
            var color = _colors[UnityEngine.Random.Range(0, _colors.Length)];

            ObjectColor = color;
        }


        void SpawnObstacle()
        {
            var facade = _obastaclePool.Pop<ObstacleFacade>();
            
            var id = Guid.NewGuid();
            var pos = _environment.GetRandomObstacleSpawnPos();
            var endPosy = _environment.GetGroundY();            

            facade.Init(id, pos, endPosy);

            facade.SetRandomColor(ObjectColor);

            _audioService.StereoSetting(pos);

            facade.Model.IsAlive.Subscribe(bAlive =>
            {
                if (bAlive == false)
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
                _obastaclePool.push(facade.gameObject);
                _model._facadeDic.Remove(id);
            }
            else
            {
                Debug.LogWarning($"{id} isnt Here");
            }
        }



        public class FactoryModel : IObstacleControllerModel
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

            public ObstacleFacade.IObstacleModel GetFacadeObservable(Guid id)
            {
                return _facadeDic[id].Model;
            }

            public void Disposable()
            {
                _facadeDic.Dispose();
            }

            public ReactiveDictionary<Guid, ObstacleFacade> _facadeDic = new ReactiveDictionary<Guid, ObstacleFacade>();
        }
    }

    public interface IObstacleControllerModel
    {
        public IObservable<Guid> GetAddedFacadeObservable();
        public IObservable<Guid> GetRemoveFacadeModelId();
        public IObservable<int> ObserveCount();
        public ObstacleFacade.IObstacleModel GetFacadeObservable(Guid id);
    }
}
