// 씬 전환 시 선택된 곡과 최종 점수를 보존하는 정적 데이터 컨테이너
public static class GameSession
{
    public static SongData CurrentSong;
    public static int      FinalScore;
    public static int      MaxCombo;
    public static int      PerfectCount;
    public static int      MissCount;
}
