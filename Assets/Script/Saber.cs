using UnityEngine;

public class Saber : MonoBehaviour
{
    public LayerMask layer;

    [Header("이 사버의 색 타입 (Red / Blue)")]
    public NotePool.NoteType saberType = NotePool.NoteType.Red;

    Vector3 prevPos;

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 1, layer))
        {
            Cube note = hit.transform.GetComponentInParent<Cube>();
            if (note == null || note.noteType != saberType) return;

            Vector3 v1 = transform.position - prevPos;
            if (Vector3.Angle(v1, note.transform.up) > 100)
            {
                note.isHit = true;
                HitEffectPool.Instance.Play(note.transform.position, saberType);
                SliceEffectPool.Instance?.Play(note.transform.position, note.transform.rotation, saberType);
                ScoreManager.Instance.NoteHit();
                NotePool.Instance.Return(note.gameObject, saberType);
            }
        }
        prevPos = transform.position;
    }
}
