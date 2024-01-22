using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class Skinarmor : Item
{
    protected override void Init()
    {
        Name = "Skin Upgrade";
        Value = 0.1f;
        Cost = 300;
        Desc = $"{Value*100f} % Reduce damage";
        Disposable = false;
    }

    public override void Dispose()
    {
        Sprite = null;
        Name = string.Empty;
        Value = 0f;
        Cost = 0;
        Desc = string.Empty;
    }
}
