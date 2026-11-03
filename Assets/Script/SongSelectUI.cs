// 곡 선택 화면 — 버튼 클릭 시 해당 곡을 GameSession에 저장하고 Game 씬 로드
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongSelectUI : MonoBehaviour
{
    [Header("곡 목록")]
    public SongData[] songs;

    [Header("버튼 생성 기준 부모")]
    public Transform buttonContainer;

    [Header("버튼 프리팹 (Button + TMP_Text 포함)")]
    public GameObject songButtonPrefab;

    [Header("미리듣기 AudioSource")]
    public AudioSource previewSource;

    [Header("미리듣기 페이드 시간 (초)")]
    public float fadeTime = 0.4f;

    [Header("버튼 호버 색상")]
    public Color normalColor  = Color.white;
    public Color hoverColor   = new Color(1f, 0.85f, 0.2f); // 노란빛

    Coroutine _fadeCoroutine;

    void Start()
    {
        foreach (SongData song in songs)
        {
            GameObject btn = Instantiate(songButtonPrefab, buttonContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text =
                $"{song.songName}\n<size=70%>{song.artist}</size>";

            SongData captured = song;
            btn.GetComponent<Button>().onClick.AddListener(() => SelectSong(captured));

            TextMeshProUGUI label = btn.GetComponentInChildren<TextMeshProUGUI>();

            // 호버 이벤트 등록
            EventTrigger trigger = btn.AddComponent<EventTrigger>();

            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener(_ =>
            {
                StartPreview(captured);
                if (label != null) label.color = hoverColor;
            });
            trigger.triggers.Add(enterEntry);

            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener(_ =>
            {
                StopPreview();
                if (label != null) label.color = normalColor;
            });
            trigger.triggers.Add(exitEntry);
        }
    }

    void SelectSong(SongData song)
    {
        StopPreview();
        GameSession.CurrentSong = song;
        SceneManager.LoadScene("Game");
    }

    void StartPreview(SongData song)
    {
        if (previewSource == null || song.clip == null) return;

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadePreview(song));
    }

    void StopPreview()
    {
        if (previewSource == null) return;

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeOut());
    }

    IEnumerator FadePreview(SongData song)
    {
        // 기존 재생 중이면 먼저 페이드 아웃
        if (previewSource.isPlaying)
        {
            float startVol = previewSource.volume;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / fadeTime;
                previewSource.volume = Mathf.Lerp(startVol, 0f, t);
                yield return null;
            }
            previewSource.Stop();
        }

        // 새 곡 페이드 인
        previewSource.clip = song.clip;
        previewSource.time = song.previewStartTime;
        previewSource.volume = 0f;
        previewSource.Play();

        float t2 = 0f;
        while (t2 < 1f)
        {
            t2 += Time.deltaTime / fadeTime;
            previewSource.volume = Mathf.Lerp(0f, 1f, t2);
            yield return null;
        }
        previewSource.volume = 1f;
    }

    IEnumerator FadeOut()
    {
        float startVol = previewSource.volume;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeTime;
            previewSource.volume = Mathf.Lerp(startVol, 0f, t);
            yield return null;
        }
        previewSource.Stop();
        previewSource.volume = 1f;
    }
}
