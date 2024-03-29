public class Heart : Item
{
    public override void Action()
    {
        Value *= Disposable ? 1 : Upgrade;
        Upgrade++;
    }

    public override void Init()
    {
        Name = AbilityNames.Heart.ToString();
        Value = 5.0f;
        Upgrade = 1;
        Cost = 450;
        Desc = $"Total HP +{(int)Value}";
        Disposable = false;
    }
}
