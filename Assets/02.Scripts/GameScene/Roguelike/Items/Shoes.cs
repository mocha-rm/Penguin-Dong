public class Shoes : Item
{
    public override void Action()
    {
        throw new System.NotImplementedException();
    }

    public override void Init()
    {
        Name = "Shoes";
        Value = 0.1f;
        Cost = 300;
        Desc = $"Slip reduction -{Value*100}%";
        Disposable = false;
    }
}
