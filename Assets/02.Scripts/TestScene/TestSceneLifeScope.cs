using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using VContainer;



namespace TestScene
{
    public class TestSceneLifeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            RegisterController(builder);
            RegisterFacade(builder);
            RegisterMessage(builder);
        }

        private void RegisterController(IContainerBuilder builder)
        {

        }

        private void RegisterFacade(IContainerBuilder builder)
        {
        
        }

        private void RegisterMessage(IContainerBuilder builder)
        {

        }


        public static class Hierarchy
        {
        
        }
    }
}
