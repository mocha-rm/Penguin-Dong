public class Heart : Item
{
    public override void Action()
    {
        throw new System.NotImplementedException();
    }

    public override void Init()
    {
        Name = "Heart";
        Value = 15.0f;
        Cost = 450;
        Desc = $"Total HP +{(int)Value}";
        Disposable = false;
    }
}
