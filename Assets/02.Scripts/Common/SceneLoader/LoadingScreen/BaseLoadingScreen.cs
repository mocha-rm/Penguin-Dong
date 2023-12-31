using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



    public abstract class BaseLoadingScreen : MonoBehaviour, IProgress<float>
    {
        public virtual void SetLoadingText(string str)
        {
            //Debug.Log($"[Scene]<color=gray>{str}</color>");
            return;
        }
        public abstract void Report(float value);
    }
