// 노트 앞면에 삼각형 방향 표시를 절차적으로 생성하는 컴포넌트
using UnityEngine;

public class NoteDirectionArrow : MonoBehaviour
{
    [Header("화살표 색상")]
    public Color arrowColor = Color.white;

    [Header("삼각형 크기 (노트 면 기준 0~0.5)")]
    [Range(0.1f, 0.45f)]
    public float size = 0.35f;

    [Header("노트 앞면 Z 오프셋")]
    [Tooltip("양수/음수를 바꾸면 반대 면에 표시됨")]
    public float faceOffset = -0.51f;

    [Header("머티리얼 (비워두면 기본 흰색)")]
    public Material arrowMaterial;

    void Awake()
    {
        BuildTriangle();
    }

    void BuildTriangle()
    {
        GameObject obj = new GameObject("DirectionTriangle");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(0f, 0f, faceOffset);
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale    = Vector3.one;

        MeshFilter   mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        mf.mesh = BuildMesh(size);

        // 머티리얼 적용
        if (arrowMaterial != null)
        {
            mr.material = arrowMaterial;
        }
        else
        {
            // 머티리얼 미지정 시 기본 URP Unlit 사용
            mr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        }

        // 색상 적용
        var block = new MaterialPropertyBlock();
        mr.GetPropertyBlock(block);
        block.SetColor("_BaseColor", arrowColor);
        mr.SetPropertyBlock(block);
    }

    Mesh BuildMesh(float s)
    {
        Mesh mesh = new Mesh();

        // 삼각형 꼭짓점 — 노트 면(1x1) 안에 맞게 배치
        Vector3[] verts =
        {
            new Vector3( 0f,        -s,         0f), // 아래 꼭짓점 (방향 끝)
            new Vector3(-s * 0.9f,   s * 0.7f,  0f), // 왼쪽 위
            new Vector3( s * 0.9f,   s * 0.7f,  0f), // 오른쪽 위
        };

        // 앞면 + 뒷면 모두 렌더링 (양면)
        int[] tris =
        {
            0, 2, 1, // 앞면 (플레이어 방향)
            0, 1, 2  // 뒷면
        };

        Vector3[] normals =
        {
            Vector3.back, Vector3.back, Vector3.back
        };

        mesh.vertices  = verts;
        mesh.triangles = tris;
        mesh.normals   = normals;
        mesh.RecalculateBounds();

        return mesh;
    }
}
