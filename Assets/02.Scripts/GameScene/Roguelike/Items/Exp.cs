

public class Exp : Item
{
    public override void Action()
    {
        if (Upgrade > 1)
        {
            Value += 0.3f;
        }

        Upgrade++;
    }

    public override void Init()
    {
        Name = AbilityNames.Exp.ToString();
        Value = 1.2f;
        Upgrade = 1;
        Cost = 350;
        Desc = $"Get x{Value} xp more";
        Disposable = false;
    }
}
