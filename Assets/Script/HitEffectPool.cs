// 노트 타격 시 재생되는 파티클 이펙트 풀
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectPool : MonoBehaviour
{
    public static HitEffectPool Instance { get; private set; }

    [Header("이펙트 프리팹 (ParticleSystem 포함)")]
    public GameObject redEffectPrefab;
    public GameObject blueEffectPrefab;

    [Header("초기 풀 크기")]
    public int initialPoolSize = 6;

    [Header("이펙트 색상")]
    public Color redColor  = new Color(1f, 0.25f, 0.05f);  // 붉은 오렌지
    public Color blueColor = new Color(0.05f, 0.6f, 1f);   // 밝은 시안

    readonly Queue<GameObject> _redPool  = new();
    readonly Queue<GameObject> _bluePool = new();

    void Awake()
    {
        Instance = this;
        Prewarm(redEffectPrefab,  _redPool);
        Prewarm(blueEffectPrefab, _bluePool);
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

    public void Play(Vector3 position, NotePool.NoteType type)
    {
        Queue<GameObject> pool  = type == NotePool.NoteType.Red ? _redPool : _bluePool;
        GameObject prefab       = type == NotePool.NoteType.Red ? redEffectPrefab : blueEffectPrefab;

        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab, transform);
        obj.transform.position = position;
        obj.SetActive(true);

        ParticleSystem ps = obj.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            // 노트 타입에 맞게 파티클 색상 적용
            var main = ps.main;
            main.startColor = type == NotePool.NoteType.Red ? redColor : blueColor;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
            StartCoroutine(ReturnAfter(obj, type, ps.main.duration + ps.main.startLifetime.constantMax));
        }
        else
        {
            // ParticleSystem이 없는 경우 0.5초 후 반환
            StartCoroutine(ReturnAfter(obj, type, 0.5f));
        }
    }

    IEnumerator ReturnAfter(GameObject obj, NotePool.NoteType type, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        if (type == NotePool.NoteType.Red) _redPool.Enqueue(obj);
        else                               _bluePool.Enqueue(obj);
    }
}
