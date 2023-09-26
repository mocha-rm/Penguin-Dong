using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using System;
using GameScene.Message;

namespace GameScene.UI
{
    public class InGameUIController : MonoBehaviour, IInitializable, IDisposable
    {
        [Inject] IObjectResolver _container;

        InfoIndicateFacade _infoIndicateFacade;
        InteractionFacade _interactionFacade;
        IPublisher<GameOverEvent> _gameOverPub;


        public void Initialize()
        {
            _infoIndicateFacade = _container.Resolve<InfoIndicateFacade>();
            _interactionFacade = _container.Resolve<InteractionFacade>();
            _gameOverPub = _container.Resolve<IPublisher<GameOverEvent>>();
        }
        public void Dispose()
        {

        }


        public void ScoreUIAction(int score)
        {
            _infoIndicateFacade.IndicateScore(score);
        }

        public void LevelUIAction()
        {
            //TODO: make public method in _infoIndicateFacade and use here
        }

        public void LifeUIAction()
        {
            
        }
    }
}
