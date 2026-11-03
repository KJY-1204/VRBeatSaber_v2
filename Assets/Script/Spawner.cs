// BPM 박자 또는 BeatDetector 이벤트에 맞춰 노트를 스폰하는 스포너
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] point;

    void Start()
    {
        if (BeatDetector.Instance != null)
            BeatDetector.Instance.OnBeatDetected += SpawnNote;
        else if (AudioManager.Instance != null)
            AudioManager.Instance.OnBeat += SpawnNote;
        else
            Debug.LogError("Spawner: AudioManager와 BeatDetector가 모두 없습니다. 씬에 추가해 주세요.");
    }

    void OnDisable()
    {
        if (BeatDetector.Instance != null)
            BeatDetector.Instance.OnBeatDetected -= SpawnNote;
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnBeat -= SpawnNote;
    }

    void SpawnNote()
    {
        int p = Random.Range(0, point.Length);
        NotePool.NoteType type = (NotePool.NoteType)Random.Range(0, 2);

        Quaternion rot = point[p].rotation *
                         Quaternion.AngleAxis(90f * Random.Range(0, 4), Vector3.forward);

        GameObject obj = NotePool.Instance.Get(type, point[p].position, rot);

        Cube ctrl = obj.GetComponent<Cube>();
        if (ctrl != null) ctrl.noteType = type;
    }
}
