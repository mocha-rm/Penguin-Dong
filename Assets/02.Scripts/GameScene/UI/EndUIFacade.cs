using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VContainer;
using VContainer.Unity;
using UniRx;


namespace GameScene.UI
{
    public class EndUIFacade : IInitializable, IDisposable
    {
        [Inject] IObjectResolver _container;


        public void Initialize()
        {
            throw new NotImplementedException();
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }


        public static class Constants
        {

        }

        public class FacadeModel
        {

        }
    }
}
