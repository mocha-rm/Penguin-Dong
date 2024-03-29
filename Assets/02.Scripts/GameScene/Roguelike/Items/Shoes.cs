public class Shoes : Item
{
    public override void Action()
    {
        Value *= Disposable ? 1 : Upgrade;
        Upgrade++;     
    }

    public override void Init()
    {
        Name = AbilityNames.Shoes.ToString();
        Value = 0.1f;
        Upgrade = 1;
        Cost = 300;
        Desc = $"Slip reduction -{Value*100}%";
        Disposable = false;
    }
}
