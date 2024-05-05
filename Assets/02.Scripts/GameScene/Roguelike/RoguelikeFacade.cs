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


namespace GameScene
{
    public class RoguelikeFacade : BaseFacade, IRegistMonobehavior
    {
        public IRoguelikeModel Model { get { return _model; } }

        //set container here
        [SerializeField] List<Item> _items;
        Item _pickedItem;


        //RefreshBtn
        Button _refreshBtn;
        TextMeshProUGUI _refreshCostText;

        //SkipBtn
        Button _skipBtn;

        //Item1
        Item _item1;
        Button _item1Btn;
        TextMeshProUGUI _item1Name;
        Image _item1Img;
        TextMeshProUGUI _item1Desc;
        TextMeshProUGUI _item1Cost;

        //Item2
        Item _item2;
        Button _item2Btn;
        TextMeshProUGUI _item2Name;
        Image _item2Img;
        TextMeshProUGUI _item2Desc;
        TextMeshProUGUI _item2Cost;

        //Movement
        RectTransform _rect;
        float smoothSpeed = 5f;
        Coroutine _moveRoutine = null;


        FacadeModel _model;

        CompositeDisposable _disposable;

        IPublisher<RoguelikePayEvent> _roguePayPub; //action for press item (buy)

        IPublisher<RoguelikeRefreshEvent> _rogueRefreshPub; //action for refresh item

        IPublisher<RoguelikeSkipEvent> _rogueSkipPub; //action for skip



        public void RegistBehavior(IContainerBuilder builder)
        {
            _rect = GetComponent<RectTransform>();
            
            _refreshBtn = gameObject.GetHierachyPath<Button>(Hierachy.RefreshButton);
            _refreshCostText = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierachy.RefreshCostText);

            _skipBtn = gameObject.GetHierachyPath<Button>(Hierachy.SkipButton);

