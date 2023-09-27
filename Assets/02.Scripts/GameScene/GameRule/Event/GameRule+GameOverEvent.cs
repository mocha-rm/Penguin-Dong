using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using MessagePipe;


namespace GameScene.Rule
{
    using Message;

    public partial class GameRule
    {
        ISubscriber<GameOverEvent> _gameOverSub;

        IDisposable SubscribeGameOverEvent()
        {
            _gameOverSub = _container.Resolve<ISubscriber<GameOverEvent>>();
            return _gameOverSub.Subscribe(data =>
            {
                _model.GameState.Value = GameState.GameOver;
                _uiController.GameOverUIAction(false); //boolian value is for compare new record or not
                
                Debug.Log("GameOver");
            });
        }
    }

}

namespace GameScene.Message
{
    class GameOverEvent
    {

    }
}
