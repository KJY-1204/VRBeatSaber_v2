// 비트맵 JSON을 역직렬화하는 데이터 클래스
using System;
using System.Collections.Generic;

[Serializable]
public class NoteData
{
    public float time;      // 곡 시작 기준 스폰 시각 (초)
    public int   type;      // 0 = Red, 1 = Blue
    public int   position;  // 0~3, 스폰 위치 인덱스
    public int   direction; // 0~3, 회전 (0=0°, 1=90°, 2=180°, 3=270°)
}

[Serializable]
public class BeatmapData
{
    public List<NoteData> notes = new();
}
