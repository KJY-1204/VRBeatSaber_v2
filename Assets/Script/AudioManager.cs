// 음악 재생과 BPM 기반 박자 이벤트를 관리하는 매니저
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("오디오")]
    public AudioSource audioSource;

    [Header("BPM 설정")]
    [Tooltip("곡의 BPM. 박자 간격 = 60 / BPM 초")]
    public float bpm = 120f;

    // 한 박자마다 호출. 구독자: Spawner
    public event Action OnBeat;
    // 곡 재생이 끝났을 때 호출. 구독자: GameManager
    public event Action OnSongEnd;

    double _nextBeatDspTime;
    double _beatInterval;
    bool   _isPlaying;
    bool   _songEndFired;

    void Awake()
    {
        Instance = this;
    }

    // 재생은 GameManager가 전담 — 여기서 자동 시작하지 않음

    public void Play()
    {
        _beatInterval    = 60.0 / bpm;
        _nextBeatDspTime = AudioSettings.dspTime + _beatInterval;
        audioSource.Play();
        _isPlaying    = true;
        _songEndFired = false;
    }

    public void Stop()
    {
        audioSource.Stop();
        _isPlaying = false;
    }

    void Update()
    {
        if (!_isPlaying) return;

        // dspTime 기준으로 박자 체크 — 프레임 독립적
        while (AudioSettings.dspTime >= _nextBeatDspTime)
        {
            OnBeat?.Invoke();
            _nextBeatDspTime += _beatInterval;
        }

        // 곡 종료 감지 — 중복 호출 방지
        if (!audioSource.isPlaying && !_songEndFired)
        {
            _isPlaying    = false;
            _songEndFired = true;
            OnSongEnd?.Invoke();
        }
    }
}
