// Intro 씬 배경음을 페이드 인/아웃으로 재생하는 컴포넌트
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroBGM : MonoBehaviour
{
    [Header("배경음")]
    public AudioClip bgmClip;

    [Header("볼륨")]
    [Range(0f, 1f)]
    public float targetVolume = 0.6f;

    [Header("페이드 인 시간 (초)")]
    public float fadeInTime = 1.5f;

    [Header("씬 전환 시 페이드 아웃 시간 (초)")]
    public float fadeOutTime = 0.8f;

    AudioSource _source;

    void Awake()
    {
        _source = gameObject.AddComponent<AudioSource>();
        _source.clip   = bgmClip;
        _source.loop   = true;
        _source.volume = 0f;
        _source.playOnAwake = false;
    }

    void Start()
    {
        if (bgmClip == null) return;
        _source.Play();
        StartCoroutine(FadeIn());
    }

    // IntroUI.OnPlay() 에서 호출 — 페이드 아웃 후 씬 전환
    public void PlayAndTransit(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeInTime;
            _source.volume = Mathf.Lerp(0f, targetVolume, t);
            yield return null;
        }
        _source.volume = targetVolume;
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        float startVol = _source.volume;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeOutTime;
            _source.volume = Mathf.Lerp(startVol, 0f, t);
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }
}
