using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScene.Message;
using MessagePipe;
using VContainer;


namespace GameScene.Rule
{
    public partial class GameRule
    {
        ISubscriber<ScoreUpEvent> _scoreUpSub;

        IDisposable SubscribeScoreUpEvent()
        {
            _scoreUpSub = _container.Resolve<ISubscriber<ScoreUpEvent>>();
            return _scoreUpSub.Subscribe(_ =>
            {
                _uiController.ScoreUIAction(_model.Score.Value++);
            });
        }
        
    }
}

namespace GameScene.Message
{
    public class ScoreUpEvent
    {

    }
}
