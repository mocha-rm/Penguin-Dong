using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


    public class MiniPool : IDisposable
    {
        public IEnumerable<GameObject> AllObjects { get => _poolObject; }   //For Inject

        GameObject _original;
        Queue<GameObject> _poolObject = new Queue<GameObject>();
        bool _isInitalize = false;
        Transform _parent;


        public void Init(GameObject original, int Count)
        {
            if (_isInitalize)
            {
                Debug.LogError($"{original.name} is Already Initalized");
                return;
            }

            _isInitalize = true;

            _parent = new GameObject(original.name).transform;

            _original = original;
            for (int i = 0; i < Count; i++)
            {
                CreatePool();
            }
        }

        void CreatePool()
        {
            var newObj = UnityEngine.Object.Instantiate(_original, _parent);
            newObj.name = _original.name;
            push(newObj);
        }

        public T Pop<T>(Transform parent = null) where T : MonoBehaviour
        {
            if (_poolObject.Count <= 0)
            {
                Debug.LogWarning($"Pool is Lack If {_original.name} is BaseFacade, Can't Resolve Anywhere");
                CreatePool();
            }

            var popObject = _poolObject.Dequeue();
            popObject.SetActive(true);
            if (parent != null)
            {
                popObject.transform.SetParent(parent);
            }

            return popObject.GetOrAddComponent<T>();
        }

        public void push(GameObject poolObject)
        {
            if (poolObject.name == _original.name)
            {
                poolObject.transform.SetParent(_parent);
                poolObject.SetActive(false);
                _poolObject.Enqueue(poolObject);
            }
        }

        public void Clear()
        {
            foreach (var obj in _poolObject)
            {
                UnityEngine.Object.Destroy(obj);
            }

            _poolObject.Clear();
            _isInitalize = false;
        }

        public void Dispose()
        {
            Clear();
        }
    }
