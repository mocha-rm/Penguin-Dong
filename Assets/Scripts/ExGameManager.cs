using UnityEngine;

public class ExGameManager : MonoSingleton<ExGameManager>
{
    public static Player player = null;


    private void Awake()
    {
        Init();
    }

    #region GamePart
    private void Init()
    {
        Debug.Log($"GameManager Initialized");
    }
    #endregion
}
