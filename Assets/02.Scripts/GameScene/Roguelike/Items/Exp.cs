

public class Exp : Item
{
    public override void Init()
    {
        Name = "Exp";
        Value = 1.2f;
        Cost = 350;
        Desc = $"Get x{Value} xp more";
        Disposable = false;
    }
}
