## Bullet Shooting Game (총알 회피 게임)

Unity로 만든 **3D 서바이벌(총알 회피) 미니게임**입니다. 회전 스포너가 발사하는 총알을 피하며 **생존 시간/점수**를 올립니다.

## 데모

- **WebGL 빌드 포함**: `Build/` 폴더(로컬 서버로 실행 권장)
- **데모 링크**: 없음(레포에 빌드 산출물 포함)

## 문제 정의(Why) / 목표(Goal)

- **Why**: Unity에서 *게임 루프 / 충돌 / UI / 씬 전환 / 코루틴*을 한 프로젝트로 끝까지 묶어보는 학습용 실습이 필요했습니다.
- **Goal**: 난이도(페이즈) 증가, 아이템, 기록 표시까지 포함한 “완결된 플레이 흐름”을 구현합니다.

## 주요 기능(Features)

1. **이동/조작**: WASD 또는 방향키로 이동
2. **총알 회피**: 플레이어를 향해 날아오는 총알 회피(충돌 시 피해)
3. **페이즈(난이도) 증가**: 일정 시간마다 총알 속도 증가
4. **난이도 선택**: Easy/Medium/Hard (시작 체력 5/3/2)
5. **아이템 3종**: 체력(+1), 무적(3초), 점수(+1000)
6. **UI**: 점수/시간/체력/무적 카운트다운/페이즈 안내/게임오버
7. **기록 표시(세션 기준)**: 난이도별 최고 점수/최장 생존시간을 **플레이 세션 내에서** 유지 및 표시

## 기술 스택

- **Frontend(클라이언트)**: Unity 2022.3.42f1, C#
- **Backend**: 없음
- **DB**: 없음 (런타임 메모리 + 일부 UI 애니메이션 상태는 `PlayerPrefs` 사용)
- **Infra/Deploy**: WebGL 빌드(`Build/`), 로컬 정적 서버(Python/Node/Docker)로 실행
- **외부 API/연동**: 없음
- **인증/보안**: 해당 없음

## 시스템 구성도(Architecture)

```mermaid
flowchart LR
  Input[Input System(WASD/Arrow)] --> PC[PlayerController]
  PC --> RB[Rigidbody 이동]

  DM[DifficultyManager<br/>(Singleton, DontDestroyOnLoad)] --> GM[GameManager<br/>(점수/체력/페이즈/아이템 스폰)]
  DM --> B[Bullet<br/>(속도/수명)]

  BS[BulletSpawner<br/>(회전 + 발사)] --> B
  B -->|OnTriggerEnter| GM

  IM[ItemManager<br/>(Health/Invincibility/Score)] -->|OnTriggerEnter| GM
  GM --> UI[Unity UI + TMP<br/>(점수/시간/체력/알림)]

  BTN[StartButtonHandler] --> SM[SceneManager<br/>(Main/Rule/Option/Start/InGame)]
```

## 빠른 시작(Quick Start)

### 요구사항

- **Unity Hub + Unity 2022.3.42f1** (에디터 실행 시)
- **(선택) Python 3** 또는 **Node.js** 또는 **Docker** (WebGL 로컬 실행 시 정적 서버용)

### 로컬 실행 (Unity 에디터)

1. Unity Hub에서 **Unity 2022.3.42f1** 설치
2. 이 레포를 Unity Hub에서 **Open**으로 열기
3. 씬 열기: `Assets/Scenes/MainScene.unity` (또는 바로 난이도 선택으로 가려면 `StartScene.unity`)
4. **Play** 클릭

### 로컬 실행 (WebGL 빌드)

> Unity WebGL은 브라우저 보안 정책 때문에 `index.html` 더블클릭만으로는 동작이 깨질 수 있어, **로컬 서버로 실행**을 권장합니다.

#### 옵션 A) Python (권장: 설치가 쉬움)

```bash
python -m http.server 8080 --directory Build
```

브라우저에서 `http://localhost:8080` 접속

#### 옵션 B) Node.js (http-server)

```bash
npx --yes http-server Build -p 8080 -c-1
```

