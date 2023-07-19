using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;



namespace GameScene.Icicle
{
    public class BasicIcicleFacade : IcicleFacade
    {
        protected override FacadeModel CreateModel(float gr = 1.0f)
        {
            return base.CreateModel(gr);
        }
    }
}
