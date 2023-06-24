using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public abstract class Poo : MonoBehaviour
{
    [Header("Position")]
    private readonly Vector3 ORIGIN_POSITION = new Vector3(0f,13f,0f);


    #region Common Functions
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log($"Game Over");
            //GameOver Action
        }

        if (other.tag == "Ground")
        {
            Debug.Log("Destroyed");
        }

        gameObject.SetActive(false);
    }

   void OnDisable()
   {
        transform.position = ORIGIN_POSITION;       
   }

    #endregion
}
