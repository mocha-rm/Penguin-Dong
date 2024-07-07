using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingBoard : MonoBehaviour
{
    [SerializeField]
    List<TextMeshProUGUI> _tmpList;



    public void Init()
    {
        _tmpList = new List<TextMeshProUGUI>();
    }

    public void SetTMP(Dictionary<string, int> dic)
    {
        int index = 0;

        foreach (var kvp in dic)
        {
            if (index < _tmpList.Count)
            {
                _tmpList[index].text = $"{kvp.Key}   {kvp.Value}";
            }
            else
            {
                // 딕셔너리 항목 수가 _tmpList 수보다 많을 경우 break
                break;
            }
            index++;
        }

        // 만약 dic 항목 수가 _tmpList 항목 수보다 적다면 나머지 항목들을 Empty로 설정
        for (int i = index; i < _tmpList.Count; i++)
        {
            _tmpList[i].text = "Empty";
        }
    }
}
