// 노트 절단 시 두 조각이 튕겨 나가다가 페이드 아웃되는 이펙트
using System.Collections;
using UnityEngine;

public class SliceEffect : MonoBehaviour
{
    Rigidbody    _topRb,  _botRb;
    MeshRenderer _topMr,  _botMr;
    Material     _topMat, _botMat;

    void Awake()
    {
        GameObject top = CreateHalf("TopHalf", new Vector3(0f,  0.26f, 0f));
        GameObject bot = CreateHalf("BotHalf", new Vector3(0f, -0.26f, 0f));

        _topRb = top.GetComponent<Rigidbody>();
        _botRb = bot.GetComponent<Rigidbody>();
        _topMr = top.GetComponent<MeshRenderer>();
        _botMr = bot.GetComponent<MeshRenderer>();

        _topMat = CreateTransparentMat();
        _botMat = CreateTransparentMat();
        _topMr.material = _topMat;
        _botMr.material = _botMat;
    }

    // 조각 하나 생성
    GameObject CreateHalf(string halfName, Vector3 localPos)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = halfName;
        obj.transform.SetParent(transform);
        obj.transform.localPosition = localPos;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale    = new Vector3(0.95f, 0.48f, 0.95f);

        Rigidbody rb   = obj.AddComponent<Rigidbody>();
        rb.useGravity  = true;
        rb.isKinematic = false;

        // 조각끼리 충돌 불필요
        Destroy(obj.GetComponent<BoxCollider>());
        return obj;
    }

    // URP Unlit 반투명 머티리얼 생성
    Material CreateTransparentMat()
    {
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetFloat("_Surface", 1f);   // Transparent
        mat.SetFloat("_Blend",   0f);   // Alpha
        mat.SetFloat("_ZWrite",  0f);
        mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        return mat;
    }

    public void Play(Vector3 position, Quaternion rotation, Color color)
    {
        transform.position = position;
        transform.rotation = rotation;

        // 조각 위치/회전 초기화
        _topRb.transform.localPosition = new Vector3(0f,  0.26f, 0f);
        _botRb.transform.localPosition = new Vector3(0f, -0.26f, 0f);
        _topRb.transform.localRotation = Quaternion.identity;
        _botRb.transform.localRotation = Quaternion.identity;

        // 속도 초기화 후 새 힘 적용
        _topRb.linearVelocity  = Vector3.zero;
        _botRb.linearVelocity  = Vector3.zero;
        _topRb.angularVelocity = Vector3.zero;
        _botRb.angularVelocity = Vector3.zero;

        // 색상 적용
        color.a = 1f;
        _topMat.color = color;
        _botMat.color = color;

        gameObject.SetActive(true);

        // 위쪽 조각 — 위 + 옆으로 튕김
        float side = Random.Range(-1.5f, 1.5f);
        _topRb.AddForce(new Vector3(side,  Random.Range(2f, 4f), Random.Range(-1f, 1f)), ForceMode.VelocityChange);
        _topRb.AddTorque(Random.insideUnitSphere * 6f, ForceMode.VelocityChange);

        // 아래쪽 조각 — 아래 + 반대 옆으로 튕김
        _botRb.AddForce(new Vector3(-side, Random.Range(-3f, -1.5f), Random.Range(-1f, 1f)), ForceMode.VelocityChange);
        _botRb.AddTorque(Random.insideUnitSphere * 6f, ForceMode.VelocityChange);

        StartCoroutine(FadeAndReturn());
    }

    IEnumerator FadeAndReturn()
    {
        // 잠깐 날아가다가
        yield return new WaitForSeconds(0.25f);

        // 0.35초에 걸쳐 페이드 아웃
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.35f;
            float a = Mathf.Lerp(1f, 0f, t);
            SetAlpha(a);
            yield return null;
        }

        gameObject.SetActive(false);
        SliceEffectPool.Instance.Return(this);
    }

    void SetAlpha(float a)
    {
        Color tc = _topMat.color; tc.a = a; _topMat.color = tc;
        Color bc = _botMat.color; bc.a = a; _botMat.color = bc;
    }
}
