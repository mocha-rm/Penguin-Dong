public class Shield : Item
{
    public override void Action()
    {
        throw new System.NotImplementedException();
    }

    public override void Init()
    {
        Name = "Shield";
        Value = 3.0f;
        Cost = 650;
        Desc = $"Protect Damage 3 times";
        Disposable = true;
    }
}
