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
                bool isRecord = false;

                _model.Life.Value = 0f;
                _uiController.LifeUIAction(_model.Life.Value);
                _model.GameState.Value = GameState.GameOver;

                _model.Coin.Value = _model.Score.Value / 10; // need to save the total coin on DB                
                _roguelikeController.AddCoin(_model.Coin.Value);

                _dbService.SetPlayerData("TotalCoin", _roguelikeController.GetCoin().ToString());
                if (_model.Score.Value > _dbService.BestScore)
                {
                    isRecord = true;
                    _dbService.SetPlayerData("BestScore", _model.Score.Value.ToString());
                    _rankService.SubmitScore(_model.Score.Value);
                }

                _uiController.GameOverUIAction(isRecord, _model.Score.Value, _model.Level.Value, _model.Coin.Value); //boolian value is for compare new record or not
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
