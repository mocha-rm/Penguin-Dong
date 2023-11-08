using System;
using VContainer;
using MessagePipe;
namespace GameScene.Rule
{
    using Message;
    public partial class GameRule
    {
        ISubscriber<SceneLoadEvent> _sceneLoadSub;

        IDisposable SubscribeSceneLoadEvent()
        {
            _sceneLoadSub = _container.Resolve<ISubscriber<SceneLoadEvent>>();
            return _sceneLoadSub.Subscribe(evt =>
            {
                _model.GameState.Value = GameState.GameOver;
                var sceneLoader = _container.Resolve<SceneService>();
                //_audioService.Stop(AudioService.SoundType.BGM);
                sceneLoader.LoadScene(evt.Scene).Forget();
            });
        }
    }

}

namespace GameScene.Message
{
    public class SceneLoadEvent
    {
        public SceneName Scene;
    }
}
