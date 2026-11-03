// 게임 종료 후 점수와 최대 콤보를 표시하는 결과 화면
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultUI : MonoBehaviour
{
    [Header("결과 텍스트")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI maxComboText;
    public TextMeshProUGUI songNameText;
    public TextMeshProUGUI perfectText;
    public TextMeshProUGUI missText;

    void Start()
    {
        scoreText.text    = $"{GameSession.FinalScore:N0}";
        maxComboText.text = $"MAX COMBO  {GameSession.MaxCombo}";
        songNameText.text = GameSession.CurrentSong != null
            ? GameSession.CurrentSong.songName
            : "";
        perfectText.text  = $"PERFECT  {GameSession.PerfectCount}";
        missText.text     = $"MISS  {GameSession.MissCount}";
    }

    public void OnPlayAgain()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnBackToMenu()
    {
        GameSession.CurrentSong = null;
        SceneManager.LoadScene("SongSelect"); 
    }
}
