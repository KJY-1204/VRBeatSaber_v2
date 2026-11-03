// Beat Sage / Beat Saber v2 포맷 Normal.dat를 읽어 노트를 스폰하는 스포너
using System.Collections.Generic;
using UnityEngine;

public class BeatSageSpawner : MonoBehaviour
{
    [Header("노트 스폰 기준점 (4x3 그리드 중심)")]
    [Tooltip("노트가 플레이어 방향으로 날아오기 시작하는 위치")]
    public Transform spawnOrigin;

    [Header("그리드 간격")]
    public float cellWidth  = 0.4f; // 가로 간격
    public float cellHeight = 0.4f; // 세로 간격

    [Header("노트 타이밍")]
    [Tooltip("히트 타이밍 몇 초 전에 스폰할지. 이 값 × 노트 속도 = 스폰 거리")]
    public float lookAheadTime = 2f;
    [Tooltip("Cube.cs의 이동 속도와 동일하게 맞출 것")]
    public float noteSpeed = 2f;

    [Header("노트 밀도 조절")]
    [Tooltip("연속 노트 사이 최소 간격(초). 높을수록 노트 수 감소. 0=제한 없음")]
    public float minNoteInterval = 0.3f;

    // cutDirection → Z축 회전각 (NoteDirectionArrow 방향과 일치)
    static readonly float[] CutAngles = { 0f, 180f, 270f, 90f, 315f, 45f, 225f, 135f, 0f };

    BSBeatmapData    _beatmap;
    List<BSNoteData> _notes;
    int              _nextIndex;
    float            _bpm;
    float            _lastSpawnTime = -999f;

    void Start()
    {
        SongData song = GameSession.CurrentSong;
        if (song == null || song.beatSageNormalDat == null)
        {
            enabled = false;
            return;
        }

        _bpm     = song.bpm;
        _beatmap = JsonUtility.FromJson<BSBeatmapData>(song.beatSageNormalDat.text);

        // 박탄(bomb, type 3) 제외, 시간순 정렬
        _notes = _beatmap._notes.FindAll(n => n._type == 0 || n._type == 1);
        _notes.Sort((a, b) => a._time.CompareTo(b._time));

        _nextIndex = 0;
    }

    void Update()
    {
        if (_notes == null) return;

        float currentTime = AudioManager.Instance.audioSource.time;

        // lookAheadTime 초 뒤에 히트할 노트를 지금 스폰
        while (_nextIndex < _notes.Count &&
               BeatToSeconds(_notes[_nextIndex]._time) <= currentTime + lookAheadTime)
        {
            SpawnNote(_notes[_nextIndex]);
            _nextIndex++;
        }
    }

    void SpawnNote(BSNoteData note)
    {
        // 최소 간격 필터 — 너무 촘촘한 노트 건너뜀
        float noteTime = BeatToSeconds(note._time);
        if (minNoteInterval > 0f && noteTime - _lastSpawnTime < minNoteInterval) return;
        _lastSpawnTime = noteTime;

        NotePool.NoteType type = note._type == 0 ? NotePool.NoteType.Red : NotePool.NoteType.Blue;

        Vector3 spawnPos = GetSpawnPosition(note._lineIndex, note._lineLayer);

        // cutDirection으로 Z 회전 결정
        int dirIndex = Mathf.Clamp(note._cutDirection, 0, 8);
        Quaternion rot = spawnOrigin != null
            ? spawnOrigin.rotation * Quaternion.Euler(0f, 0f, CutAngles[dirIndex])
            : Quaternion.Euler(0f, 0f, CutAngles[dirIndex]);

        GameObject obj = NotePool.Instance.Get(type, spawnPos, rot);
        Cube ctrl = obj.GetComponent<Cube>();
        if (ctrl != null) ctrl.noteType = type;
    }

    // 박자(beat) → 초(second) 변환
    float BeatToSeconds(float beat) => beat * 60f / _bpm;

    // lineIndex(0~3), lineLayer(0~2) → 월드 좌표
    Vector3 GetSpawnPosition(int lineIndex, int lineLayer)
    {
        Vector3 origin    = spawnOrigin != null ? spawnOrigin.position : transform.position;
        Vector3 forward   = spawnOrigin != null ? spawnOrigin.forward  : transform.forward;

        // lookAheadTime 동안 이동할 거리만큼 앞에서 스폰
        float spawnDist = noteSpeed * lookAheadTime;

        float x = (lineIndex - 1.5f) * cellWidth;
        float y = lineLayer * cellHeight;

        return origin + forward * spawnDist + new Vector3(x, y, 0f);
    }
}
