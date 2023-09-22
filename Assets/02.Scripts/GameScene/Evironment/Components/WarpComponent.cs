using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx.Triggers;
using UniRx;

namespace GameScene.Environment
{
    public class WarpComponent : MonoBehaviour, IDisposable
    {
        [SerializeField] Transform _firstTrans;
        [SerializeField] Transform _secondTrans;

        bool CanWarp
        {
            get => _isWarpingFirst == false && _isWarpingSecond == false;
        }

        bool _isWarpingFirst;
        bool _isWarpingSecond;

        CompositeDisposable _disposables;

        public void Init()
        {
            _disposables = new CompositeDisposable();

            _isWarpingFirst = false;
            _isWarpingSecond = false;

            _firstTrans.OnTriggerEnter2DAsObservable()
                .Where(col => CanWarp)
                .Where(col => col.attachedRigidbody != null)
                .Subscribe(col =>
                {
                    if (col.attachedRigidbody.TryGetComponent<BaseFacade>(out var facade))
                    {
                        var warpable = facade as IWarpAble;
                        if (warpable != null)
                        {
                            _isWarpingSecond = true;
                            warpable.ToWarp(_secondTrans.position);
                        }
                    }
                }).AddTo(_disposables);

            _secondTrans.OnTriggerEnter2DAsObservable()
                .Where(col => CanWarp)
                .Where(col => col.attachedRigidbody != null)
                .Subscribe(col =>
                {
                    if (col.attachedRigidbody.TryGetComponent<BaseFacade>(out var facade))
                    {
                        var warpable = facade as IWarpAble;
                        if (warpable != null)
                        {
                            _isWarpingFirst = true;
                            warpable.ToWarp(_firstTrans.position);
                        }
                    }
                }).AddTo(_disposables);


            _firstTrans.OnTriggerExit2DAsObservable()
                .Where(col => _isWarpingFirst)
                .Where(col => col.attachedRigidbody != null)
                .Subscribe(col =>
                {
                    if (col.attachedRigidbody.TryGetComponent<BaseFacade>(out var facade))
                    {
                        var warpable = facade as IWarpAble;
                        if (warpable != null)
                        {
                            _isWarpingFirst = false;
                        }
                    }
                }).AddTo(_disposables);

            _secondTrans.OnTriggerExit2DAsObservable()
                .Where(col => _isWarpingSecond)
                .Where(col => col.attachedRigidbody != null)
                .Subscribe(col =>
                {
                    if (col.attachedRigidbody.TryGetComponent<BaseFacade>(out var facade))
                    {
                        var warpable = facade as IWarpAble;
                        if (warpable != null)
                        {
                            _isWarpingSecond = false;
                        }
                    }
                }).AddTo(_disposables);

        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _disposables?.Clear();
            _disposables = null;
        }
    }


    public interface IWarpAble
    {
        public void ToWarp(Vector3 position);
    }
}
