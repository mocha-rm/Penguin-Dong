using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected Sprite Sprite;
    protected string Name;
    protected int Cost;
    protected string Desc;
    protected bool Disposable;


    private void Start()
    {
        Init();
    }



    protected abstract void Init();


    public abstract void Dispose();
    public abstract void Excute();
}
