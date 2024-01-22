using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public Sprite Sprite;
    public string Name;
    public float Value;
    public int Cost;
    public string Desc;
    public bool Disposable; //is it justonetime? or not 


    private void Start()
    {
        Init();
    }



    protected abstract void Init();

    public abstract void Dispose();
}
