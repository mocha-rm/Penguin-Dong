

public class SkinUpgrade : Item
{
    public override void Init()
    {
        Name = "Skin Upgrade";
        Value = 0.1f;
        Cost = 300;
        Desc = $"{Value*100f} % Reduce damage";
        Disposable = false;
    }
}
