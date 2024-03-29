
public class Heal : Item
{
    public override void Action()
    {
        Utility.CustomLog.Log("Heal Action");
    }

    public override void Init()
    {
        Name = AbilityNames.Heal.ToString();
        Value = 0.1f;
        Cost = 500;
        Desc = "Earn 10% of total HP immediately";
        Disposable = true;
    }
}
