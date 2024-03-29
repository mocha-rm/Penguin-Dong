public class Lotto : Item
{
    public override void Action()
    {
        Utility.CustomLog.Log("Lottery Action");
    }

    public override void Init()
    {
        Name = AbilityNames.Lotto.ToString();
        Value = 0.5f;
        Cost = 0;
        Desc = $"Caution ! Success : Coin x2, Fail : Coin ÷2\nThe odds are 50:50";
        Disposable = true;
    }
}
