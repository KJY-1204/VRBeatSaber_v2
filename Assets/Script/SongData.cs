// 곡 한 곡의 메타데이터를 담는 ScriptableObject
using UnityEngine;

[CreateAssetMenu(fileName = "SongData", menuName = "VRBeatSaber/SongData")]
public class SongData : ScriptableObject
{
    public string songName;
    public string artist;
    public AudioClip clip;
    [Tooltip("곡의 BPM — AudioManager에 적용됨")]
    public float bpm = 120f;
    public Sprite    albumArt;
    [Tooltip("비트맵 JSON 파일. 없으면 Spawner(랜덤 스폰)가 동작함")]
    public TextAsset beatmapJson;
    [Tooltip("Beat Saber / Beat Sage Normal.dat 파일")]
    public TextAsset beatSageNormalDat;
    [Tooltip("미리듣기 시작 시간 (초). 0이면 곡 처음부터 재생")]
    public float previewStartTime = 0f;
}
