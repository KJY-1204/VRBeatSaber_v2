// 타이틀 화면에서 곡 선택 씬으로 전환하는 UI
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroUI : MonoBehaviour
{
    [Header("배경음 컴포넌트 (같은 오브젝트 또는 씬 내 오브젝트)")]
    public IntroBGM introBGM;

    public void OnPlay()
    {
        if (introBGM != null)
            introBGM.PlayAndTransit("SongSelect");
        else
            SceneManager.LoadScene("SongSelect");
    }
}
