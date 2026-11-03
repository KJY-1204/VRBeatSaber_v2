// Beat Saber v2 포맷 비트맵 데이터 클래스 (Beat Sage 호환)
using System;
using System.Collections.Generic;

[Serializable]
public class BSNoteData
{
    public float _time;        // 박자 단위 시간 (초로 변환 필요)
    public int   _lineIndex;   // 가로 위치 0~3 (왼→오른)
    public int   _lineLayer;   // 세로 위치 0~2 (아래→위)
    public int   _type;        // 0=Red, 1=Blue, 3=Bomb
    public int   _cutDirection;// 0=Up 1=Down 2=Left 3=Right 4=UpLeft 5=UpRight 6=DownLeft 7=DownRight 8=Any
}

[Serializable]
public class BSBeatmapData
{
    public List<BSNoteData> _notes = new();
}
