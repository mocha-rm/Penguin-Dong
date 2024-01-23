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
using GameScene.Message;
using Utility;


namespace TestScene
{
    public class RoguelikeFacade : BaseFacade, IRegistMonobehavior
    {
        //set container here
        [SerializeField] List<Item> _items;

        Item _pickedItem = null;

        //RefreshBtn
        Button _refreshBtn;

        //Item1
        Button _item1Btn;
        TextMeshProUGUI _item1Name;
        Image _item1Img;
        TextMeshProUGUI _item1Desc;

        //Item2
        Button _item2Btn;
        TextMeshProUGUI _item2Name;
        Image _item2Img;
        TextMeshProUGUI _item2Desc;

        //Movement
        RectTransform _rect;
        float smoothSpeed = 5f;
        Coroutine _moveRoutine = null;
        

        FacadeModel _model;

        //IPublisher<RoguelikePayEvent> _roguePayPub; //action for press item (buy)




        public void RegistBehavior(IContainerBuilder builder)
        {
            _rect = GetComponent<RectTransform>();
            //UI Elements Register Here...

            _refreshBtn = gameObject.GetHierachyPath<Button>(Hierachy.RefreshButton);

            _item1Btn = gameObject.GetHierachyPath<Button>(Hierachy.Item1Btn);
            _item1Name = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierachy.Item1Name);
            _item1Img = gameObject.GetHierachyPath<Image>(Hierachy.Item1Icon);
            _item1Desc = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierachy.Item1Desc);

            _item2Btn = gameObject.GetHierachyPath<Button>(Hierachy.Item2Btn);
            _item2Name = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierachy.Item2Name);
            _item2Img = gameObject.GetHierachyPath<Image>(Hierachy.Item2Icon);
            _item2Desc = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierachy.Item2Desc);
        }

        public override void Initialize()
        {
            //_roguePayPub = _container.Resolve<IPublisher<RoguelikePayEvent>>();

            if (_model == null)
            {
                _model = CreateModel();
            }

            SetOriginPosition();
            SetItems();
        }

        public override void Dispose()
        {
            foreach (var item in _items)
            {
                item.Dispose();
            }

            _model?.Dispose();
            _model = null;
        }


        #region Public
        public void SetOriginPosition()
        {
            _rect.offsetMin = new Vector2(_rect.offsetMin.x, -Screen.height);
            _rect.offsetMax = new Vector2(_rect.offsetMax.x, -Screen.height);
        }

        public void OpenAction()
        {
            if(_moveRoutine != null)
            {
                StopCoroutine(_moveRoutine);
            }

            _moveRoutine = StartCoroutine(Movement());
        }


        public int GetCoinInfo()
        {
            return _pickedItem.Cost;
        }

        public float GetItemValue()
        {
            return _pickedItem.Value;
        }
        #endregion


        #region Private
        private IEnumerator Movement()
        {
            Vector2 targetBottomPos = new Vector2(_rect.offsetMin.x, 0f);
            Vector2 targetTopPos = new Vector2(_rect.offsetMax.x, 0f);


            while(_rect.offsetMin.y <= 0f)
            {
                Vector2 targetMinOffset = Vector2.Lerp(_rect.offsetMin, targetBottomPos, smoothSpeed * Time.deltaTime);
                Vector2 targetMaxOffset = Vector2.Lerp(_rect.offsetMax, targetTopPos, smoothSpeed * Time.deltaTime);

                _rect.offsetMin = targetMinOffset;
                _rect.offsetMax = targetMaxOffset;

                yield return null;
            }

            _rect.offsetMin = targetBottomPos;
            _rect.offsetMax = targetTopPos;
        }

        private void SetItems()
        {
            var items = GetRandom();


        }

        private (Item, Item) GetRandom()
        {
            var itemList = _items;

            int index = 0;

            Item[] items = new Item[2];

            while (index < 2)
            {
                int rand = Random.Range(0, _items.Count);

                Item temp = itemList[rand];

                items[index] = temp;

                itemList.Remove(temp);

                index++;
            }

            return (items[0], items[1]);
        }
        #endregion





        public static class Hierachy
        {
            public static readonly string Item1Btn;
            public static readonly string Item1Name = "";
            public static readonly string Item1Icon = "";
            public static readonly string Item1Desc = "";
            public static readonly string Item1Coin = "";


            public static readonly string Item2Btn;
            public static readonly string Item2Name = "";
            public static readonly string Item2Icon = "";
            public static readonly string Item2Desc = "";
            public static readonly string Item2Coin = "";

            public static readonly string RefreshButton = "";
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


