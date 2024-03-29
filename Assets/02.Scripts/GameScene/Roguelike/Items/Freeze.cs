using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class Freeze : Item
{
    public override void Action()
    {
        Utility.CustomLog.Log("Freeze Action");    
    }

    public override void Init()
    {
        Name = AbilityNames.Freeze.ToString();
        Value = 5000f; //5seconds
        Cost = 300;
        Desc = $"Reduce objects`s speed 70% for {Value / 1000f} seconds";
        Disposable = true;
    }
}
