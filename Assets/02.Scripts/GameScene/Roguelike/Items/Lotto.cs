public class Lotto : Item
{
    public override void Init()
    {
        Name = "Lottery";
        Value = 2.0f;
        Cost = 0;
        Desc = $"Caution ! Success : Coin x2, Fail : Coin ÷2\nThe odds are 50:50";
        Disposable = true;
    }
}
