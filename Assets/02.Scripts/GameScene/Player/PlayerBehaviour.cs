using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameScene.Rule;


namespace GameScene.Player
{
    public class PlayerBehaviour : MonoBehaviour
    {
        //Player �� ��ǰ �� 1

        private Vector3 LeftScale = new Vector3(-1f, 1f, 1f);


        private float _maxSpeed;

        private float _speed;

        private Animator _ani;

        private SpriteRenderer _renderer;

        private Coroutine _routine = null;



        private void OnValidate() //component register method
        {
            _ani = GetComponent<Animator>();
            _renderer = GetComponent<SpriteRenderer>();
        }


        public void Init(float maxSpeed, Vector3 pos)
        {
            _maxSpeed = maxSpeed;

            transform.position = pos;

            if (Random.Range(0f, 1f) > 0.5f)
            {
                _speed = maxSpeed;
            }
            else
            {
                _speed = -maxSpeed;
            }

            transform.localScale = _speed.Equals(maxSpeed) ? Vector3.one : LeftScale;

            Debug.Log($"Player Initialized");
        }


        public void Clear()
        {
            Debug.Log($"Player Cleared");
        }


        public void Move()
        {
            _ani.SetBool("Run", true);

            transform.position += Vector3.right * Time.deltaTime * _speed;
        }

        public void SetDirection()
        {
            var state = _ani.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("penguin_walk"))
            {
                if (_routine != null)
                {
                    StopCoroutine(_routine);
                }

                _routine = StartCoroutine(_speed > 0 ? IcyTurnLeft() : IcyTurnRight());
            }
        }

        public void Crashed() //invulnerable (상처를 입힐 수 없는ㅋ)
        {
            _ani.Play("penguin_jump");

            _renderer.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }


        public void BodyColorSwitch()
        {
            _renderer.color = Color.white;
        }

        public void GameOverAnimate()
        {
            _ani.Play("GameOver");
        }


        private IEnumerator IcyTurnLeft()
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);

            float controlValue = 0.05f;

            while (true)
            {
                _speed -= controlValue;
                if (_speed < -_maxSpeed)
                {
                    break;
                }
                yield return null;
            }

            yield return null;
        }

        private IEnumerator IcyTurnRight()
        {
            transform.localScale = Vector3.one;

            float controlValue = 0.05f;

            while (true)
            {
                _speed += controlValue;
                if (_speed > _maxSpeed)
                {
                    break;
                }
                yield return null;
            }

            yield return null;
        }
    }
}

