// 오디오 스펙트럼을 실시간 분석해 비트를 감지하는 컴포넌트
using System;
using UnityEngine;

public class BeatDetector : MonoBehaviour
{
    public static BeatDetector Instance { get; private set; }

    [Header("분석 설정")]
    [Tooltip("분석할 저주파 밴드 수. 값이 클수록 베이스 외 음역도 반응")]
    [Range(4, 64)]
    public int frequencyBands = 16;

    [Tooltip("비트 감지 민감도. 낮을수록 민감 (1.2 ~ 2.0 권장)")]
    [Range(1.0f, 3.0f)]
    public float sensitivity = 1.5f;

    [Tooltip("비트 감지 후 재감지까지 최소 간격 (초)")]
    [Range(0.1f, 1.0f)]
    public float cooldown = 0.2f;

    // 비트 감지 시 호출. Spawner가 구독
    public event Action OnBeatDetected;

    const int SpectrumSize = 512;

    readonly float[] _spectrum      = new float[SpectrumSize];
    readonly float[] _energyHistory = new float[43]; // 약 0.7초 이력 (60fps 기준)
    int   _historyIndex;
    float _cooldownTimer;
    float _warmupTimer = 0.8f; // 히스토리가 채워질 때까지 감지 억제

    void Awake() => Instance = this;

    void Update()
    {
        if (!AudioManager.Instance.audioSource.isPlaying) return;

        if (_warmupTimer > 0f)
        {
            _warmupTimer -= Time.deltaTime;
            // 워밍업 중에도 히스토리는 채워서 평균 안정화
            AudioManager.Instance.audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
            float e = 0f;
            for (int i = 0; i < frequencyBands; i++) e += _spectrum[i];
            _energyHistory[_historyIndex] = e;
            _historyIndex = (_historyIndex + 1) % _energyHistory.Length;
            return;
        }

        _cooldownTimer -= Time.deltaTime;

        // 현재 프레임 저주파 에너지 계산
        AudioManager.Instance.audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float currentEnergy = 0f;
        for (int i = 0; i < frequencyBands; i++)
            currentEnergy += _spectrum[i];

        // 이력 평균 계산
        _energyHistory[_historyIndex] = currentEnergy;
        _historyIndex = (_historyIndex + 1) % _energyHistory.Length;

        float averageEnergy = 0f;
        foreach (float e in _energyHistory)
            averageEnergy += e;
        averageEnergy /= _energyHistory.Length;

        // 현재 에너지가 평균 × 민감도를 초과하면 비트로 판정
        if (currentEnergy > averageEnergy * sensitivity && _cooldownTimer <= 0f)
        {
            _cooldownTimer = cooldown;
            OnBeatDetected?.Invoke();
        }
    }
}
