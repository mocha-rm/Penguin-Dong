using System;
using VContainer;
using MessagePipe;
using UnityEngine.TextCore.Text;
using GameScene.Player;
using GameScene.Message;
using UnityEngine;
using Unity.VisualScripting;

namespace GameScene.Rule
{
    public partial class GameRule
    {
        ISubscriber<ObstacleCrashEvent> _colObstacleSub;

        int lifeImg_order = 2;

        IDisposable SubscribeObstacleCrashEvent()
        {
            _colObstacleSub = _container.Resolve<ISubscriber<ObstacleCrashEvent>>();
            return _colObstacleSub.Subscribe(data =>
            {
                if (data.Character != null && data.Obstacle != null)
                {
                    if(!_playerController.IsPlayerInvul())
                    {
                        _model.Life.Value -= 1;
                        data.Character.GetComponentInParent<PlayerFacade>().IsInvulnerable();
                        _uiController.LifeUIAction(lifeImg_order--);
                        //sound play
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (_model.GameState.Value == GameState.Playing)
                    {
                        _uiController.ScoreUIAction(_model.Score.Value++);
                    }
                }
            });
        }
    }
}


namespace GameScene.Message
{
    public class ObstacleCrashEvent
    {
        public PlayerBehaviour Character;
        public Obstacle.ObstacleFacade Obstacle;
    }
}

