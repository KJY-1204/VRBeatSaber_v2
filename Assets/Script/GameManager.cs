// 게임플레이 씬의 시작과 종료를 제어하는 매니저
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Tooltip("GameSession.CurrentSong이 없을 때 사용할 기본 곡")]
    public SongData fallbackSong;

    void Awake() => Instance = this;

    void Start()
    {
        SongData song = GameSession.CurrentSong ?? fallbackSong;

        if (song != null)
        {
            AudioManager.Instance.audioSource.clip = song.clip;
            AudioManager.Instance.bpm              = song.bpm;
        }

        AudioManager.Instance.OnSongEnd += OnSongEnd;
        ScoreManager.Instance.OnGameOver += OnGameOver;
        AudioManager.Instance.Play();
    }

    void OnDestroy()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnSongEnd -= OnSongEnd;
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnGameOver -= OnGameOver;
    }

    void OnSongEnd() => EndGame();

    void OnGameOver()
    {
        AudioManager.Instance.Stop();
        EndGame();
    }

    void EndGame()
    {
        GameSession.FinalScore   = ScoreManager.Instance.Score;
        GameSession.MaxCombo     = ScoreManager.Instance.MaxCombo;
        GameSession.PerfectCount = ScoreManager.Instance.PerfectCount;
        GameSession.MissCount    = ScoreManager.Instance.MissCount;
        SceneManager.LoadScene("Result");
    }
}
