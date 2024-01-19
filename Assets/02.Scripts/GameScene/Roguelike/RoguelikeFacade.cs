using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using Unity.Collections;

namespace TestScene
{
    public class RoguelikeFacade : BaseFacade, IRegistMonobehavior
    {
        //set container here
        [SerializeField] List<Item> _items;

        //TestButton

        //Item1
        
        //Item2




        private FacadeModel _model;

        


        public void RegistBehavior(IContainerBuilder builder)
        {
            _items = new List<Item>();
            //UI Elements Register Here...
        }

        public override void Initialize()
        {
            if(_model == null)
            {
                _model = CreateModel();
            }

            SetItems();
        }

        public override void Dispose()
        {
            foreach(var item in _items)
            {
                item.Dispose();
            }
            
            _model?.Dispose();
            _model = null;
        }


        private void Clicked()
        {

        }

        //Load 2 random items in list
        private void SetItems() //Set UI Elements on editor first
        {
            //set the values here
        }



        private static FacadeModel CreateModel()
        {
            return new FacadeModel()
            {
                _isBuy = new ReactiveProperty<bool>(false),
            };
        }

        public class FacadeModel : IRoguelikeModel
        {
            public ReactiveProperty<bool> _isBuy;

            public IReadOnlyReactiveProperty<bool> IsBuy => _isBuy;

            public void Dispose()
            {
                _isBuy?.Dispose();
                _isBuy = null;
            }
        }
    }

    public interface IRoguelikeModel
    {
        public IReadOnlyReactiveProperty<bool> IsBuy { get; }
    }
}


