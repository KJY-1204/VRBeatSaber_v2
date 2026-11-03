// RED/BLUE 노트 오브젝트를 관리하는 풀 — Instantiate/Destroy 대체
using System.Collections.Generic;
using UnityEngine;

public class NotePool : MonoBehaviour
{
    public enum NoteType { Red = 0, Blue = 1 }

    public static NotePool Instance { get; private set; }

    [Header("프리팹")]
    public GameObject redPrefab;
    public GameObject bluePrefab;

    [Header("초기 풀 크기")]
    public int initialPoolSize = 10;

    readonly Queue<GameObject> _redPool  = new();
    readonly Queue<GameObject> _bluePool = new();

    void Awake()
    {
        Instance = this;
        Prewarm(redPrefab,  _redPool);
        Prewarm(bluePrefab, _bluePool);
    }

    void Prewarm(GameObject prefab, Queue<GameObject> pool)
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get(NoteType type, Vector3 position, Quaternion rotation)
    {
        Queue<GameObject> pool = type == NoteType.Red ? _redPool : _bluePool;
        GameObject prefab      = type == NoteType.Red ? redPrefab : bluePrefab;

        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab, transform);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.transform.SetParent(null);
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj, NoteType type)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);

        if (type == NoteType.Red)
            _redPool.Enqueue(obj);
        else
            _bluePool.Enqueue(obj);
    }
}
