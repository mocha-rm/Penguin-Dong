using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Player
{
    public class ShieldBehaviour : MonoBehaviour
    {
        Animator _ani;


        public void Init()
        {
            _ani = GetComponent<Animator>();
        }

        public void ExplosionAction()
        {
            gameObject.SetActive(false);
        }

        public void HittedParticleAction()
        {
            _ani.Play("Shield_Hitted");
        }
    }
}