            _item1Btn = gameObject.GetHierachyPath<Button>(Hierachy.Item1Btn);
            _item1Name = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierachy.Item1Name);
            _item1Img = gameObject.GetHierachyPath<Image>(Hierachy.Item1Icon);
            _item1Desc = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierachy.Item1Desc);
            _item1Cost = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierachy.Item1Cost);

            _item2Btn = gameObject.GetHierachyPath<Button>(Hierachy.Item2Btn);
            _item2Name = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierachy.Item2Name);
            _item2Img = gameObject.GetHierachyPath<Image>(Hierachy.Item2Icon);
            _item2Desc = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierachy.Item2Desc);
            _item2Cost = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierachy.Item2Cost);
        }

        public override void Initialize()
        {
            _disposable = new CompositeDisposable();
            _roguePayPub = _container.Resolve<IPublisher<RoguelikePayEvent>>();
            _rogueRefreshPub = _container.Resolve<IPublisher<RoguelikeRefreshEvent>>();
            _rogueSkipPub = _container.Resolve<IPublisher<RoguelikeSkipEvent>>();


            if (_model == null)
            {
                _model = CreateModel();
            }

            InitItems();

            SetOriginPosition();

            SetItems();


            _item1Btn.OnClickAsObservable().Subscribe(_ =>
            {
                _pickedItem = _item1;

                _pickedItem.Action();
                
                _roguePayPub.Publish(new RoguelikePayEvent()
                {
                   
                });
                CustomLog.Log("Item1 Buy");
            }).AddTo(_disposable);

            _item2Btn.OnClickAsObservable().Subscribe(_ =>
            {
                _pickedItem = _item2;

                _pickedItem.Action();

                _roguePayPub.Publish(new RoguelikePayEvent()
                {
                    
                });;
               
                CustomLog.Log("Item2 Buy");
            }).AddTo(_disposable);

            _refreshBtn.OnClickAsObservable().Subscribe(_ =>
            {
                _model._isRefresh.Value = true;

                _rogueRefreshPub.Publish(new RoguelikeRefreshEvent()
                {

                });

                CustomLog.Log("Refresh");
            }).AddTo(_disposable);

            _skipBtn.OnClickAsObservable().Subscribe(_ =>
            {
                _rogueSkipPub.Publish(new RoguelikeSkipEvent()
                {

                });

                CustomLog.Log("Skip");
            }).AddTo(_disposable);
        }

        public override void Dispose()
        {
            _refreshBtn.onClick.RemoveAllListeners();
            _skipBtn.onClick.RemoveAllListeners();
            _item1Btn.onClick.RemoveAllListeners();
            _item2Btn.onClick.RemoveAllListeners();
            _disposable?.Dispose();
            _disposable = null;

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

            _item1Btn.enabled = false;
            _item2Btn.enabled = false;
            _refreshBtn.enabled = false;
            _skipBtn.enabled = false;
        }

        public void OpenAction()
        {
            if (_moveRoutine != null)
            {
                StopCoroutine(_moveRoutine);
            }

            _moveRoutine = StartCoroutine(Movement());
        }

        public void CloseAction()
        {
            SetOriginPosition();
        }

        public void SetItems()
        {
            var items = GetRandom();

            _item1 = items.Item1;
            _item1Img.sprite = items.Item1.Sprite;
            _item1Name.text = items.Item1.Name;
            _item1Desc.text = items.Item1.Desc;
            _item1Cost.text = items.Item1.Cost.ToString();

            _item2 = items.Item2;
            _item2Img.sprite = items.Item2.Sprite;
            _item2Name.text = items.Item2.Name;
            _item2Desc.text = items.Item2.Desc;
            _item2Cost.text = items.Item2.Cost.ToString();
        }

        public Item GetPickedItem()
        {
            return _pickedItem;
        }

        //public int GetCoinInfo()
        //{
        //    return _pickedItem.Cost;
        //}

        //public float GetItemValue()
        //{
        //    return _pickedItem.Value;
        //}

        //public string GetItemName()
        //{
        //    return _pickedItem.Name;
        //}

        public void SetRefreshCostValue(int cost)
        {
            _refreshCostText.text = $"Again / {cost} Coin";
        }
        #endregion


        #region Private
        private IEnumerator Movement()
        {
            Vector2 targetBottomPos = new Vector2(_rect.offsetMin.x, 0f);
            Vector2 targetTopPos = new Vector2(_rect.offsetMax.x, 0f);


            while (_rect.offsetMin.y <= 0f)
            {
                Vector2 targetMinOffset = Vector2.Lerp(_rect.offsetMin, targetBottomPos, smoothSpeed * Time.deltaTime);
                Vector2 targetMaxOffset = Vector2.Lerp(_rect.offsetMax, targetTopPos, smoothSpeed * Time.deltaTime);

                _rect.offsetMin = targetMinOffset;
                _rect.offsetMax = targetMaxOffset;

                yield return null;
            }
            _item1Btn.enabled = true;
            _item2Btn.enabled = true;
            _refreshBtn.enabled = true;
            _skipBtn.enabled = true;

            _rect.offsetMin = targetBottomPos;
            _rect.offsetMax = targetTopPos;
        }


        private (Item, Item) GetRandom()
        {
            var copyItemList = _items;

            int index = 0;

            int prevRand = -1;

            Item[] items = new Item[2];


            while (index < 2)
            {
                int rand = Random.Range(0, _items.Count);

                if(prevRand.Equals(rand))
                {
                    continue;
                }

                prevRand = rand;

                Item temp = copyItemList[rand];

                items[index] = temp;


                index++;
            }

            return (items[0], items[1]);
        }

        private void InitItems()
        {
            foreach(Item item in _items)
            {
                item.Init();
            }
        }
        #endregion





        public static class Hierachy
        {
            public static readonly string Item1Btn = "Item1";
            public static readonly string Item1Name = "Item1/Name";
            public static readonly string Item1Icon = "Item1/Icon";
            public static readonly string Item1Desc = "Item1/Desc";
            public static readonly string Item1Cost = "Item1/Coin/Cost";


            public static readonly string Item2Btn = "Item2";
            public static readonly string Item2Name = "Item2/Name";
            public static readonly string Item2Icon = "Item2/Icon";
            public static readonly string Item2Desc = "Item2/Desc";
            public static readonly string Item2Cost = "Item2/Coin/Cost";

            public static readonly string RefreshButton = "Refresh";
            public static readonly string RefreshCostText = "Refresh/Cost";

            public static readonly string SkipButton = "SkipBtn";
        }


        private static FacadeModel CreateModel()
        {
            return new FacadeModel()
            {
                _isRefresh = new ReactiveProperty<bool>(false),
            };
        }

        public class FacadeModel : IRoguelikeModel
        {
            public ReactiveProperty<bool> _isRefresh;

            public IReadOnlyReactiveProperty<bool> IsRefresh => _isRefresh;

            public void ReturnRefreshStatus()
            {
                _isRefresh.Value = false;
            }

            public void Dispose()
            {
                _isRefresh?.Dispose();
                _isRefresh = null;
            }
        }
    }

    public interface IRoguelikeModel
    {
        public IReadOnlyReactiveProperty<bool> IsRefresh { get; }

        public void ReturnRefreshStatus();
    }
}


