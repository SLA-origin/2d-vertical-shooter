# 2D Vertical Shooter

Unity로 제작한 2D 종스크롤 슈팅 게임 프로젝트입니다.  
플레이어 이동/연사, 적 자동 스폰, 피격 및 라이프 UI, 게임오버/재시작 흐름이 구현되어 있습니다.

## 1. 프로젝트 개요

- 장르: 2D Vertical Shooting
- 엔진: Unity 6 (6000.4.1f1)
- 핵심 플레이:
	- 플레이어가 화면 내에서 이동하며 자동 연사
	- 일정 주기로 적이 생성되어 하강/측면 진입
	- 적 처치 시 점수 획득, 피격 시 라이프 감소
	- 라이프 소진 시 게임오버 패널 표시

## 2. 주요 기능

- 플레이어
	- 8방향 이동(대각선 정규화)
	- 발사 패턴 3단계(power 1~3)
	- 리스폰 및 무적 시간 처리
- 적
	- 타입 A/B/C
	- 타입 C는 플레이어를 조준해 총알 발사
	- 피격 시 스프라이트 피드백, 체력 0 이하 시 제거
- 스폰
	- `GameManager`의 타이머 기반 자동 스폰
	- 상단 하강 스폰 + 좌/우 측면 스폰 혼합
- UI
	- 점수 텍스트 업데이트
	- 라이프 이미지 알파 처리
	- 게임오버 패널 + Retry 버튼
- 경계 처리
	- `AreaDrawer` 기준으로 화면 밖 오브젝트 제거

## 3. 조작법

- 이동: `WASD` 또는 방향키
- 발사: 마우스 왼쪽 버튼 홀드
- 재시작: 게임오버 패널의 `Retry` 버튼

## 4. 핵심 스크립트

- `Assets/Player.cs`
	- 입력, 이동, 발사 패턴, 피격/사망/리스폰 관리
- `Assets/Enemy.cs`
	- 적 이동, 타입별 행동, 피격/점수 처리, 타입 C 발사
- `Assets/EnemyBullet.cs`
	- 적 총알 이동 및 화면 이탈 제거
- `Assets/PlayerBullet.cs`
	- 플레이어 총알 이동 및 화면 이탈 제거
	- 적 총알과 충돌 시 무시 처리
- `Assets/GameManager.cs`
	- 점수/라이프/게임오버 상태 관리
	- 자동 스폰 타이머 및 적 생성 루틴
- `Assets/UIManager.cs`
	- HUD 갱신, 게임오버 표시, 재시작
- `Assets/AreaDrawer.cs`
	- 플레이 영역 경계 계산 및 Gizmo 표시
- `Assets/EnemySpawner.cs`
	- 측면 스폰 방향 벡터 제공

## 5. 씬 세팅 체크리스트

### 필수 오브젝트

- `GameManager` 오브젝트
	- `enemies`, `spawnPoints`, `spawners` 할당
- `UIManager` 오브젝트
	- `lifeImages`, `scoreText`, `gameOverPanel` 연결
- `AreaDrawer` 오브젝트
	- `topLeft`, `topRight`, `bottomLeft`, `bottomRight` 연결
- 플레이어 오브젝트
	- 태그: `Player`
	- `Player` 스크립트의 `firePoint`, 총알 프리팹 연결

### 프리팹/태그 권장

- 플레이어 총알: 태그 `PlayerBullet`
- 적 총알: 태그 `EnemyBullet`
- 적 오브젝트: 태그 `Enemy`

## 6. 실행 방법

1. Unity Hub에서 프로젝트 폴더를 엽니다.
2. Unity Editor 버전은 `6000.4.1f1` 사용을 권장합니다.
3. 메인 씬을 열고 Play 버튼으로 실행합니다.

## 7. 게임 로직 흐름

1. 게임 시작 시 `GameManager`가 UI를 초기화합니다.
2. 일정 주기(`spawnInterval`)로 적을 생성합니다.
3. 플레이어는 이동하면서 연사하고, 적은 타입별로 이동/공격합니다.
4. 적 처치 시 점수가 오르고, 플레이어 피격 시 라이프가 줄어듭니다.
5. 라이프가 0이 되면 게임오버 UI를 표시하고 게임을 정지합니다.
6. Retry 버튼으로 같은 씬을 재로드합니다.

## 8. 알려진 주의사항

- `AreaDrawer` 참조가 비어 있으면 화면 이탈 제거 로직이 정상 동작하지 않을 수 있습니다.
- 적 조준 로직은 `Player` 태그를 우선 탐색합니다. 플레이어 태그가 누락되면 발사 방향 계산이 실패할 수 있습니다.
- 현재 `Player`의 피격 처리에서 `GameManager.DecreaseLife()`와 `HandlePlayerDeath()` 흐름이 함께 영향을 줄 수 있어, 체력/목숨 규칙을 확장할 때 중복 감소 여부를 점검하는 것이 좋습니다.

## 9. 폴더 구조(요약)

```text
Assets/
	Player.cs
	Enemy.cs
	EnemyBullet.cs
	PlayerBullet.cs
	GameManager.cs
	UIManager.cs
	AreaDrawer.cs
	EnemySpawner.cs
ProjectSettings/
Packages/
```

## 10. 개선 아이디어

- 스테이지/웨이브 데이터 분리(ScriptableObject)
- 적 패턴 다양화(곡선 이동, 탄막)
- 사운드/이펙트/피격 연출 강화
- 모바일 입력 대응
- 점수 저장(로컬/온라인 리더보드)
