// 절단 이펙트 오브젝트 풀 관리
using System.Collections.Generic;
using UnityEngine;

public class SliceEffectPool : MonoBehaviour
{
    public static SliceEffectPool Instance { get; private set; }

    [Header("초기 풀 크기")]
    public int initialSize = 8;

    readonly Queue<SliceEffect> _pool = new();

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < initialSize; i++)
            _pool.Enqueue(CreateNew());
    }

    SliceEffect CreateNew()
    {
        GameObject obj = new GameObject("SliceEffect");
        obj.transform.SetParent(transform);
        SliceEffect effect = obj.AddComponent<SliceEffect>(); // Awake() 즉시 실행
        obj.SetActive(false);
        return effect;
    }

    public void Play(Vector3 position, Quaternion rotation, NotePool.NoteType type)
    {
        Color color = type == NotePool.NoteType.Red
            ? new Color(1f, 0.2f, 0.1f)
            : new Color(0.1f, 0.55f, 1f);

        SliceEffect effect = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
        effect.Play(position, rotation, color);
    }

    public void Return(SliceEffect effect)
    {
        effect.transform.SetParent(transform);
        _pool.Enqueue(effect);
    }
}
