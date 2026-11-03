// 노트 미스 시 MISS 텍스트가 나타났다가 페이드 아웃되는 UI
using System.Collections;
using TMPro;
using UnityEngine;

public class MissUI : MonoBehaviour
{
    [Header("MISS 텍스트")]
    public TextMeshProUGUI missText;

    [Header("표시 유지 시간 (초)")]
    public float holdTime = 0.4f;

    [Header("페이드 아웃 시간 (초)")]
    public float fadeTime = 0.3f;

    Coroutine _anim;
    Color _baseColor; // Inspector에서 설정한 색상 보존

    void Start()
    {
        _baseColor = missText.color;
        _baseColor.a = 0f;
        missText.color = _baseColor; // 처음엔 투명
        ScoreManager.Instance.OnNoteMiss += ShowMiss;
    }

    void OnDisable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnNoteMiss -= ShowMiss;
    }

    void ShowMiss()
    {
        if (_anim != null) StopCoroutine(_anim);
        _anim = StartCoroutine(PlayAnim());
    }

    IEnumerator PlayAnim()
    {
        // 즉시 표시 (Inspector 색상 그대로, 불투명)
        _baseColor.a = 1f;
        missText.color = _baseColor;
        missText.transform.localScale = Vector3.one * 1.3f;

        // 크기 팝업
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.12f;
            missText.transform.localScale = Vector3.Lerp(
                Vector3.one * 1.3f, Vector3.one, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        // 잠깐 유지
        yield return new WaitForSeconds(holdTime);

        // 페이드 아웃
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeTime;
            Color c = missText.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            missText.color = c;
            yield return null;
        }

        _baseColor.a = 0f;
        missText.color = _baseColor;
    }
}
