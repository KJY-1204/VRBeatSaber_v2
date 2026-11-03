# context-notes.md — 기술 결정 기록

## 2026-05-14

### [분석] 기존 코드 구조 파악

**Cube.cs 이동 방식.**
`transform.forward * 2 * Time.deltaTime` — 월드 forward가 아닌 오브젝트 로컬 forward를 사용함.
Spawner에서 `obj.transform.Rotate(transform.forward, 90 * Random.Range(0, 4))` 로 회전을 주므로,
노트마다 이동 방향이 달라질 가능성 있음. Phase 1에서는 건드리지 않고 현상 유지.
→ 향후 노트 이동 방향을 월드 -Z로 고정할지 논의 필요.

**Saber.cs Raycast 거리.**
`Physics.Raycast(..., 1, layer)` — 최대 감지 거리 1 유닛.
VR 세이버 길이에 따라 부족할 수 있음. 현재는 수정하지 않음.

**Spawner.cs `timer -= beat` 방식.**
타이머를 0으로 리셋하지 않고 차감 — 누적 오차 방지용으로 의도된 설계로 판단.
이벤트 기반으로 교체 시 이 패턴은 불필요해짐.

---

### [결정] Object Pooling 방식 — `Queue<GameObject>` 직접 관리

**선택.** `Queue<GameObject>` 기반 수동 풀

**대안.**
- Unity 6 내장 `ObjectPool<T>` (`UnityEngine.Pool`)
- 직접 `Queue<GameObject>` 구현

**이유.**
- Unity 6 `UnityEngine.Pool.ObjectPool<T>`이 존재하지만, MonoBehaviour 기반 오브젝트에서는 직접 Queue 관리가 더 명시적이고 디버깅이 쉬움.
- 복잡한 제네릭 래퍼보다 코드 가독성 우선.
- 단, 내부적으로 `UnityEngine.Pool`을 써도 무방 — 구현 시 최종 결정.

---

### [결정] BPM 타이밍 기준 — `AudioSettings.dspTime`

**선택.** `AudioSettings.dspTime`

**대안.**
- `Time.time` — Update 루프 기준, 프레임 의존적
- `AudioSource.time` — 재생 위치 기준, 일시정지 처리 간편
- `AudioSettings.dspTime` — 오디오 DSP 스레드 기준, 가장 정밀

**이유.**
- VR 리듬 게임에서 타이밍 정밀도가 핵심.
- `Time.time`은 프레임 드랍 시 오차 발생.
- `AudioSettings.dspTime`은 오디오 스레드 기준으로 프레임레이트 독립적.
- `AudioSource.PlayScheduled(dspTime)`와 조합하면 스폰 이벤트도 같은 타임라인에서 관리 가능.

---

### [결정] 이벤트 방식 — `System.Action` 델리게이트

**선택.** `System.Action` (C# 표준 델리게이트)

**대안.**
- `UnityEvent` — Inspector 연결 가능, 오버헤드 있음
- `System.Action` — 코드 연결, 오버헤드 최소

**이유.**
- 리듬 게임 박자 이벤트는 매 초 여러 번 발생 → UnityEvent 오버헤드 불필요.
- 명시적 구독/해제(`+=`/`-=`)로 메모리 누수 방지 관리 용이.

---

### [결정] AudioManager 참조 방식 — 싱글톤

**선택.** 싱글톤 (`AudioManager.Instance`)

**이유.**
- Spawner, NoteController 모두 AudioManager를 참조해야 함.
- 씬에 하나만 존재하는 게임 매니저 성격 → 싱글톤 적합.
- `DontDestroyOnLoad` 여부는 씬 구조 확정 후 결정.

---

### [보류] Cube.cs 이동 방향 통일

노트 이동 방향(`transform.forward` vs 월드 `-Z`)이 현재 불명확.
Spawner의 Rotate 로직과 함께 Phase 2에서 재검토 예정.

---

## 2026-05-14 — Milestone 1 완료

**완료된 파일.**
- `NotePool.cs` — Queue 기반 RED/BLUE 풀, 싱글톤
- `AudioManager.cs` — dspTime 기준 OnBeat 이벤트, 싱글톤
- `Spawner.cs` — Update 타이머 제거, OnBeat 구독, NotePool.Get 사용
- `Cube.cs` — 이동 보존, despawnZ 통과 시 NotePool.Return
- `Saber.cs` — Destroy → NotePool.Return 1줄 교체

**씬 연결 (1-F)은 사용자가 Unity에서 직접 진행.**
- AudioClip 없이도 BPM=120 기준으로 스폰 테스트 가능.
- AudioManager.Play()가 clip null 체크를 하므로, 테스트 시 수동 호출 필요.

**다음 단계 — Milestone 2.**
- ScoreManager (콤보, 점수)
- HitEffect (파티클)
- Miss 판정 (노트 통과 시 콤보 초기화)
