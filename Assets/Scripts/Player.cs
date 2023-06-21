using System.Collections;
using UnityEngine;
using UniRx;


public class Player : MonoBehaviour
{
    private const float MAX_SPEED = 5f;


    [Header("Status")]
    [SerializeField]private float speed = 0f;

    private Vector3 originPosition;

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

        transform.position = originPosition;

        speed = Random.Range(0f, 1f) > 0.5f ? MAX_SPEED : -MAX_SPEED;

        transform.localScale = speed.Equals(3) ? Vector3.one : new Vector3(-1f, 1f, 1f);

        Debug.Log($"Player Initialized");
    }

    private void Move() => transform.position += Vector3.right * Time.deltaTime * speed;
   
    public void SetDirection()
    {
        if(routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(speed > 0 ? IcyTurnLeft() : IcyTurnRight());
    }

    private IEnumerator IcyTurnLeft()
    {
        transform.localScale = new Vector3(-1f, 1f, 1f);;

        float controlValue = 0.05f;

        while(true)
        {
            speed -= controlValue;
            if(speed < -MAX_SPEED)
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

        while(true)
        {
            speed += controlValue;
            if(speed > MAX_SPEED)
            {
                break;
            }
            yield return null;
        }

        yield return null;
    }
}
