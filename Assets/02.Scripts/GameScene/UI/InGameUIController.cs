using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using System;
using GameScene.Message;
using UnityEditor.VersionControl;
using Unity.VisualScripting;

namespace GameScene.UI
{
    public class InGameUIController : MonoBehaviour, VContainer.Unity.IInitializable, IDisposable, ITickable
    {
        [Inject] IObjectResolver _container;

        InfoIndicateFacade _infoIndicateFacade;
        InteractionFacade _interactionFacade;
        IPublisher<GameOverEvent> _gameOverPub;


        //For Test
        int _tScore = 0;
        int _addAmount = 100;
        bool _isRecord;

        int lifeOrder = 2;

        float _levelGuage = 0f;
        int _currentLevel = 1;


        public void Initialize()
        {
            _infoIndicateFacade = _container.Resolve<InfoIndicateFacade>();
            _interactionFacade = _container.Resolve<InteractionFacade>();
            _gameOverPub = _container.Resolve<IPublisher<GameOverEvent>>();
        }
        public void Dispose()
        {
            
        }

        public void Tick() //just in testcase
        {
            //Nontest situation use Bloc class to access between ui <-> datas

            if (Input.GetKeyDown(KeyCode.S))
            {
                _tScore += _addAmount;
                _infoIndicateFacade.IndicateScore(_tScore);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                _infoIndicateFacade.IndicateLifeStatus(lifeOrder--);
                if (lifeOrder < 0)
                {
                    _interactionFacade.ActivateGameOverPanel(_isRecord);
                    _gameOverPub.Publish(new GameOverEvent());
                }
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                _levelGuage += UnityEngine.Random.Range(0.1f, 0.3f);

                if (_levelGuage > 1f)
                {
                    _currentLevel += 1;
                    _levelGuage -= 1f;
                }

                _infoIndicateFacade.IndicateLevelStatus(_levelGuage, _currentLevel);
            }
        }
    }
}
