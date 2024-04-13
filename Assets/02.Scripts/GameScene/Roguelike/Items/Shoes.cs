public class Shoes : Item
{
    public override void Action()
    {
        Value += 0.03f;
        Upgrade++;
    }

    public override void Init()
    {
        Name = AbilityNames.Shoes.ToString();
        Value = 0f;
        Upgrade = 1;
        Cost = 300;
        Desc = $"Shoes Upgrade : Slip reduction";
        Disposable = false;


        /*Shoes Values
         * 0.03
         * 0.06
         * 0.09
         * 0.12
         * ...
         * ...
         */
    }
}
