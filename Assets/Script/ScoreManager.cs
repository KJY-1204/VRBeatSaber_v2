// 점수와 콤보를 관리하는 싱글톤 매니저
using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score        { get; private set; }
    public int Combo        { get; private set; }
    public int MaxCombo     { get; private set; }
    public int PerfectCount { get; private set; }
    public int MissCount    { get; private set; }

    [Header("게임 오버 조건")]
    [Tooltip("이 횟수만큼 미스하면 게임 오버")]
    public int maxMisses = 10;

    // UI 등 외부에서 구독
    public event Action<int> OnScoreChanged;
    public event Action<int> OnComboChanged;
    public event Action      OnNoteMiss;
    public event Action      OnGameOver;

    // 콤보 구간별 배율: 1~9 → x1, 10~19 → x2, 20+ → x4
    static readonly int[] ComboThresholds    = { 20, 10, 0 };
    static readonly int[] ComboMultipliers   = {  4,  2, 1 };

    void Awake() => Instance = this;

    public void NoteHit()
    {
        Combo++;
        PerfectCount++;
        if (Combo > MaxCombo) MaxCombo = Combo;
        Score += 100 * GetMultiplier();
        OnComboChanged?.Invoke(Combo);
        OnScoreChanged?.Invoke(Score);
    }

    public void NoteMiss()
    {
        MissCount++;
        OnNoteMiss?.Invoke();

        if (MissCount >= maxMisses)
        {
            OnGameOver?.Invoke();
            return;
        }

        if (Combo == 0) return;
        Combo = 0;
        OnComboChanged?.Invoke(Combo);
    }

    int GetMultiplier()
    {
        for (int i = 0; i < ComboThresholds.Length; i++)
            if (Combo >= ComboThresholds[i]) return ComboMultipliers[i];
        return 1;
    }
}
