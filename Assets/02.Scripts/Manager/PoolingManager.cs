using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PoolingManager : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject icicle;
    private List<GameObject> icicles = new List<GameObject>();

    [SerializeField] private float spawnTime;
    private WaitForSeconds wfs = null;

    private Vector3 originPosition;

    private int spawnOrder = 0;



    private Coroutine routine = null;



    void Awake()
    {
        Init();
    }

    private void Start()
    {
        Observable.EveryUpdate()
        .Where(_ => spawnOrder >= icicles.Count)
        .Subscribe(_ => spawnOrder = 0).AddTo(this.gameObject);

        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(SpawnRoutine());
    }

    private void Init()
    {
        spawnOrder = 0;
        originPosition = icicle.transform.position;
        wfs = new WaitForSeconds(spawnTime);
        CreateIcicle();
    }


    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnIcicle();
            yield return wfs;
        }
    }

    private void CreateIcicle()
    {
        for(int i = 0; i < 30; i++)
        {
            GameObject ici = Instantiate(icicle, originPosition, Quaternion.identity, this.transform);
            icicles.Add(ici);
        }
    }

    private void SpawnIcicle()
    {
        Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0.0f, 1.0f),1.1f,0f));
        pos.z = 0.0f;

        GameObject t_icicle = icicles[spawnOrder++];
        t_icicle.transform.position = pos;
        t_icicle.SetActive(true);
    }
}
