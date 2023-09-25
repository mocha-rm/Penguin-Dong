using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Environment
{
    public class EnvironmentFacade : BaseFacade
    {
        [SerializeField] Transform _obstacleSpawnTrans;
        [SerializeField] Transform _groundTrans;
        //[SerializeField] List<WarpComponent> _warpComponentList;


        Collider2D _spawnCollider;

        public Vector3 GetRandomObstacleSpawnPos()
        {
            var bound = _spawnCollider.bounds;
            return new Vector3(Random.Range(bound.min.x, bound.max.x), Random.Range(bound.min.y, bound.max.y), 0f);
        }

        public float GetGroundY()
        {
            //5.4 + 0.6 = 6.0
            return _groundTrans.position.y + 0.6f;
        }

        public override void Initialize()
        {
            _spawnCollider = _obstacleSpawnTrans.GetComponent<Collider2D>();

            //_warpComponentList.ForEach(warp => warp.Init());

        }

        public override void Dispose()
        {
        }
    }

}
