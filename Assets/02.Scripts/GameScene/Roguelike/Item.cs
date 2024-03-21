using UnityEngine;
using Utility;
using UniRx;


public abstract class Item : MonoBehaviour
{
    public Sprite Sprite;
    public string Name;
    public float Value;
    public int Upgrade;
    public int Cost;
    public string Desc;
    public bool Disposable; //일회용 : true, 누적템 : false


    public abstract void Init();

    public abstract void Action();

    public void Dispose()
    {
        Sprite = null;
        Name = string.Empty;
        Value = 0f;
        Upgrade = 1;
        Cost = 0;
        Desc = string.Empty;
        Disposable = false;

        CustomLog.Log("Item disposed");
    }
}
