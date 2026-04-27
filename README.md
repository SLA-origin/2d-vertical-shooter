# 🚀 2D Vertical Shooter

Unity로 제작한 2D 종스크롤 슈팅 게임입니다.

---

## 📖 프로젝트 소개

플레이어가 위에서 내려오는 적기들을 피하고 격추하며 생존하는 종스크롤 슈팅 게임입니다.  
파워 아이템으로 무기를 강화하고, 폭탄(Boom)으로 위기를 탈출하세요.

---

## 🛠️ 개발 환경

| 항목 | 내용 |
|------|------|
| 엔진 | Unity 6 (6000.4.1f1) |
| 언어 | C# |
| 렌더 파이프라인 | Universal Render Pipeline (URP) |
| 플랫폼 | PC (Windows) |

---

## 🎮 조작법

| 입력 | 동작 |
|------|------|
| `W A S D` / 방향키 | 플레이어 이동 |
| 마우스 **좌클릭** | 총알 발사 (클릭 즉시 발사 + 꾹 누르면 연속 발사) |
| 마우스 **우클릭** | 폭탄(Boom) 사용 — 화면 중앙에 폭발 연출 |

---

## ✨ 주요 기능

### 🔫 무기 시스템 (Power 레벨)
| 레벨 | 발사 패턴 |
|------|-----------|
| Power 1 | 중앙 단발 |
| Power 2 | 좌우 2발 동시 발사 |
| Power 3 | 중앙 강화탄 + 좌우 2발 |

### 💥 Boom (폭탄) 시스템
- 마우스 우클릭 시 화면 중앙에 2초간 폭발 애니메이션 재생
- 콘솔에 "폭발 발생!" 로그 출력

### 👾 적기 시스템
- **Enemy A / B / C** 3종 — 각각 고유 이동 패턴 보유
- 위쪽 스폰 포인트에서 하강 또는 사이드에서 진입
- 일정 간격으로 자동 생성 (GameManager 타이머 기반)
- 격추 시 점수 지급: A=100점 / B=200점 / C=300점
- Enemy C: 플레이어를 향해 자동 조준 발사

### 🎁 아이템 시스템
- **Coin** — 스코어 +1000
- **Power** — 스코어 +500 / 파워 레벨 +1 (MAX 3)
- **Boom** — 스코어 +500 / 폭탄 카운트 +1 (MAX 3)

### 🔄 오브젝트 풀링 (Object Pooling)
`ObjectManager` 싱글톤이 모든 오브젝트를 풀로 관리합니다.  
`Instantiate` / `Destroy` 대신 `SetActive` 재사용으로 GC 스파이크 없이 성능 최적화.

| 오브젝트 | 풀 크기 |
|----------|---------|
| EnemyL | 10 |
| EnemyM | 10 |
| EnemyS | 20 |
| ItemCoin | 20 |
| ItemPower | 10 |
| ItemBoom | 10 |
| PlayerBulletA | 100 |
| PlayerBulletB | 100 |
| BulletEnemyA | 100 |
| BulletEnemyB | 100 |

---

## 📁 프로젝트 구조

```text
Assets/
├── Scripts/
│   ├── Player.cs           # 플레이어 이동, 발사, 피격, 폭탄 처리
│   ├── PlayerBullet.cs     # 플레이어 총알 이동 및 충돌
│   ├── Enemy.cs            # 적기 이동, 발사, 피격, 사망 처리
│   ├── EnemyBullet.cs      # 적 총알 이동 및 충돌
│   ├── Item.cs             # 아이템 낙하 및 플레이어 충돌
│   ├── Boom.cs             # 폭발 오브젝트 (2초 후 풀 반환)
│   ├── GameManager.cs      # 점수, 목숨, 적 스폰, 게임오버 관리
│   ├── ObjectManager.cs    # 오브젝트 풀링 싱글톤 (핵심 최적화)
│   ├── UIManager.cs        # 점수/목숨 UI 업데이트
│   ├── EnemySpawner.cs     # 사이드 스폰 포인트 및 방향 계산
│   └── AreaDrawer.cs       # 화면 경계 감지 유틸리티
├── Prefab/                 # 총알, 적기, 아이템, 폭탄 프리팹
├── Scenes/
│   ├── GameScene.unity     # 메인 게임 씬
│   ├── ObjectPoolScene.unity
│   └── MecanimScene.unity
└── Animation/              # 플레이어 애니메이션 (Idle, Left, Right)
```

---

## ⚙️ 씬 세팅 체크리스트

### 필수 오브젝트
- **ObjectManager** — Inspector에서 10가지 프리팹 슬롯 연결
- **GameManager** — `spawnPoints`, `spawners` 할당
- **UIManager** — `lifeImages`, `scoreText`, `gameOverPanel` 연결
- **AreaDrawer** — 화면 경계 4개 포인트 연결
- **Player** — 태그 `Player`, `firePoint`, 총알 프리팹, `boomPrefab` 연결

### 태그
| 태그 | 오브젝트 |
|------|----------|
| `Player` | 플레이어 |
| `PlayerBullet` | 플레이어 총알 |
| `Enemy` | 적기 |
| `EnemyBullet` | 적 총알 |

---

## 🚀 실행 방법

1. Unity Hub에서 프로젝트 폴더 Open
2. `Assets/Scenes/GameScene.unity` 씬 오픈
3. **ObjectManager** 오브젝트 → Inspector에서 프리팹 10종 연결
4. Play 버튼으로 실행

---

## 📝 구현 체크리스트

- [x] 플레이어 이동 & 애니메이션 (Idle / Left / Right)
- [x] 마우스 클릭 발사 (즉시 발사 + 연속 발사)
- [x] Power 레벨별 발사 패턴 (1~3단계)
- [x] 적기 3종 스폰 시스템 (타이머 자동 생성)
- [x] Enemy C 플레이어 조준 발사
- [x] 아이템 애니메이션 & 이동 & 충돌 처리
- [x] Boom 폭탄 시스템 (마우스 우클릭, 화면 중앙 2초 폭발)
- [x] 점수 / 목숨 UI
- [x] 플레이어 사망 & 리스폰 (무적 시간 포함)
- [x] 오브젝트 풀링 (ObjectManager 싱글톤, 10종 580개 예약)
- [ ] 적기 처치 시 아이템 드랍 (확률형: None 30% / Coin 30% / Power 20% / Boom 20%)
- [ ] Coin/Power/Boom 아이템 획득 효과 적용
- [ ] Boom 사용 시 화면 내 모든 적 & 적 총알 제거

---

## 💡 개선 아이디어

- 스테이지/웨이브 데이터 분리 (ScriptableObject)
- 적 이동 패턴 다양화 (곡선 이동, 탄막)
- BGM / SFX 사운드 시스템
- 점수 저장 (PlayerPrefs 또는 온라인 리더보드)
- 모바일 터치 입력 대응

