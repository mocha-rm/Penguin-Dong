using UnityEngine;
using UnityEngine.UI;
using UniRx;


public class InGameUI : MonoBehaviour
{
    [Header("PlayerControl")]
    [SerializeField] private Button directionControlBtn;

    [Header("CountDown")]
    private int count; //TODO : Make CountSystem;


    private void Start()
    {
        directionControlBtn.OnClickAsObservable()
             .Subscribe(_ =>
             {
                 ExGameManager.player.SetDirection();
             }).AddTo(this.gameObject);
    }
}
