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
        ISubscriber<GetRewardAndContinueEvent> _getRewardSub;

        IDisposable SubscribeGetRewardAndContinueEvent()
        {
            _getRewardSub = _container.Resolve<ISubscriber<GetRewardAndContinueEvent>>();
            return _getRewardSub.Subscribe(data =>
            {
                isGameOver = false;
                _model.IsGetReward.Value = data.isRewardGet;
                _model.Life.Value = 100.0f;
                _uiController.LifeUIAction(_model.Life.Value);
                _uiController.ResetHpBar();
                _uiController.ResetGameOverPanelUIElements();
                _uiController.NonactiveAdContinueBtn();
                _model.GameState.Value = GameState.Waiting;

                //Countdown And Play again
                _playerController.IdleAction();
                _uiController.ReCountdown();
            });
        }
    }
}

namespace GameScene.Message
{
    class GetRewardAndContinueEvent
    {
        public bool isRewardGet;
    }
}


