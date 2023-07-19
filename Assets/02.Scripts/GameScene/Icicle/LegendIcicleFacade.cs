using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameScene.Icicle
{
    public class LegendIcicleFacade : IcicleFacade
    {
        protected override FacadeModel CreateModel(float gr = 1.5f)
        {
            return base.CreateModel(gr);
        }
    }
}
