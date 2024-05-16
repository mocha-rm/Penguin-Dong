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
                _model.Life.Value = 0f;
                _uiController.LifeUIAction(_model.Life.Value);
                _model.GameState.Value = GameState.GameOver;

                _model.Coin.Value = (int)(_model.Score.Value * 0.1f); // need to save the total coin on DB
                _roguelikeController.AddCoin(_model.Coin.Value);
                _dbService.SetPlayerData("TotalCoin", _roguelikeController.GetCoin().ToString());

                _uiController.GameOverUIAction(true, _model.Score.Value, _model.Level.Value, _model.Coin.Value); //boolian value is for compare new record or not

                //만약 점수가 지금의 베스트보다 높으면 서버로 업로드
                if (_model.Score.Value > _dbService.BestScore)
                {
                    _dbService.SetPlayerData("BestScore", _model.Score.Value.ToString());
                }
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
