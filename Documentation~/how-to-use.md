# 사용법

NexUI Designer 창의 모든 패널과 기능을 처음부터 끝까지 다룹니다. 빠르게 훑고 싶다면 "권장 작업 흐름" 절로 바로 가도 됩니다.

## 목차

1. [디자이너 열기](#디자이너-열기)
2. [창 레이아웃 개요](#창-레이아웃-개요)
3. [화면 선택하기](#화면-선택하기)
4. [메타데이터 만들기](#메타데이터-만들기)
5. [컴포넌트 팔레트](#컴포넌트-팔레트)
6. [계층(Hierarchy) 패널](#계층hierarchy-패널)
7. [뷰포트 다루기](#뷰포트-다루기)
8. [다중 선택 & 정렬 (Align 패널)](#다중-선택--정렬-align-패널)
9. [인스펙터 — 섹션별 상세](#인스펙터--섹션별-상세)
10. [Simple / Advanced 모드](#simple--advanced-모드)
11. [도형(Shape) 설정](#도형shape-설정)
12. [값 기반 컴포넌트 미리보기 (Progress Bar / Radial Fill / Spinner)](#값-기반-컴포넌트-미리보기-progress-bar--radial-fill--spinner)
13. [이미지 넣기 (Image / Icon Button)](#이미지-넣기-image--icon-button)
14. [History 패널](#history-패널)
15. [언어 바꾸기](#언어-바꾸기)
16. [키보드 단축키 전체 목록](#키보드-단축키-전체-목록)
17. [창/패널 크기 조절](#창패널-크기-조절)
18. [화면 검증하기](#화면-검증하기)
19. [저장하기](#저장하기)
20. [Figma 브리지 (진행 중)](#figma-브리지-진행-중)
21. [마이그레이션 위저드](#마이그레이션-위저드)
22. [권장 작업 흐름](#권장-작업-흐름)
23. [알려진 제한 (TODO)](#알려진-제한-todo)

---

## 디자이너 열기

```text
Tools/NexUI/Designer
```

메뉴 하나로 창이 열립니다. 창을 닫았다 다시 열어도 마지막에 조절한 패널 크기(아래 "창/패널 크기 조절" 참고)는 유지됩니다.

## 창 레이아웃 개요

창은 위에서 아래로, 왼쪽에서 오른쪽으로 다음과 같이 구성됩니다.

```text
┌───────────────────────────────────────────────────────────────┐
│ 툴바 (Screen/Metadata 필드, Mode 토글, Preview/State/Input,   │
│      Snap, Zoom, Rebuild/Save/Validate)                        │
├───────────┬─────────────────────────────────┬──────┬──────────┤
│ Palette   │                                 │      │Inspector │
│ (컴포넌트  │                                 │Align │Validation│
│  카테고리) │           뷰포트(캔버스)          │패널  │History   │
│           │                                 │(정렬/│          │
│ Hierarchy │                                 │분배/ │          │
│ (계층)     │                                 │레이어)│          │
├───────────┴─────────────────────────────────┴──────┴──────────┤
│ State 프리뷰 │ Command 프리뷰 │ Screen Graph                   │
└───────────────────────────────────────────────────────────────┘
```

- **모든 경계선은 드래그로 크기 조절이 가능**합니다(툴바 하단 경계 포함). 자세한 내용은 "창/패널 크기 조절" 절을 참고하세요.
- 좌측은 **컴포넌트 팔레트 + 계층**이 한 컬럼, 그 오른쪽에 **Align 패널**(정렬/분배/레이어 버튼)이 세로로 붙어 있습니다.
- 우측은 **인스펙터 + 검증 패널 + History 패널**이 세로로 쌓여 있습니다.
- 하단은 **State 프리뷰 / Command 프리뷰 / Screen Graph** 3개 카드가 가로로 나열됩니다.

## 화면 선택하기

1. `UIScreenDefinition`을 만들거나 선택합니다.
2. 툴바의 `Screen` 필드에 할당합니다.
3. `Rebuild`를 누릅니다.

뷰포트에는 현재 프리뷰 셸이 표시됩니다. 계층과 인스펙터는 선택한 화면과 디자이너 메타데이터를 기준으로 갱신됩니다.

## 메타데이터 만들기

디자이너 전용 데이터는 `DesignerMetadataAsset`에 저장합니다. 아래 메뉴에서 만들거나, 툴바의 `Metadata` 필드 옆 `New` 버튼을 눌러 현재 열린 화면용으로 바로 생성할 수 있습니다.

```text
Create > NexUI > Designer > Metadata
```

메타데이터에는 런타임 UI 로직이 아니라 제작을 돕기 위한 정보를 넣습니다.

- element id / 표시 이름 / 타입 / 도형(Shape) / 프리뷰 값
- style class, 틴트, 텍스트 색상, 폰트 크기
- binding key (text/value/visibility/class/command/interactable)
- Auto Layout / Constraints 설정
- Motion 프리셋과 상태별 variant
- Theme 토큰 오버라이드
- Accessibility 라벨/역할
- validation hint, localization link, preview row, screen variant

## 컴포넌트 팔레트

좌측 상단 **Components** 패널입니다. 20개 컴포넌트 타입이 5개의 폴더(Foldout)로 정리되어 있습니다.

| 카테고리 | 포함 컴포넌트 |
| --- | --- |
| **Containers** | Panel, Card, Container, Modal |
| **Text & Media** | Text(Label), Image |
| **Actions & Input** | Button, Icon Button, Choice List |
| **Feedback & Status** | Toast, Tooltip, Popover, Progress Bar, Stat Bar, Radial Fill, Spinner, Skeleton |
| **Data & Lists** | List, Grid, Slot |

- 카테고리 제목을 클릭하면 접고 펼칠 수 있고, 이 열림/닫힘 상태는 `EditorPrefs`에 저장되어 창을 껐다 켜도 유지됩니다.
- 상단 검색창에 이름을 입력하면 일치하는 컴포넌트만 남고, 매치가 있는 카테고리는 자동으로 펼쳐집니다.
- 컴포넌트 버튼을 클릭하면 현재 화면에 그 타입의 요소가 추가되고 자동으로 선택됩니다. 컴포넌트 타입에 따라 도형(Shape)이 자동으로 다르게 설정됩니다 — 예: Spinner/Radial Fill은 원형, Toast/Tooltip/Icon Button은 알약형, 나머지는 둥근 사각형(Rounded).
- 팔레트 하단의 **Selection actions**(Left/Center/Right/Top/Middle/Bottom/Fill/Copy/Delete)는 **선택한 요소 하나를 캔버스 자체 기준**으로 정렬/복제/삭제합니다 (여러 요소를 서로 기준으로 정렬하는 것은 아래 "Align 패널"이 담당).

## 계층(Hierarchy) 패널

좌측 하단, Palette 아래에 있습니다.

- 검색창으로 id/표시 이름을 필터링합니다.
- 각 행에 **◉ (표시/숨김 토글)**과 **✓ (잠금 토글)** 아이콘이 있습니다.
  - ◉ 클릭: 이 요소를 디자이너 캔버스에서만 숨깁니다(런타임 표시에는 영향 없음) — 메타데이터를 삭제하지 않고 캔버스를 정리할 때 유용합니다.
  - ✓ 클릭: 캔버스에서 이동/크기조절/사각형 편집을 막습니다(인스펙터 필드는 계속 동작).
- 목록에서 다중 선택(Shift/Ctrl 클릭)이 가능하며, 캔버스 선택과 항상 양방향으로 동기화됩니다.

## 뷰포트 다루기

- **줌**: 툴바의 `-`/`+` 버튼, 또는 마우스 휠.
- **그리드/스냅**: 툴바의 `Snap` 토글과 그리드 크기. 기본 그리드는 8px(8pt 스케일과 일치).
- **선택**: 빈 캔버스에서 드래그하면 사각형 영역과 겹치는 요소가 모두 선택됩니다.
  - `Shift` + 클릭/드래그: 기존 선택에 추가
  - `Ctrl` + 클릭/드래그: 선택 토글
- **그룹 이동**: 다중 선택된 요소 중 하나를 드래그하면 선택된 요소 전체가 함께 이동합니다.
  - 드래그 중 `Shift`: 이동량이 더 큰 축으로 고정(축 고정 이동)
  - 리사이즈 핸들(우하단 모서리)을 드래그하면 그 요소 하나만 리사이즈됩니다.
- **우클릭 메뉴**: 빈 캔버스와 요소 각각에 대해 다른 메뉴가 뜹니다. 클릭 지점에 요소가 여러 개 겹쳐 있으면 메뉴 상단에 `Select Element/<이름>` 목록이 추가되어, 가장 위 요소만이 아니라 겹친 요소 중 원하는 것을 정확히 고를 수 있습니다. 요소 메뉴에는 Select/Add to Selection/Select Children/Select Parent, Duplicate/Delete/Rename, Bring Forward/Send Backward/Bring To Front/Send To Back, Align, Distribute, Group/Ungroup, Create Motion Clip From Selection이 포함됩니다.
- **F 키**: 선택한 요소로 뷰를 스크롤해서 이동시킵니다.
- 요소를 더블클릭하면 이름을 바로 편집할 수 있는 인라인 필드가 뜹니다(Enter로 확정, Esc로 취소).

## 다중 선택 & 정렬 (Align 패널)

정렬/분배/레이어 순서 조작 버튼들은 **좌측, 컴포넌트 팔레트 바로 옆의 세로 패널**에 있습니다(과거 툴바 하단 3번째 줄에 있던 것이 이쪽으로 옮겨졌습니다).

### 정렬 (Align)

2개 이상 선택 시 **선택된 요소들의 bounding box**를 기준으로 정렬합니다. 1개만 선택했을 때는 팔레트 하단 "Selection actions"처럼 캔버스 해상도를 기준으로 정렬합니다.

| 버튼 | 동작 | 단축키 |
| --- | --- | --- |
| ⇤ | 왼쪽 정렬 | `Alt+L` |
| ⇔x | 가로 중앙 정렬 | `Alt+C` |
| ⇥ | 오른쪽 정렬 | `Alt+R` |
| ⇡ | 위쪽 정렬 | `Alt+T` |
| ⇕y | 세로 중앙 정렬 | `Alt+M` |
| ⇣ | 아래쪽 정렬 | `Alt+B` |

### 분배 (Distribute)

3개 이상 선택했을 때만 활성화됩니다.

| 버튼 | 동작 | 단축키 |
| --- | --- | --- |
| ⋯H | 가로축으로 균등 배치 | `Alt+H` |
| ⋮V | 세로축으로 균등 배치 | `Alt+V` |

### 레이어 순서 (Layer)

`Metadata.elements` 리스트 순서가 곧 z-order이며, uGUI/UI Toolkit 저장 시 sibling 순서에도 그대로 반영됩니다.

| 버튼 | 동작 | 단축키 |
| --- | --- | --- |
| Fwd | 한 단계 앞으로 | `Ctrl+]` |
| Back | 한 단계 뒤로 | `Ctrl+[` |
| Front | 맨 앞으로 | `Ctrl+Shift+]` |
| Bottom | 맨 뒤로 | `Ctrl+Shift+[` |

각 버튼에 마우스를 올리면 툴팁에 단축키가 함께 표시되니, 굳이 이 문서를 다시 찾아보지 않아도 됩니다.

### 그룹 (Group/Ungroup)

2개 이상 선택 후 `Ctrl+G`(또는 우클릭 메뉴)로 Group을 실행하면 선택 영역의 bounding box 크기를 가진 새 Panel 요소가 만들어지고, 선택된 요소들의 `parentId`가 그 그룹으로 재지정됩니다(좌표는 절대 캔버스 좌표를 그대로 유지). `Ctrl+Shift+G`(Ungroup)는 그룹의 자식들을 그룹의 부모로 되돌리고 그룹 요소를 삭제합니다.

## 인스펙터 — 섹션별 상세

우측 패널 상단, 선택한 요소(또는 화면)에 따라 아래 섹션들이 나타납니다. **Simple 모드에서는 일부 섹션이 숨겨집니다** (다음 절 참고).

### Selection (다중 선택 시)

2개 이상 선택하면 다른 섹션들 위에 나타납니다. 선택 개수, 공통 위치/크기(다르면 "Mixed"), 차지하는 레이어 순서 범위를 보여줍니다.

### Layout

| 필드 | 설명 |
| --- | --- |
| Position | 캔버스 픽셀 기준 좌상단 위치(절대 좌표) |
| Size | 너비/높이(각 축 최소 24px) |
| Anchor | uGUI 스타일 앵커 프리셋(저장 시 백엔드 RectTransform/style에 적용, Stretch는 해당 축을 부모에 꽉 채움) |
| Locked | 캔버스에서 이동/크기조절/사각형 편집 방지(인스펙터 필드는 계속 동작) |

### Auto Layout

이 요소를 Figma 스타일 Auto Layout 컨테이너로 만들어 직계 자식들을 한 축으로 배치합니다.

| 필드 | 설명 |
| --- | --- |
| Auto Layout (토글) | 켜면 이 요소가 직계 자식을 배치하는 컨테이너가 됩니다 |
| Direction | Row(왼→오) 또는 Column(위→아래) |
| Spacing | 자식 사이 간격(px) |
| Padding (Left/Top/Right/Bottom) | 이 요소 가장자리에서 자식까지의 여백 |
| Width / Height (Self sizing) | **이 요소 자신**이 부모가 Auto Layout일 때 어떻게 크기를 정하는지: Fixed(자기 rect 크기 사용) / Hug(내용에 맞춤) / Fill(남는 공간만큼 늘어남) |

> 현재는 메타데이터 편집까지만 지원합니다. 뷰포트 실시간 미리보기에 flexbox를 그대로 반영하는 것과 UXML 자동 반영은 별도 후속 작업입니다.

### Style

| 필드 | 설명 |
| --- | --- |
| Element Id | 바인딩 키/commandKey/백엔드 이름 매칭에 쓰이는 고유 id. 이름을 바꾸면 자식들의 parentId가 자동으로 다시 연결됨 |
| Name | Hierarchy 패널에 표시되는 이름(표시용) |
| Type | 요소의 의미론적 타입(Panel/Button/... 등 20종) |
| Text | 정적 미리보기 텍스트 |
| Classes | 공백으로 구분된 스타일 클래스 |
| **Shape** | 캔버스 프리뷰 박스 모양: **Rectangle**(각진 모서리) / **Rounded**(작은 반경, 기본값) / **Pill**(완전히 둥근 알약형) / **Circle**(원/타원). 아래 "도형 설정" 절 참고 |
| Tint | 배경/그래픽 색 |
| Text Color | 텍스트 색 |
| Font Size | 8-96 |
| Hidden | 캔버스에서만 숨김(런타임 표시 무관) |
| **Preview Value** | Progress Bar/Stat Bar/Radial Fill 선택 시에만 표시. 채움 정도(Min/Max 범위 내) |
| **Min Value / Max Value** | 위와 같은 타입에서만 표시. Unity Slider의 min/maxValue와 동일한 개념 |
| **Fill Direction** | Progress Bar/Stat Bar에서만 표시. LeftToRight/RightToLeft/BottomToTop/TopToBottom |
| **Clockwise** | Radial Fill/Spinner에서만 표시. 채움/스핀 방향 |
| **Image** | Image/Icon Button에서만 표시. 실제 텍스처를 지정하면 캔버스에 그대로 렌더링됨 |

굵게 표시된 필드는 이 세션에서 새로 추가된 것들이며, 아래 두 절에서 더 자세히 다룹니다.

### Constraints (Advanced 전용)

부모가 리사이즈될 때(예: C1 반응형 브레이크포인트 전환) 이 요소가 어떻게 반응할지 정합니다. Figma의 Constraints와 동일한 모델입니다.

| 필드 | 설명 |
| --- | --- |
| Horizontal | Start(왼쪽 고정) / End(오른쪽 고정) / Center / Scale(비율에 맞춰 크기 조절) |
| Vertical | Start(위 고정) / End(아래 고정) / Center / Scale |

> 현재는 메타데이터 편집까지만 지원하며, 반응형 프리뷰가 실제로 이 값을 반영해 재배치하는 것은 후속 작업입니다.

### Binding

| 필드 | 설명 |
| --- | --- |
| Text Key | UIStateStore 문자열 키가 텍스트를 구동(비우면 정적 텍스트) |
| Value Key | UIStateStore float 키가 Value 기능(프로그레스 바 등)을 구동 |
| Visibility Key | UIStateStore bool 키가 표시/숨김을 구동 |
| Class Key | UIStateStore 문자열 키가 스타일 클래스를 구동 |
| Command Key | 클릭 시 실행되는 액션 키. **Pick...** 버튼으로 프로젝트에 이미 쓰인 커맨드 중에서 고를 수 있음(직접 입력 불필요) |
| Interactable Key | UIStateStore bool 키가 상호작용 가능 여부를 구동 |

Simple 모드에서는 Command Key만 보이고 나머지 Key 필드들은 Advanced 모드에서만 노출됩니다. Text/Value/Interactable Key 옆의 **Pick...** 버튼은 리플렉션으로 찾은 `IBindableProperty<T>` 데이터 소스 목록을 보여줍니다.

### Theme

| 필드 | 설명 |
| --- | --- |
| Theme | 직접 테마 에셋 참조(할당 시 Theme Id도 자동 채움) |
| Theme Id | 런타임 테마 해석용 id 문자열 |
| Classes | 테마 전용 스타일 클래스 |
| Token Overrides | 이 요소에서만 특정 테마 토큰을 오버라이드하는 key/value 목록. `Add Token Override`로 빈 항목 추가 |
| **Eyedropper → Add as Token** | 색상 스와치를 클릭하면 Unity 색상 피커가 열리고(내장 스포이드로 화면 어디든 픽셀 채취 가능), `Add as Token` 버튼으로 그 색을 새 토큰 오버라이드로 추가 |

### Motion

Motion 프리셋과 Initial/Animate/Exit/Hover/Pressed/Focus 6개 상태별 variant 이름을 지정합니다. `Open Motion Graph`로 노드 그래프 에디터를, `Open Motion Clip Editor`로 이 요소로 스코프된 타임라인 에디터를 엽니다.

### Accessibility

Screen-reader용 라벨과 의미론적 역할(Button/Toggle/Slider 등, ARIA/UIKit 어휘 준수)을 지정합니다.

### Policy (화면 레벨, Advanced 전용)

요소가 아니라 **열린 화면 전체**에 적용됩니다: Input Blocking, Pause Game Behind, Close On Back, Cursor/Time/Focus/Conflict/Lifetime Policy.

### Capability (읽기 전용)

선택한 요소의 Type이 노출할 것으로 기대되는 런타임 capability 인터페이스 목록을 보여줍니다(정보 제공용).

### Validation (요소별)

이 요소의 데이터가 어떤 4개 카테고리(화면/바인딩/모션/테마)로 검사되는지 안내합니다. 실제 문제 목록은 우측 하단의 Validation 패널에서 확인합니다.

## Simple / Advanced 모드

툴바의 `Mode: Simple` / `Mode: Advanced` 버튼으로 전환합니다.

- **Simple**: 드래그앤드롭 컴포넌트 팔레트, Auto Layout 기본, Binding의 Command Key만. Theme 토큰/Motion/Policy/Capability/Constraints, Binding의 나머지 Key 필드는 숨겨집니다.
- **Advanced**: 전부 노출.
- 선택은 사용자별로 `EditorPrefs`에 저장됩니다.
- Simple 모드에서는 버튼 옆에 작은 회색 힌트("Theme/Motion/Policy hidden")가 붙어서, 필요하면 Advanced 모드가 있다는 걸 자연스럽게 알아차릴 수 있습니다.

## 도형(Shape) 설정

Style 인스펙터의 `Shape` 드롭다운으로 모든 요소의 캔버스 프리뷰 모양을 바꿀 수 있습니다.

| 값 | 결과 |
| --- | --- |
| Rectangle | 각진 사각형(반경 0) |
| Rounded | 작은 반경(8px) — 이전까지의 기본 모양 |
| Pill | 양 끝이 완전히 둥근 알약 모양(높이에 따라 자동으로 클램프됨) |
| Circle | 정사각형 rect면 정원, 아니면 타원 |

새로 팔레트에서 추가하는 컴포넌트는 타입에 맞는 기본 도형이 자동으로 설정됩니다(Spinner/Radial Fill → Circle, Toast/Tooltip/Icon Button → Pill, 나머지 → Rounded). 언제든 Style 인스펙터에서 직접 바꿀 수 있습니다.

이 값은 **순수 디자이너 프리뷰용**입니다 — 실제 백엔드(UXML/프리팹)의 스타일에는 영향을 주지 않으므로, 실제 라운드 처리는 여전히 USS/우GUI 스프라이트 쪽에서 별도로 해야 합니다.

## 값 기반 컴포넌트 미리보기 (Progress Bar / Radial Fill / Spinner)

Progress Bar, Stat Bar, Radial Fill, Spinner는 이제 실제 내부 구조를 그립니다 — 색칠된 박스가 아니라 실제로 채워진 바/링입니다.

- **Progress Bar / Stat Bar**: 배경 트랙 + 채움 바. `Min Value`/`Max Value`/`Preview Value`로 채움 비율을 계산하고(Unity Slider와 동일한 개념), `Fill Direction`으로 어느 방향에서 자라나는지 결정합니다.
  - `LeftToRight`(기본): 왼쪽부터 채워짐, 트랙은 박스 하단의 가로 바
  - `RightToLeft`: 오른쪽부터 채워짐
  - `BottomToTop` / `TopToBottom`: 트랙이 박스 우측의 세로 바로 바뀌고, 아래→위 또는 위→아래로 채워짐
- **Radial Fill**: 배경 링 + `Preview Value`만큼 위쪽(12시 방향)부터 채워지는 아크. `Clockwise` 토글로 시계/반시계 방향을 바꿉니다. 실제로 Unity의 `Painter2D` API로 직접 그린 호(arc)이며, 텍스처나 스프라이트가 필요 없습니다.
- **Spinner**: 같은 방식의 링이 계속 회전하며 실제로 도는 로딩 스피너처럼 보입니다.
- **Choice List**: 체크박스 3줄(첫 번째 항목은 체크된 상태) 미리보기.
- **List**: 세로로 쌓인 placeholder 행 3개.
- **Grid**: 2열 그리드 형태의 placeholder 셀 6개.
- **Skeleton**: 깜빡이는(opacity 애니메이션) shimmer 바 3개.

모든 채움/셀 크기는 **퍼센트 기반**이라, 요소를 드래그로 늘리거나 줄여도 자동으로 비율이 맞춰집니다. 별도로 다시 계산할 필요가 없습니다.

## 이미지 넣기 (Image / Icon Button)

Style 인스펙터에서 Type을 `Image` 또는 `Icon Button`으로 설정하면 **Image** 필드가 나타납니다.

1. 원하는 `Texture2D` 에셋을 Image 필드에 드래그하거나 오브젝트 선택창에서 고릅니다.
2. **Image 타입**: 텍스처가 박스 전체를 채우도록(ScaleToFit) 렌더링됩니다.
3. **Icon Button 타입**: 텍스처가 버튼 중앙에 24x24 크기의 작은 아이콘으로 렌더링되고, 버튼 자체의 틴트는 배경으로 그대로 유지됩니다.

이 값도 **디자이너 프리뷰 전용**입니다 — 실제 런타임에서 어떤 스프라이트/텍스처를 쓸지는 백엔드(UXML/프리팹) 쪽 에셋이 그대로 담당합니다.

## History 패널

우측 패널 맨 아래에 있습니다. 이 세션에서 있었던 편집 액션 이름(예: "Edit NexUI Element Tint", "Move NexUI Elements")을 최신순으로 최대 50개까지 나열합니다.

- **점프 기능은 없습니다.** Unity의 공개 Undo API는 과거 undo 그룹을 열거하거나 임의 지점으로 이동하는 기능을 제공하지 않기 때문에, 이 패널은 "방금 무엇이 바뀌었는지" 확인하는 읽기 전용 로그입니다.
- 실제로 되돌리려면 여전히 `Ctrl+Z`/`Ctrl+Y`(Unity 기본 Undo/Redo)를 사용하세요.
- 창을 닫았다 열거나 에디터를 재시작하면 로그는 초기화됩니다(세션 한정).

## 언어 바꾸기

```text
Tools/NexUI/Designer/Language/Korean
Tools/NexUI/Designer/Language/English
```

- 에디터 UI(패널 제목, 섹션 제목, **모든 필드/버튼의 툴팁**)가 디자이너 로컬라이제이션 테이블(`Localization/en-US.json`, `ko-KR.json`)을 사용합니다.
- 언어를 바꾸면 **창을 닫았다 열 필요 없이** 즉시 전체 UI가 다시 그려집니다.
- 누락된 문자열은 창을 깨뜨리지 않고 키 또는 영어 기본 문자열로 표시됩니다.
- 컴포넌트 타입 이름(Panel, Button 등)이나 State/Input 드롭다운 선택지(Normal/Hover/..., Keyboard/Gamepad/...)처럼 기술적 식별자에 가까운 것들은 번역 대상에서 의도적으로 제외했습니다.

## 키보드 단축키 전체 목록

| 단축키 | 동작 |
| --- | --- |
| `Ctrl+A` | Select All |
| `Esc` | Clear Selection |
| `Delete` / `Backspace` | Delete Selection |
| `Ctrl+D` | Duplicate Selection |
| `Ctrl+C` / `Ctrl+V` | Copy / Paste |
| `Ctrl+G` / `Ctrl+Shift+G` | Group / Ungroup |
| 방향키 | 1px 이동 |
| `Shift` + 방향키 | 10px 이동 |
| `Ctrl+]` / `Ctrl+[` | Bring Forward / Send Backward |
| `Ctrl+Shift+]` / `Ctrl+Shift+[` | Bring To Front / Send To Back |
| `Alt+H` / `Alt+V` | Distribute Horizontal / Vertical |
| `Alt+L` / `Alt+C` / `Alt+R` | Align Left / Align Center X / Align Right |
| `Alt+T` / `Alt+M` / `Alt+B` | Align Top / Align Center Y / Align Bottom |
| `F` (뷰포트에 포커스) | 선택한 요소로 스크롤 이동 |
| `Ctrl+Z` / `Ctrl+Y` | Undo / Redo (Unity 기본 단축키) |

단축키는 `UIDesignerShortcutRegistry`(`Editor/Core/Commands/UIDesignerShortcut.cs`)에 정의되어 있고 `EditorPrefs`에 저장되어 재바인딩이 가능한 구조입니다. 다만 재바인딩 UI 패널은 아직 없습니다(TODO).

## 창/패널 크기 조절

**모든 주요 영역이 마우스로 드래그해서 늘리거나 줄일 수 있습니다.**

- 툴바 하단 경계 — 툴바 자체의 높이
- 좌측(Palette+Hierarchy+Align 패널) ↔ 가운데(뷰포트) 경계
- 가운데(뷰포트) ↔ 우측(Inspector 등) 경계
- 본문 ↔ 하단(State/Command/Screen Graph) 경계

각 분할선에 마우스를 올리면 바이올렛 색으로 강조되어 드래그 가능한 위치임을 알 수 있습니다. **조절한 위치는 자동으로 저장**되어 창을 닫았다 다시 열어도 유지됩니다.

## 화면 검증하기

툴바의 `Validate`를 누르거나 아래 메뉴를 실행합니다.

```text
Tools/NexUI/Designer/Validate Current Screen
```

검증은 누락된 선택, 지원되지 않는 백엔드 상태, 메타데이터 문제, 로컬라이제이션 링크, 반응형 규칙, 계약 문제 같은 정적 제작 오류를 보여줍니다. 우측 Validation 패널의 각 항목을 클릭하면 해당 요소가 Hierarchy/캔버스에서 선택됩니다.

## 저장하기

`Save`를 누르거나 아래 메뉴를 실행합니다.

```text
Tools/NexUI/Designer/Save Current Screen
```

디자이너는 저장 작업을 현재 백엔드의 serializer에 위임하고, 무엇이 실제로 디스크에 기록되었는지 콘솔과 툴바 상태에 정직하게 보고합니다. 저장 결과는 `DesignerSaveReport`로 요약됩니다.

### 무엇이 저장되나요 (persisted vs preview-only)

- **항상 저장됨**: `DesignerMetadataAsset` (element id, rect, shape, preview value/fill/image, binding, class, localization link, variant, responsive rule, contract, snapshot). `UIScreenDefinition`도 dirty로 표시 후 저장됩니다.
- **uGUI (프리팹 백엔드)**: 백엔드 에셋이 프리팹이면 디자이너 소유 데이터를 프리팹에 기록합니다.
  - RectTransform 위치/크기(좌상단 기준), active 상태
  - Label/Button/Toast 등의 텍스트(TMP 우선, 없으면 UnityEngine.UI.Text), 텍스트 색상/크기
  - Graphic/Image 틴트, Image가 없으면 추가
  - Button/IconButton에 Button 컴포넌트가 없으면 추가
  - `parentId` 기준 부모-자식 계층
  - 프리팹은 `LoadPrefabContents/SaveAsPrefabAsset/UnloadPrefabContents`로 안전하게 저장되어 기존 참조를 보존합니다. GameObject 이름이 중복되면 이름 기반 매칭이 불안정해질 수 있어 경고합니다.
  - **Shape/Preview Value/Fill Direction/Clockwise/Preview Image는 디자이너 프리뷰 전용**이라 프리팹에는 기록되지 않습니다 — 실제 라운드 처리, 진행률, 스프라이트는 프리팹의 실제 컴포넌트(Image.fillAmount, sprite 등)에서 별도로 설정해야 합니다.
- **UI Toolkit (UXML 백엔드)**: **UXML은 다시 쓰지 않습니다 (프리뷰/메타데이터 전용).** UXML/USS 저작은 UI Builder가 담당합니다. 저장 시 메타데이터만 기록하고, 메타데이터 element id와 UXML의 `name` 불일치를 검증해 경고로 보고합니다. 프리뷰에서 본 UXML 변경이 저장되었다고 절대 가장하지 않습니다.

### UI Toolkit 관련 명령

```text
Tools/NexUI/Designer/Backend/Sync Metadata From Backend    # UXML/프리팹의 이름을 메타데이터로 가져오기
Tools/NexUI/Designer/Backend/Apply Metadata To Preview     # 메타데이터를 라이브 프리뷰에만 반영 (저장 아님)
Tools/NexUI/Designer/Backend/Open Backend Asset In UI Builder
Tools/NexUI/Designer/Backend/Ping Backend Asset
```

## Figma 브리지 (진행 중)

```text
Tools/NexUI/Designer/Advanced/Figma Bridge
```

Figma 개인 액세스 토큰을 저장하고(EditorPrefs, 프로젝트별, VCS에 절대 포함되지 않음) 연결을 확인할 수 있습니다.

1. Figma 계정 설정에서 발급받은 Personal Access Token을 붙여넣고 **Save**.
2. **Test Connection**으로 토큰이 유효한지 확인.
3. Figma 파일 URL의 `figma.com/file/<fileKey>/...` 부분에서 fileKey를 복사해 입력하고 **Fetch File**로 원본 JSON 응답을 확인.

**아직 없는 것**: 실제 "프레임 → NexUI 요소" 매핑 엔진(Auto Layout 변환, 텍스트/폰트 매핑, 좌표 변환, 중첩 컴포넌트, 이름 기반 자동 바인딩)은 별도의 큰 후속 작업입니다. 지금은 연결과 원본 데이터 확인까지만 가능합니다.

## 마이그레이션 위저드

```text
Tools/NexUI/Migration Wizard
```

메이저 버전 업데이트로 네임스페이스/패키지 ID가 바뀌었을 때(예: 과거 `Hyojun.NexUI` → 현재 `emiteat.NexUI`), 프로젝트의 스크립트/씬/프리팹/에셋 파일을 스캔해 남아있는 옛 참조를 찾아줍니다. 파일별로 몇 군데가 바뀌는지 보여주고, 체크박스로 선택한 파일만 골라 일괄 치환할 수 있습니다. 변경 전 원본은 `.bak` 파일로 자동 백업됩니다.

## 권장 작업 흐름

1. `com.emiteat.nexui`에서 런타임 화면 데이터를 정의합니다.
2. 대응되는 `DesignerMetadataAsset`을 만듭니다.
3. 디자이너를 열고 화면을 할당합니다.
4. 팔레트에서 컴포넌트를 드래그해 배치하고, Layout/Style에서 위치·도형·색을 다듬습니다.
5. Progress Bar/Radial Fill 등 값 기반 컴포넌트는 Preview Value/Min/Max/Fill Direction으로 실제 느낌을 미리 확인합니다.
6. element id, class, binding, localization link를 추가합니다.
7. 검증 패널이 깨끗해질 때까지 문제를 수정합니다.
8. 저장한 뒤 Play Mode에서 실제 동작을 확인합니다.

## 알려진 제한 (TODO)

- 정렬 기준은 항상 선택 bounding box이며, Figma처럼 마지막 선택 요소(key object)를 기준으로 정렬하는 기능은 아직 없습니다.
- `Alt` 드래그로 이동 중 복제하는 기능은 아직 없습니다.
- 단축키 재바인딩 UI 패널은 아직 없습니다.
- Auto Layout/Constraints는 메타데이터 편집까지만 지원하며, 뷰포트 실시간 flexbox 반영과 UXML 자동 적용은 후속 작업입니다.
- 룰러/가이드, Figma 스타일 컬럼 그리드 오버레이는 아직 없습니다.
- 컴포넌트 인스턴스 오버라이드(마스터 컴포넌트 + 여러 화면에 걸친 인스턴스)는 설계가 보류된 상태입니다.
- Figma 브리지는 연결/원본 데이터 확인까지만 가능하고, 실제 프레임→요소 매핑 엔진은 없습니다.
