using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public abstract class Poo : MonoBehaviour
{
    #region Common Functions

    private void Awake()
    {
        Init();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player")
        {
            Debug.Log($"°×¿À¹ö");
            //SetActive(False); After gameover coroutine
            //Sound And Animation
        }
    }


    private void Init()
    {
        Debug.Log($"Poo Init");
    }
    #endregion
}
