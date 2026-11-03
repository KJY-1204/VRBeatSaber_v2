// 오디오 스펙트럼을 실시간으로 읽어 원형 이퀄라이저 바를 렌더링하는 컴포넌트
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    [Header("바 설정")]
    [Tooltip("이퀄라이저 바 개수")]
    public int barCount = 64;
    [Tooltip("원의 반지름")]
    public float radius = 5f;
    [Tooltip("바 하나의 가로/두께 크기")]
    public float barWidth = 0.15f;
    [Tooltip("바 최대 길이 (원 바깥쪽으로 뻗는 거리)")]
    public float maxHeight = 3f;
    [Tooltip("높이 변화 부드러움 (값이 클수록 빠르게 반응)")]
    public float smoothSpeed = 10f;

    [Header("색상")]
    [Tooltip("낮은 진폭일 때 색상")]
    public Color lowColor  = new Color(0.2f, 0.4f, 1.0f, 0.5f);
    [Tooltip("높은 진폭일 때 색상")]
    public Color highColor = new Color(1.0f, 0.2f, 0.4f, 0.9f);

    [Header("머티리얼")]
    [Tooltip("바에 사용할 머티리얼. 비워두면 기본 머티리얼 사용")]
    public Material barMaterial;

    const int SpectrumSize = 512;

    readonly float[] _spectrum   = new float[SpectrumSize];
    float[]          _barHeights;
    Transform[]      _bars;
    Vector3[]        _barDirs;       // 각 바의 방사 방향
    Renderer[]       _renderers;
    MaterialPropertyBlock _propBlock;

    void Awake()
    {
        _barHeights = new float[barCount];
        _bars       = new Transform[barCount];
        _barDirs    = new Vector3[barCount];
        _renderers  = new Renderer[barCount];
        _propBlock  = new MaterialPropertyBlock();

        for (int i = 0; i < barCount; i++)
        {
            float angle = i * Mathf.PI * 2f / barCount;
            Vector3 dir = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
            _barDirs[i] = dir;

            GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bar.name = $"Bar_{i}";
            bar.transform.SetParent(transform);

            // 바의 Y축이 원 바깥을 향하도록 회전
            bar.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir);
            bar.transform.localScale    = new Vector3(barWidth, 0.01f, barWidth);
            bar.transform.localPosition = dir * radius;

            Destroy(bar.GetComponent<Collider>());

            if (barMaterial != null)
                bar.GetComponent<Renderer>().material = barMaterial;

            _bars[i]      = bar.transform;
            _renderers[i] = bar.GetComponent<Renderer>();
        }
    }

    void Update()
    {
        if (AudioManager.Instance == null || !AudioManager.Instance.audioSource.isPlaying)
            return;

        AudioManager.Instance.audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);

        int samplesPerBar = SpectrumSize / barCount;

        for (int i = 0; i < barCount; i++)
        {
            float sum = 0f;
            for (int j = 0; j < samplesPerBar; j++)
                sum += _spectrum[i * samplesPerBar + j];

            float targetHeight = Mathf.Clamp(sum / samplesPerBar * 300f, 0.01f, maxHeight);

            _barHeights[i] = Mathf.Lerp(_barHeights[i], targetHeight, Time.deltaTime * smoothSpeed);

            float h = Mathf.Max(_barHeights[i], 0.01f);

            // 바의 뿌리가 원 위에 고정되고 바깥쪽으로 성장
            _bars[i].localScale    = new Vector3(barWidth, h, barWidth);
            _bars[i].localPosition = _barDirs[i] * (radius + h / 2f);

            // 진폭에 따라 색상 그라디언트
            float t = Mathf.InverseLerp(0f, maxHeight, h);
            Color c = Color.Lerp(lowColor, highColor, t);
            _renderers[i].GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_BaseColor", c);
            _renderers[i].SetPropertyBlock(_propBlock);
        }
    }
}
