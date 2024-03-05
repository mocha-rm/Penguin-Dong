
public class GodBless : Item
{
    public override void Init()
    {
        Name = "God Bless";
        Value = 3000f; //3Seconds
        Cost = 800;
        Desc = $"HP or Money\nDown from the sky for {Value * 0.001f} seconds";
        Disposable = true;
    }
}
 