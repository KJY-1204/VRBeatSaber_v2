// 점수와 콤보를 TextMeshPro UI에 표시하는 컴포넌트
using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("UI 텍스트")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    Coroutine _comboAnim;
    Coroutine _comboFade;
    Color _comboBaseColor; // Inspector에서 설정한 색상 보존

    void Start()
    {
        _comboBaseColor = comboText.color;
        ScoreManager.Instance.OnScoreChanged += UpdateScore;
        ScoreManager.Instance.OnComboChanged += UpdateCombo;
        UpdateScore(ScoreManager.Instance.Score);
        UpdateCombo(ScoreManager.Instance.Combo);
    }

    void OnDisable()
    {
        if (ScoreManager.Instance == null) return;
        ScoreManager.Instance.OnScoreChanged -= UpdateScore;
        ScoreManager.Instance.OnComboChanged -= UpdateCombo;
    }

    void UpdateScore(int score) => scoreText.text = $"{score:N0}";

    void UpdateCombo(int combo)
    {
        if (combo > 1)
        {
            comboText.text = $"{combo}\n<size=50%>COMBO</size>";

            // 기존 애니메이션 중단 후 팝업 재생
            if (_comboAnim != null) StopCoroutine(_comboAnim);
            if (_comboFade != null) StopCoroutine(_comboFade);
            _comboAnim = StartCoroutine(PopCombo());
        }
        else
        {
            // Miss — 콤보 텍스트 페이드 아웃
            if (_comboAnim != null) StopCoroutine(_comboAnim);
            if (_comboFade != null) StopCoroutine(_comboFade);
            _comboFade = StartCoroutine(FadeOutCombo());
        }
    }

    // 콤보 증가 시 크게 튀어오르고 원래 크기로 돌아옴
    IEnumerator PopCombo()
    {
        _comboBaseColor.a = 1f;
        comboText.color = _comboBaseColor;
        comboText.transform.localScale = Vector3.one * 1.4f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.15f;
            comboText.transform.localScale = Vector3.Lerp(
                Vector3.one * 1.4f, Vector3.one, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        comboText.transform.localScale = Vector3.one;
    }

    // Miss 시 페이드 아웃
    IEnumerator FadeOutCombo()
    {
        Color c = comboText.color;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.3f;
            c.a = Mathf.Lerp(1f, 0f, t);
            comboText.color = c;
            yield return null;
        }
        comboText.text = "";
        _comboBaseColor.a = 1f;
        comboText.color = _comboBaseColor;
    }
}
