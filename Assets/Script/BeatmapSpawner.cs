// 비트맵 JSON을 읽어 정해진 타이밍에 노트를 스폰하는 스포너
using UnityEngine;

public class BeatmapSpawner : MonoBehaviour
{
    public Transform[] point;

    BeatmapData _beatmap;
    int         _nextIndex;

    void Start()
    {
        SongData song = GameSession.CurrentSong;
        if (song == null || song.beatmapJson == null)
        {
            Debug.LogWarning("BeatmapSpawner: SongData 또는 beatmapJson이 없습니다. Spawner를 사용하세요.");
            enabled = false;
            return;
        }

        _beatmap   = JsonUtility.FromJson<BeatmapData>(song.beatmapJson.text);
        _nextIndex = 0;
    }

    void Update()
    {
        if (_beatmap == null) return;

        float currentTime = AudioManager.Instance.audioSource.time;

        while (_nextIndex < _beatmap.notes.Count &&
               currentTime >= _beatmap.notes[_nextIndex].time)
        {
            SpawnNote(_beatmap.notes[_nextIndex]);
            _nextIndex++;
        }
    }

    void SpawnNote(NoteData note)
    {
        if (note.position < 0 || note.position >= point.Length) return;

        NotePool.NoteType type = (NotePool.NoteType)note.type;
        Quaternion rot = point[note.position].rotation *
                         Quaternion.AngleAxis(90f * note.direction, Vector3.forward);

        GameObject obj = NotePool.Instance.Get(type, point[note.position].position, rot);

        Cube ctrl = obj.GetComponent<Cube>();
        if (ctrl != null) ctrl.noteType = type;
    }
}
