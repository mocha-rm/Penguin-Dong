using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;


namespace GameScene.Icicle
{
    public class SuperIcicleFacade : IcicleFacade
    {
        protected override FacadeModel CreateModel(float gr = 1.3f)
        {
            return base.CreateModel(gr);
        }
    }
}

