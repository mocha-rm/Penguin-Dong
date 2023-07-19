using UniRx;

namespace GameScene.Icicle
{
    public class LegendIcicleFacade : IcicleFacade
    {
        protected override FacadeModel CreateModel()
        {
            return new FacadeModel()
            {
                _isAlive = new ReactiveProperty<bool>(true),
                _gravity = new ReactiveProperty<float>(Constants.GRAVITY_Scales[(int)IcicleType.Legend]),
            };
        }
    }
}
