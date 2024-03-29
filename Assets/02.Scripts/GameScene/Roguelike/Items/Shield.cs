public class Shield : Item
{
    public override void Action()
    {
        Utility.CustomLog.Log("Shield Action");
    }

    public override void Init()
    {
        Name = AbilityNames.Shield.ToString();
        Value = 3.0f;
        Cost = 650;
        Desc = $"Protect Damage 3 times";
        Disposable = true;
    }
}
