using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour
{
    //Animation Test

    public Animator _animator;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            _animator.Play("penguin_walk");
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            _animator.Play("penguin_jump");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            _animator.Play("penguin_preslide");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _animator.Play("penguin_slide");
        }
    }
}
