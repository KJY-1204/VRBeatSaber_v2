// 노트의 이동과 풀 반환을 담당하는 컨트롤러
using UnityEngine;

public class Cube : MonoBehaviour
{
    [Tooltip("플레이어 기준 이 Z 거리를 지나면 풀로 반환")]
    public float despawnZ = -2f;

    [HideInInspector] public NotePool.NoteType noteType;
    // Saber가 타격하면 true — 통과 판정과 구분하기 위해 사용
    [HideInInspector] public bool isHit;

    void OnEnable()
    {
        isHit = false;
    }

    void Update()
    {
        transform.position += Time.deltaTime * transform.forward * 2;

        if (transform.position.z < despawnZ)
        {
            if (!isHit)
                ScoreManager.Instance.NoteMiss();

            NotePool.Instance.Return(gameObject, noteType);
        }
    }
}
