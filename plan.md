# VRBeatSaber_v2 개발 계획

## 현재 구현 상태

| 파일 | 역할 | 상태 |
|---|---|---|
| `Cube.cs` | 노트 이동 (`transform.forward * 2`) | 완료 |
| `Spawner.cs` | 노트 생성 (`Instantiate`, beat=1초 고정) | 부분 완료 |
| `Saber.cs` | 타격 판정 (Raycast + 각도 >100°, `Destroy`) | 완료 |

**핵심 문제점.**
1. `Spawner.cs`가 `Instantiate`/`Destroy`를 사용 → VR 환경에서 GC 스파이크 발생 가능
2. 노트 스폰 타이밍이 음악 BPM과 무관 → 게임성 핵심 요소 미구현
3. 노트가 플레이어를 통과해도 회수되지 않음 → 메모리 누수

---

## 목표 아키텍처

```
[AudioManager]
    ├─ AudioSource 제어 (재생/정지/시간 조회)
    ├─ BPM 기반 박자 이벤트 브로드캐스트
    └─ DSP 타임 기준 타이밍 동기화

[NotePool]
    ├─ RED / BLUE 프리팹 풀 관리
    ├─ Get(type, position, rotation) → 활성화
    └─ Return(note) → 비활성화 (Destroy 대체)

[BeatmapSpawner] ← Spawner.cs 확장
    ├─ AudioManager 이벤트 구독
    ├─ 박자마다 NotePool에서 노트 꺼내 배치
    └─ 향후 BeatmapData(JSON) 로드 지원 예정

[NoteController] ← Cube.cs 확장
    ├─ 이동 속도 외부 주입 (AudioManager의 BPM 연동)
    ├─ 플레이어 통과 시 NotePool.Return() 자동 호출
    └─ 풀에서 꺼낼 때 상태 초기화

[Saber.cs] ← 변경 없음
    └─ 파괴 시 Destroy → NotePool.Return() 변경만 필요
```

---

## 단계별 구현 계획

### Phase 1 — 코어 인프라 (현재 목표)
**목표.** 음악 재생에 BPM이 동기화된 노트 스폰, 오브젝트 풀링 적용

1. **NotePool.cs** 작성 — Instantiate/Destroy 대체
2. **AudioManager.cs** 작성 — AudioSource 래퍼, BPM 박자 이벤트
3. **Spawner.cs 수정** → AudioManager 이벤트 구독, NotePool 사용
4. **Cube.cs 수정** → 풀 반환 로직 추가 (이동은 그대로 유지)
5. **Saber.cs 수정** → `Destroy` → `NotePool.Return()` 1줄 교체

### Phase 2 — 점수 & 피드백
- ScoreManager.cs — 콤보, 점수 계산
- HitEffect — 파티클 이펙트 (VisualEffect Graph 또는 ParticleSystem)
- Miss 판정 — 노트가 플레이어를 지나친 경우

### Phase 3 — 게임 플로우
- SongData (ScriptableObject) — BPM, 곡 정보, 비트맵 데이터
- 씬 전환 (Intro → SongSelect → Gameplay → Results)
- 결과 화면 UI

---

## 기술 선택 기준

- **Object Pooling 방식.** `Queue<GameObject>` 기반 단순 풀 (MonoBehaviour 오버헤드 없는 직접 관리)
- **BPM 동기화.** `AudioSettings.dspTime` 기준 — `Time.time`보다 정밀하고 오디오 스레드와 동기화됨
- **이벤트 방식.** `System.Action` 델리게이트 — UnityEvent 대비 오버헤드 낮음
- **Cube.cs / Saber.cs 원칙.** 최소 침습 수정, 핵심 로직 보존
