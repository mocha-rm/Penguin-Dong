using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;


public class DefaultLoadingScreen : BaseLoadingScreen
{
    [SerializeField] Image _slider;

    public override void Report(float value)
    {
        _slider.fillAmount = value;
    }
}