#### 옵션 C) Docker (nginx로 정적 서빙)

PowerShell:

```powershell
docker run --rm -p 8080:80 -v "${PWD}\Build:/usr/share/nginx/html:ro" nginx:alpine
```

macOS/Linux:

```bash
docker run --rm -p 8080:80 -v "$(pwd)/Build:/usr/share/nginx/html:ro" nginx:alpine
```

### 테스트(있을 때)

- 현재 레포에 자동화 테스트는 포함되어 있지 않습니다.

### 배포(있다면)

- 별도 배포는 없습니다. 대신 `Build/`에 **WebGL 빌드 산출물**을 포함합니다.

## 환경변수(.env.example)

- 이 프로젝트는 **환경변수를 사용하지 않습니다.**

| 키 | 설명 | 예시(가짜 값) |
|---|---|---|
| (없음) | - | - |

## 폴더 구조(간단)

```
/
├─ Assets/
│  ├─ Scenes/              # Main/Rule/Option/Start/InGame
│  └─ Scripts/             # 핵심 게임 로직(C#)
├─ Build/                  # WebGL 빌드 산출물(정적 서버로 실행)
├─ Packages/               # Unity 패키지 매니페스트
└─ ProjectSettings/        # Unity 프로젝트 설정
```

## 내 기여

- **(개인 프로젝트)**: 기획/구현/디버깅/빌드(WebGL) 전 과정 수행
- **코어 구현**: `GameManager`, `DifficultyManager`, `BulletSpawner`, `Bullet`, `ItemManager`, `PlayerController`
- **게임 흐름**: 씬 전환(Main/Rule/Option/Start/InGame) 및 UI 상태(점수/시간/체력/알림) 연결

## 트러블슈팅/의사결정(요약)

1. **WebGL 실행 재현성**: 파일 더블클릭 대신 **정적 서버 실행**으로 안내(브라우저 정책 이슈 회피)
2. **난이도/페이즈 데이터 공유**: 씬 전환에도 유지되도록 `DifficultyManager`를 **Singleton + `DontDestroyOnLoad`**로 설계
3. **시간 기반 로직**: 아이템 스폰/무적 카운트다운을 코루틴으로 구현해, Update 루프 과밀을 피함

## 라이선스/기타

- 별도 라이선스 파일이 없어 **기본 저작권 상태**입니다. 필요 시 `LICENSE`를 추가하세요.

## (부록) 면접 질문 5개 + 답변 초안

1. **Q. 왜 `DifficultyManager`를 Singleton(`DontDestroyOnLoad`)으로 했나요?**
   - A. 난이도/총알 속도 같은 전역 상태를 씬 전환 간 유지하려고 했고, 해당 범위에서는 전역 단일 인스턴스가 가장 단순했습니다. 단, 커지면 DI/Service 패턴으로 교체 여지가 있습니다.

2. **Q. 페이즈 난이도 증가를 어떤 방식으로 구현했나요?**
   - A. `GameManager`가 일정 시간마다 페이즈를 올리고, `DifficultyManager.IncreaseBulletSpeed()`로 총알 속도만 단계적으로 증가시키는 “단일 파라미터 스케일링”으로 시작했습니다.

3. **Q. 아이템 스폰을 Update가 아니라 코루틴으로 둔 이유는요?**
   - A. 간격 기반 작업(스폰/카운트다운)은 코루틴이 의도가 명확하고, Update에서 타이머를 여러 개 관리하는 복잡도를 줄일 수 있습니다.

4. **Q. 총알이 매번 Instantiate/Destroy되는데 성능 이슈는 없나요?**
   - A. 현재 스케일에서는 단순성을 우선했고, 스폰 빈도가 커지면 **오브젝트 풀링**으로 GC/프레임 드랍을 줄이도록 개선할 수 있습니다.

5. **Q. 기록(최고 점수/최장 시간)을 어떻게 저장하나요?**
   - A. 현재는 난이도별 배열로 **플레이 세션 내 유지**합니다. 영구 저장이 필요하면 `PlayerPrefs`(간단) 또는 파일/JSON 저장으로 확장할 수 있습니다.

