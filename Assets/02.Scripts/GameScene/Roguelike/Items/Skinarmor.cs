using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class Skinarmor : Item
{
    [SerializeField] Sprite _sprite;
    private float _lessAmount = 0f;



    protected override void Init()
    {
        Sprite = _sprite;
        Name = "Skin Upgrade";
        Cost = 300;
        Desc = $"{_lessAmount} % Reduce damage";
        Disposable = true;
    }

    public override void Dispose()
    {
        Sprite = null;
        Name = string.Empty;
        Cost = 0;
        Desc = string.Empty;
        Disposable = false;

        CustomLog.Log($"{this.name} Disposed");
    }

    public override void Excute()
    {
        
    }
}
