using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VContainer;
using MessagePipe;

namespace GameScene.Rule
{
    using Message;

    public partial class GameRule
    {
        ISubscriber<CountDownComplete> _countDownSub;

        IDisposable SubscribeCountDownCompleteEvent() //구독자들이 받는 이벤트
        {
            _countDownSub = _container.Resolve<ISubscriber<CountDownComplete>>();
            return _countDownSub.Subscribe(data =>
            {
                _model.GameState.Value = GameState.Playing;
                _audioService.Play(AudioService.AudioResources.GO, AudioService.SoundType.SFX);
            });
        }
    }
}


namespace GameScene.Message
{
    class CountDownComplete
    {
        
    }
}
