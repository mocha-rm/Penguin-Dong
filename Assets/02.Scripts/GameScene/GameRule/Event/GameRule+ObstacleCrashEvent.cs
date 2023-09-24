using System;
using VContainer;
using MessagePipe;
using UnityEngine.TextCore.Text;
using GameScene.Player;
using GameScene.Message;

namespace GameScene.Rule
{
    public partial class GameRule
    {
        ISubscriber<ObstacleCrashEvent> _colObstacleSub;

        IDisposable SubscribeObstacleCrashEvent()
        {
            _colObstacleSub = _container.Resolve<ISubscriber<ObstacleCrashEvent>>();
            return _colObstacleSub.Subscribe(data =>
            {
                if (data.Character != null && data.Obstacle != null)
                {
                    //sound play
                    //Delete Heart
                }
                else
                {
                    if (_model.GameState.Value == GameState.Playing)
                    {
                        _model.Score.Value++;
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
        public PlayerFacade Character;
        public Obstacle.ObstacleFacade Obstacle;
    }

}

