

public class SkinUpgrade : Item
{
    public override void Action()
    {
        Value *= Disposable ? 1 : Upgrade;
        Upgrade++; 
    }

    public override void Init()
    {
        Name = AbilityNames.SkinUpgrade.ToString();
        Value = 5.0f;
        Upgrade = 1;
        Cost = 300;
        Desc = $"{Value*100f} % Reduce damage";
        Disposable = false;
    }
}
