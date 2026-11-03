# VRBeatSaber_v2 체크리스트

## Milestone 1 — 오브젝트 풀링 + BPM 동기화 스폰

### 1-A. NotePool.cs 작성
- [x] `Assets/Script/NotePool.cs` 생성
- [x] RED / BLUE 프리팹 각각 별도 Queue로 관리
- [x] `Get(NoteType, Vector3, Quaternion)` → 풀에서 꺼내 활성화
- [x] `Return(GameObject, NoteType)` → 비활성화 후 풀에 반환
- [x] 풀 고갈 시 새 인스턴스 생성 (안전망)
- [x] Inspector에서 초기 풀 크기 설정 가능

### 1-B. AudioManager.cs 작성
- [x] `Assets/Script/AudioManager.cs` 생성
- [x] `AudioSource` 래퍼 (Play, Stop)
- [x] Inspector에서 BPM 설정
- [x] `AudioSettings.dspTime` 기준 박자 계산
- [x] `OnBeat` 이벤트 (`System.Action`) 브로드캐스트
- [x] 싱글톤 방식 채택

### 1-C. Spawner.cs 수정 (최소 침습)
- [x] `AudioManager.OnBeat` 이벤트 구독
- [x] `Instantiate` → `NotePool.Get()` 교체
- [x] `Update()` 타이머 루프 제거 (이벤트 기반으로 전환)
- [x] `point[]` Inspector 연결 유지 (`cube[]`는 NotePool이 담당하므로 제거)

### 1-D. Cube.cs 수정 (최소 침습)
- [x] 이동 로직(`transform.forward * 2`) 보존
- [x] 플레이어 통과 판정 추가 (Z축 임계값 `despawnZ`)
- [x] 통과 시 `NotePool.Return()` 호출

### 1-E. Saber.cs 수정 (1줄 교체)
- [x] `Destroy(hit.transform.gameObject)` → `NotePool.Instance.Return(...)` 교체
- [x] 타격 판정 로직(Raycast, 각도 조건) 변경 없음

### 1-F. Unity 씬 연결 및 테스트
- [ ] Game.unity에 AudioManager, NotePool 오브젝트 배치
- [ ] Inspector 참조 연결 확인
- [ ] 음악 없이 BPM=120 기준 박자 스폰 동작 확인
- [ ] 노트 타격 시 풀 반환 동작 확인
- [ ] 노트 통과 시 풀 반환 동작 확인

---

## Milestone 2 — 점수 & 히트 피드백

### 2-A. ScoreManager.cs
- [x] 싱글톤, Score / Combo 상태 관리
- [x] `NoteHit()` — 콤보 증가, 배율 적용 점수 누적
- [x] `NoteMiss()` — 콤보 초기화
- [x] `OnScoreChanged`, `OnComboChanged` 이벤트 (UI 연결용)
- [x] 콤보 배율: 1~9 → x1 / 10~19 → x2 / 20+ → x4

### 2-B. HitEffectPool.cs
- [x] RED / BLUE 파티클 이펙트 풀 (Queue 기반)
- [x] `Play(position, type)` — 풀에서 꺼내 위치 지정 후 재생
- [x] ParticleSystem 재생 완료 후 자동 반환 (코루틴)

### 2-C. Saber.cs 수정
- [x] 타격 판정 성공 시 `HitEffectPool.Instance.Play()` 호출
- [x] `ScoreManager.Instance.NoteHit()` 호출
- [x] `note.isHit = true` 세팅 후 풀 반환

### 2-D. Cube.cs 수정
- [x] `isHit` 플래그 추가 (풀 반환 시 OnEnable에서 초기화)
- [x] despawnZ 통과 시 `isHit`가 false일 때만 `NoteMiss()` 호출

### 2-E. Unity 씬 연결 및 테스트
- [ ] Hierarchy에서 Create Empty → 이름 `ScoreManager` → Inspector에서 Add Component → `ScoreManager` 스크립트 추가
- [ ] Hierarchy에서 Create Empty → 이름 `HitEffectPool` → Inspector에서 Add Component → `HitEffectPool` 스크립트 추가
- [ ] RED/BLUE 파티클 프리팹 제작 및 HitEffectPool Inspector의 Red/Blue Effect Prefab 슬롯에 연결
- [ ] ScoreUI.cs Unity 씬 연결 (아래 참고)
- [ ] 타격 시 이펙트 재생 확인
- [ ] 콤보/점수 UI 표시 확인
- [ ] Miss 시 콤보 초기화 확인

## Milestone 3 — 게임 플로우 (예정)
- [ ] SongData ScriptableObject
- [ ] 씬 전환 (Intro → SongSelect → Gameplay → Results)
- [ ] 결과 화면 UI
