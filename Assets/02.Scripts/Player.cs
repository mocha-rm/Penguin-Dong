using System.Collections;
using UnityEngine;
using UniRx;
using Utility;


public class Player : MonoBehaviour
{
    private const float MAX_SPEED = 5f;


    [Header("Status")]
    [SerializeField] private float speed = 0f;

    private Vector3 originPosition;

    private Animator ani;

    private Coroutine routine = null;




    private void Awake()
    {
        originPosition = transform.position;
        Init();
    }

    void Start()
    {
        Observable.EveryFixedUpdate()
            .Where(_ => Application.isPlaying)//TODO : Option Will Change : When CountDown End...
            .Subscribe(_ =>
            {
                Move();
            }).AddTo(this.gameObject);
    }

    private void Init()
    {
        ExGameManager.player = this;

        ani = GetComponent<Animator>();

        transform.position = originPosition;

        speed = Random.Range(0f, 1f) > 0.5f ? MAX_SPEED : -MAX_SPEED;

        transform.localScale = speed.Equals(MAX_SPEED) ? Vector3.one : new Vector3(-1f, 1f, 1f);

        CustomLog.Log($"Player Initialized");
    }

    private void Move()
    {
        ani.SetBool("Run",true);
        transform.position += Vector3.right * Time.deltaTime * speed;
    }

    public void SetDirection()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(speed > 0 ? IcyTurnLeft() : IcyTurnRight());
    }

    private IEnumerator IcyTurnLeft()
    {
        transform.localScale = new Vector3(-1f, 1f, 1f);

        float controlValue = 0.05f;

        while (true)
        {
            speed -= controlValue;
            if (speed < -MAX_SPEED)
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
            speed += controlValue;
            if (speed > MAX_SPEED)
            {
                break;
            }
            yield return null;
        }

        yield return null;
    }

    public void SuperSlide() //�ʻ��
    {
        ani.SetTrigger("SuperSlide");
        //Rigidbody 2D �̿��ؼ� ������ �ٴ� ���� �ָ� ���� ��
    }
}
