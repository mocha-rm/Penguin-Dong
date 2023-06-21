using System.Collections;
using UnityEngine;
using UniRx;


public class Player : MonoBehaviour
{
    private enum Side { Left = -1, Right = 1 }


    [Header("Status")]
    [SerializeField] private float speed = 0f;

    private Vector3 originPosition;


    private void Awake()
    {
        originPosition = transform.position;
        Init();
    }

    void Start()
    {
        Observable.EveryUpdate()
            .Where(_ => Application.isPlaying)//TODO : Option Will Change : When CountDown End...
            .Subscribe(_ =>
            {
                Move();
            }).AddTo(this.gameObject);
    }

    private void Init()
    {
        ExGameManager.player = this;

        transform.position = originPosition;

        speed = Random.Range(0f, 1f) > 0.5f ? 3f : -3f;

        transform.localScale = speed.Equals(3) ? new Vector3((int)Side.Right * 0.5f, 0f, 0f) : new Vector3((int)Side.Left * 0.5f, 0f, 0f);

        Debug.Log($"Player Initialized");
    }

    private void Move()
    {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }


    public void SetDirection()
    {
        if (transform.position.x < 0)
        {
            speed = 3.0f;
        }
        else
        {
            speed = -3.0f;
        }
    }
}
