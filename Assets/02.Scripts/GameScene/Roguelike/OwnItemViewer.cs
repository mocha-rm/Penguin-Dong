using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;


public class OwnItemViewer : MonoBehaviour
{
    List<Image> _itemsImg;
    List<Button> _itemsBtn;
    List<TextMeshProUGUI> _textObjects;
    Dictionary<string, int> _ownitems;

    int index;


    public void Init()
    {
        index = 0;

        _itemsImg = new List<Image>();
        _itemsBtn = new List<Button>();
        _textObjects = new List<TextMeshProUGUI>();
        _ownitems = new Dictionary<string, int>();


        for(int i = 0; i < transform.childCount; i++)
        {
            _itemsImg.Add(transform.GetChild(i).GetComponent<Image>());
            _itemsBtn.Add(transform.GetChild(i).GetComponent<Button>());

            _itemsImg[i].sprite = null;
        }

        foreach (Button btn in _itemsBtn)
        {
            btn.OnClickAsObservable().Subscribe(_ =>
            {
                ShowItemLevelInfo(btn.transform);
            }).AddTo(this.gameObject);

            _textObjects.Add(btn.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>());
        }
    }


    public void SetImageAndLevel(Item item)
    {
        if (!item.Disposable)
        {
            if (!_ownitems.ContainsKey(item.Name))
            {
                int temp = index;
                _ownitems.Add(item.Name, temp);
                _itemsImg[index].sprite = item.Sprite;
                _itemsImg[index].gameObject.SetActive(true);
                index++;
            }
            else
            {
                _textObjects[_ownitems[item.Name]].text = $"Lv.{item.Upgrade}";
            }
        }
    }

    private void ShowItemLevelInfo(Transform tr)
    {
        bool isOn = tr.GetChild(0).gameObject.activeInHierarchy;
    
        isOn = !isOn;

        tr.GetChild(0).gameObject.SetActive(isOn);
    }
}
