
public class Freeze : Item
{
    public override void Action()
    {
        throw new System.NotImplementedException();
    }

    public override void Init()
    {
        Name = "Freeze";
        Value = 5000f; //5seconds
        Cost = 300;
        Desc = $"Slow fireball for {Value / 1000f} seconds";
        Disposable = true;
    }
}
