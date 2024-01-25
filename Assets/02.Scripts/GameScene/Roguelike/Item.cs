using UnityEngine;
using Utility;
using UniRx;


public abstract class Item : MonoBehaviour
{
    public Sprite Sprite;
    public string Name;
    public float Value;
    public int Cost;
    public string Desc;
    public bool Disposable; //is it justonetime? or not 



    public abstract void Init();

    public void Dispose()
    {
        Sprite = null;
        Name = string.Empty;
        Value = 0f;
        Cost = 0;
        Desc = string.Empty;
        Disposable = false;

        CustomLog.Log("Item disposed");
    }
}