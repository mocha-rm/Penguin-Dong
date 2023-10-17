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
                _model.Coin.Value = (int)(_model.Score.Value * 0.1f); // need to save the total coin on DB
                _uiController.GameOverUIAction(false, _model.Score.Value, _model.Level.Value, _model.Coin.Value); //boolian value is for compare new record or not
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
