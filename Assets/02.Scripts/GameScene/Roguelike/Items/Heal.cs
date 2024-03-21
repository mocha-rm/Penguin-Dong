public class Heal : Item
{
    public override void Action()
    {
        throw new System.NotImplementedException();
    }

    public override void Init()
    {
        Name = "Heal";
        Value = 0.1f;
        Cost = 500;
        Desc = "Earn 10% of total HP immediately";
        Disposable = true;
    }
}
