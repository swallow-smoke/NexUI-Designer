# NexUI Designer 사용자 가이드

> **Unity 안에서 화면을 배치하고, 구조를 만들고, 데이터를 연결하고, 검증한 뒤 Backend에 반영하는 전체 작업 흐름**

NexUI Designer는 Unity 프로젝트 안에서 UI 화면을 시각적으로 편집하기 위한 제작 도구입니다.  
이 문서는 처음 사용하는 디자이너도 화면 생성부터 저장과 검증까지 따라갈 수 있도록, 현재 제공되는 기능과 실제 사용 순서를 기준으로 설명합니다.

> [!IMPORTANT]
> 이 문서는 **현재 구현된 NexUI Designer의 사용 방법**을 설명합니다.  
> 장기적으로 계획된 전체 기능 범위는 [`FEATURE_SPEC.md`](./FEATURE_SPEC.md)에서 별도로 확인합니다.

---

## 문서 정보

| 항목 | 내용 |
| --- | --- |
| 대상 독자 | UI/UX 디자이너, 테크니컬 디자이너, Unity UI 개발자 |
| 문서 목적 | NexUI Designer의 실제 화면 제작 방법과 패널별 사용법 설명 |
| 지원 Backend | uGUI, UI Toolkit |
| 주요 데이터 | `UIScreenDefinition`, `DesignerMetadataAsset` |
| 주요 작업 | 화면 선택, 요소 배치, 계층 관리, 스타일 편집, Binding, 검증, 저장 |
| 권장 사용 모드 | 처음에는 **Simple**, 세부 설정이 필요할 때 **Advanced** |

---

## 현재 지원 범위

| 상태 | 의미 |
| --- | --- |
| **지원** | 현재 Designer에서 편집하거나 실행할 수 있음 |
| **부분 지원** | 메타데이터 편집 또는 Preview까지만 지원 |
| **진행 중** | 연결 구조는 있으나 핵심 변환 기능은 아직 없음 |
| **미지원** | 명세에는 있으나 현재 버전에서는 사용할 수 없음 |

### 주요 기능 상태

| 기능 | 상태 | 비고 |
| --- | --- | --- |
| 화면 및 Metadata 연결 | 지원 | 툴바에서 직접 할당 |
| Canvas 요소 배치 | 지원 | 선택, 이동, 크기 변경, 복제, 삭제 |
| 다중 선택 및 정렬 | 지원 | Align 패널과 단축키 제공 |
| 부모-자식 계층 | 지원 | `parentId` 기준 |
| Shape Preview | 지원 | Designer Preview 전용 |
| Progress/Radial/Spinner Preview | 지원 | 실제 형태를 Canvas에서 시각화 |
| Auto Layout | 부분 지원 | Metadata 편집 중심 |
| Constraints | 부분 지원 | Metadata 편집 중심 |
| Binding 설정 | 지원 | 일부 Picker 제공 |
| Motion 편집기 연결 | 지원 | Motion Graph/Clip Editor 열기 |
| uGUI Prefab 저장 | 지원 | Designer 소유 데이터 반영 |
| UI Toolkit UXML 자동 작성 | 미지원 | Metadata 및 Preview 중심 |
| Figma Bridge | 진행 중 | 연결과 원본 JSON 조회까지 |
| Runtime Live Edit | 제한적 | 실제 지원 범위는 버전별 확인 필요 |

---

# 빠른 시작

처음 사용하는 경우 아래 순서만 따라가면 기본 화면을 만들 수 있습니다.

1. Unity 메뉴에서 `Tools/NexUI/Designer`를 엽니다.
2. `UIScreenDefinition`을 만들거나 기존 화면을 선택합니다.
3. 툴바의 **Screen** 필드에 화면을 할당합니다.
4. **Metadata** 필드 옆 `New`를 눌러 `DesignerMetadataAsset`을 만듭니다.
5. `Rebuild`를 눌러 화면을 불러옵니다.
6. 좌측 **Components** 패널에서 Button, Text, Panel 등을 추가합니다.
7. Canvas에서 위치와 크기를 조절합니다.
8. 우측 Inspector에서 이름, 색상, 텍스트, Binding을 설정합니다.
9. `Validate`로 문제를 확인합니다.
10. `Save`로 Metadata와 지원되는 Backend 데이터를 저장합니다.

> [!TIP]
> 화면을 처음 만드는 동안은 **Simple 모드**를 유지하는 것이 좋습니다.  
> Binding, Theme, Motion, Policy까지 다뤄야 할 때만 Advanced 모드로 전환합니다.

---

# NexUI Designer의 기본 구조

NexUI Designer에서 화면 하나는 다음 데이터가 함께 구성합니다.

```text
UIScreenDefinition
├─ 화면의 런타임 정의
├─ Backend 정보
└─ 연결된 UI 에셋

DesignerMetadataAsset
├─ Element 목록
├─ 위치와 크기
├─ 부모-자식 관계
├─ Preview 전용 정보
├─ Binding 정보
├─ Theme / Motion / Accessibility 정보
└─ 디자이너 제작 보조 데이터
```

## 핵심 개념

| 용어 | 설명 |
| --- | --- |
| **Screen** | 메뉴, HUD, 인벤토리처럼 하나의 독립된 UI 화면 |
| **Element** | Panel, Text, Image, Button처럼 화면을 구성하는 개별 요소 |
| **Metadata** | Designer가 화면을 편집하기 위해 사용하는 제작 데이터 |
| **Backend** | Designer 결과가 실제 Unity UI로 연결되는 방식 |
| **Preview** | 실제 런타임을 실행하지 않고 Designer에서 확인하는 화면 |
| **Binding** | 게임 데이터와 UI 속성을 연결하는 설정 |
| **Element ID** | Binding과 Backend 요소를 연결하는 고유 식별자 |
| **Parent ID** | 요소의 부모를 지정하는 식별자 |
| **Shape** | Designer Canvas에서만 사용하는 요소 모양 |
| **Variant** | 상태나 용도에 따라 다른 표현을 선택하는 값 |

---

# 전체 작업 흐름

```text
화면 준비
   ↓
Screen / Metadata 연결
   ↓
Component 추가
   ↓
Hierarchy와 Layout 구성
   ↓
Style과 Preview 조정
   ↓
Binding / Theme / Motion 설정
   ↓
Validate
   ↓
Save
   ↓
Play Mode에서 실제 동작 확인
```

---

# 목차

## Part 1. 시작과 화면 준비

1. [Designer 열기](#1-designer-열기)
2. [Screen 준비하기](#2-screen-준비하기)
3. [Metadata 만들기](#3-metadata-만들기)
4. [화면 불러오기와 Rebuild](#4-화면-불러오기와-rebuild)
5. [창 레이아웃 이해하기](#5-창-레이아웃-이해하기)

## Part 2. 화면 디자인

6. [Component Palette](#6-component-palette)
7. [Hierarchy 패널](#7-hierarchy-패널)
8. [Canvas와 Viewport](#8-canvas와-viewport)
9. [선택과 다중 선택](#9-선택과-다중-선택)
10. [이동과 크기 변경](#10-이동과-크기-변경)
11. [정렬과 분배](#11-정렬과-분배)
12. [Layer 순서](#12-layer-순서)
13. [Group과 Ungroup](#13-group과-ungroup)
14. [Context Menu](#14-context-menu)

## Part 3. Inspector

15. [Inspector 개요](#15-inspector-개요)
16. [Layout](#16-layout)
17. [Auto Layout](#17-auto-layout)
18. [Style](#18-style)
19. [Shape](#19-shape)
20. [값 기반 Component Preview](#20-값-기반-component-preview)
21. [Image와 Icon Button](#21-image와-icon-button)
22. [Constraints](#22-constraints)
23. [Binding](#23-binding)
24. [Theme](#24-theme)
25. [Motion](#25-motion)
26. [Accessibility](#26-accessibility)
27. [화면 Policy와 Capability](#27-화면-policy와-capability)

## Part 4. 작업 환경과 보조 기능

28. [Simple / Advanced 모드](#28-simple--advanced-모드)
29. [Preview 상태와 입력 모드](#29-preview-상태와-입력-모드)
30. [History 패널](#30-history-패널)
31. [언어 변경](#31-언어-변경)
32. [패널 크기 조절](#32-패널-크기-조절)
33. [키보드 단축키](#33-키보드-단축키)

## Part 5. 검증과 저장

34. [화면 검증](#34-화면-검증)
35. [저장](#35-저장)
36. [저장되는 데이터와 Preview 전용 데이터](#36-저장되는-데이터와-preview-전용-데이터)
37. [uGUI 저장 방식](#37-ugui-저장-방식)
38. [UI Toolkit 저장 방식](#38-ui-toolkit-저장-방식)
39. [Backend 관련 명령](#39-backend-관련-명령)

## Part 6. 고급 기능

40. [Figma Bridge](#40-figma-bridge)
41. [Migration Wizard](#41-migration-wizard)
42. [권장 작업 흐름](#42-권장-작업-흐름)
43. [문제 해결](#43-문제-해결)
44. [알려진 제한](#44-알려진-제한)

---

# Part 1. 시작과 화면 준비

## 1. Designer 열기

Unity 상단 메뉴에서 다음 경로를 선택합니다.

```text
Tools/NexUI/Designer
```

Designer 창은 일반 Unity Editor Window처럼 Docking할 수 있습니다.

권장 배치는 다음과 같습니다.

- Game View와 나란히 배치
- Inspector보다 넓은 가로 공간 확보
- Motion Timeline을 사용할 경우 하단 높이를 충분히 확보
- 16:9 또는 Ultrawide 모니터에서는 중앙 Canvas 영역을 가장 넓게 설정

마지막으로 조정한 주요 패널 크기는 `EditorPrefs`에 저장되며, 창을 닫았다 다시 열어도 유지됩니다.

---

## 2. Screen 준비하기

NexUI Designer는 `UIScreenDefinition`을 기준으로 작업할 화면을 선택합니다.

### 기존 Screen 사용

1. Project 창에서 사용할 `UIScreenDefinition`을 찾습니다.
2. Designer 툴바의 **Screen** 필드에 드래그합니다.
3. `Rebuild`를 누릅니다.

### 새로운 Screen 사용

프로젝트에서 사용하는 NexUI Runtime 생성 메뉴를 통해 새로운 `UIScreenDefinition`을 만듭니다.

> [!NOTE]
> `UIScreenDefinition`은 단순한 Designer 파일이 아닙니다.  
> 실제 화면 식별자와 Backend 연결을 담당하는 런타임 정의입니다.

### Screen 선택 시 확인할 항목

- 화면 ID가 비어 있지 않은가
- 연결된 Backend가 올바른가
- Backend Asset이 실제로 존재하는가
- 같은 화면에 연결된 Metadata가 있는가
- uGUI와 UI Toolkit 중 어떤 방식으로 출력되는가

---

## 3. Metadata 만들기

Designer 전용 정보는 `DesignerMetadataAsset`에 저장합니다.

다음 메뉴에서 직접 만들 수 있습니다.

```text
Create > NexUI > Designer > Metadata
```

또는 Designer 툴바의 **Metadata** 필드 옆 `New` 버튼을 누릅니다.

### Metadata가 저장하는 정보

- Element ID
- 표시 이름
- Component Type
- 위치와 크기
- Shape
- 부모-자식 관계
- Preview Value
- Tint와 Text Color
- Font Size
- Style Class
- Binding Key
- Auto Layout
- Constraints
- Theme Token Override
- Motion Preset
- State Variant
- Accessibility 정보
- Localization 연결
- Validation Hint
- Responsive Rule
- Screen Variant

### Metadata에 저장하지 않는 것

- 실제 게임 데이터
- 런타임 State Store 값
- 사용자 세이브 데이터
- 게임 로직
- Command의 실제 구현
- 런타임에서 변경된 임시 값

> [!WARNING]
> Metadata를 삭제하면 Designer에서 만든 배치와 제작 정보가 사라질 수 있습니다.  
> Backend Prefab이나 UXML이 남아 있어도 Designer 상태가 완전히 복원된다고 보장할 수 없습니다.

---

## 4. 화면 불러오기와 Rebuild

Screen과 Metadata를 연결한 뒤 툴바의 `Rebuild`를 누릅니다.

`Rebuild`는 다음 작업을 수행합니다.

1. 현재 Screen과 Metadata를 다시 읽습니다.
2. Element 목록을 Hierarchy에 구성합니다.
3. Canvas Preview를 다시 만듭니다.
4. Backend 상태를 확인합니다.
5. 현재 선택이 유효하면 가능한 범위에서 유지합니다.
6. Inspector와 Validation 패널을 갱신합니다.

### Rebuild가 필요한 경우

- Screen을 변경한 뒤
- Metadata를 변경한 뒤
- 외부 스크립트가 Metadata를 수정한 뒤
- Backend Asset을 교체한 뒤
- Canvas 표시가 실제 데이터와 어긋난 경우
- Migration 이후
- Undo/Redo 후 Preview가 비정상적으로 보이는 경우

> [!TIP]
> `Rebuild`는 저장이 아닙니다.  
> 화면을 다시 읽고 Preview를 재구성하는 동작입니다.

---

## 5. 창 레이아웃 이해하기

기본 레이아웃은 다음과 같습니다.

```text
┌────────────────────────────────────────────────────────────────────┐
│ Toolbar                                                            │
│ Screen / Metadata / Mode / Preview / Input / Snap / Zoom / Actions │
├──────────────┬──────────────────────────────┬────────┬─────────────┤
│ Components   │                              │ Align  │ Inspector   │
│              │                              │        │ Validation  │
│ Hierarchy    │          Canvas              │ Layer  │ History     │
│              │                              │        │             │
├──────────────┴──────────────────────────────┴────────┴─────────────┤
│ State Preview       │ Command Preview       │ Screen Graph         │
└────────────────────────────────────────────────────────────────────┘
```

### Toolbar

화면과 제작 환경의 전역 상태를 조절합니다.

- Screen
- Metadata
- Simple / Advanced
- Preview State
- Input Mode
- Snap
- Grid Size
- Zoom
- Rebuild
- Save
- Validate

### 좌측 영역

- Component Palette
- Hierarchy

### 중앙 영역

- Canvas
- 선택 박스
- Resize Handle
- Box Selection
- Context Menu

### Align 영역

- Align
- Distribute
- Layer
- Group 관련 빠른 작업

### 우측 영역

- Inspector
- Validation
- History

### 하단 영역

- State Preview
- Command Preview
- Screen Graph

---

# Part 2. 화면 디자인

## 6. Component Palette

좌측 상단 **Components** 패널에서 화면에 추가할 UI 요소를 선택합니다.

### 기본 Component

| 카테고리 | Component |
| --- | --- |
| **Containers** | Panel, Card, Container, Modal |
| **Text & Media** | Text(Label), Image |
| **Actions & Input** | Button, Icon Button, Choice List |
| **Feedback & Status** | Toast, Tooltip, Popover, Progress Bar, Stat Bar, Radial Fill, Spinner, Skeleton |
| **Data & Lists** | List, Grid, Slot |

### Component 추가 방법

1. Components 패널에서 원하는 항목을 찾습니다.
2. Component 버튼을 클릭합니다.
3. 새 Element가 현재 Screen에 추가됩니다.
4. 추가된 Element가 자동으로 선택됩니다.
5. Inspector에서 이름과 스타일을 수정합니다.

### 검색

검색창에 이름 일부를 입력하면 일치하는 Component만 표시됩니다.

예시:

```text
button
progress
list
image
```

검색 결과가 있는 카테고리는 자동으로 펼쳐집니다.

### 카테고리 Foldout

각 카테고리는 접고 펼칠 수 있습니다.

Foldout 상태는 `EditorPrefs`에 저장되므로 창을 다시 열어도 유지됩니다.

### 자동 Shape

새 Component는 Type에 따라 기본 Shape가 자동 지정됩니다.

| Component | 기본 Shape |
| --- | --- |
| Spinner | Circle |
| Radial Fill | Circle |
| Toast | Pill |
| Tooltip | Pill |
| Icon Button | Pill |
| 나머지 | Rounded |

### Selection Actions

Palette 하단에는 현재 선택을 대상으로 하는 빠른 작업이 있습니다.

- Left
- Center
- Right
- Top
- Middle
- Bottom
- Fill
- Copy
- Delete

> [!IMPORTANT]
> Palette 하단 정렬은 주로 **Canvas 기준 정렬**입니다.  
> 여러 요소 사이를 정렬하거나 균등 분배할 때는 Align 패널을 사용합니다.

---

## 7. Hierarchy 패널

Hierarchy는 화면의 Element 구조와 순서를 보여줍니다.

### Hierarchy에서 할 수 있는 작업

- Element 선택
- 다중 선택
- 이름 검색
- ID 검색
- 표시/숨김
- 잠금/해제
- 부모-자식 구조 확인
- 선택 상태 확인
- Canvas 선택과 동기화

### 검색

검색창은 다음 값을 기준으로 필터링합니다.

- Element ID
- 표시 이름

### 표시/숨김

각 행의 표시 아이콘을 누르면 해당 Element를 Designer Canvas에서 숨깁니다.

이 기능은 다음 상황에서 유용합니다.

- 겹친 UI를 잠시 숨길 때
- 큰 배경 요소가 선택을 방해할 때
- 특정 그룹만 집중해서 편집할 때
- 복잡한 화면의 가독성을 높일 때

> [!NOTE]
> Designer에서 숨기는 것은 런타임 표시 상태와 별개입니다.

### 잠금

잠근 Element는 Canvas에서 다음 작업이 제한됩니다.

- 이동
- Resize Handle 사용
- 사각형 직접 편집

Inspector 필드 수정은 계속 가능합니다.

### 선택 동기화

Hierarchy와 Canvas 선택은 양방향으로 동기화됩니다.

- Hierarchy에서 선택하면 Canvas에서 강조
- Canvas에서 선택하면 Hierarchy에서 선택
- 다중 선택도 동일하게 반영

---

## 8. Canvas와 Viewport

Canvas는 실제 화면 배치가 이루어지는 중앙 작업 공간입니다.

### Zoom

다음 방법으로 확대와 축소가 가능합니다.

- 툴바 `-`
- 툴바 `+`
- 마우스 휠

### Grid와 Snap

툴바의 `Snap`을 활성화하면 이동과 배치가 Grid 단위에 맞춰집니다.

기본 Grid 크기:

```text
8px
```

8px 단위는 일반적인 UI spacing 체계와 잘 맞으므로 기본값으로 사용하기 적합합니다.

### Focus Selection

Canvas에 포커스가 있는 상태에서 `F`를 누르면 선택한 Element가 보이는 위치로 이동합니다.

### Outline 확인

Element Preview는 실제 Runtime UI의 완전한 렌더링 결과가 아닐 수 있습니다.

Canvas는 다음을 확인하는 데 집중합니다.

- 배치
- 크기
- 구조
- 색상
- 텍스트
- Component 의미
- 값 기반 Preview
- 부모-자식 관계

---

## 9. 선택과 다중 선택

### 단일 선택

Element를 클릭합니다.

### 선택 추가

```text
Shift + 클릭
```

기존 선택을 유지한 채 새 Element를 추가합니다.

### 선택 토글

```text
Ctrl + 클릭
```

선택된 Element는 해제하고, 선택되지 않은 Element는 추가합니다.

### Box Selection

빈 Canvas 영역을 드래그하면 선택 사각형과 겹치는 Element가 선택됩니다.

Modifier Key도 동일하게 적용됩니다.

- Shift + Drag: 기존 선택에 추가
- Ctrl + Drag: 선택 상태 토글

### 전체 선택

```text
Ctrl + A
```

현재 Screen의 선택 가능한 Element를 모두 선택합니다.

### 선택 해제

```text
Esc
```

### 겹친 Element 선택

여러 Element가 같은 위치에 겹쳐 있으면 우클릭 메뉴의 다음 항목을 사용합니다.

```text
Select Element/<Element Name>
```

가장 위에 있는 Element뿐 아니라 아래에 가려진 Element도 정확하게 선택할 수 있습니다.

---

## 10. 이동과 크기 변경

### 마우스 이동

선택한 Element를 드래그합니다.

### 다중 이동

여러 Element를 선택한 상태에서 선택된 Element 중 하나를 드래그하면 전체가 함께 이동합니다.

### 축 고정 이동

드래그 중 `Shift`를 누르면 이동량이 더 큰 축으로 고정됩니다.

예시:

- 가로 이동량이 더 크면 X축 고정
- 세로 이동량이 더 크면 Y축 고정

### 방향키 이동

| 입력 | 이동량 |
| --- | ---: |
| 방향키 | 1px |
| Shift + 방향키 | 10px |

### 크기 변경

선택한 Element의 우하단 Resize Handle을 드래그합니다.

현재 Resize Handle은 해당 Element 하나를 대상으로 합니다.

### 최소 크기

Inspector의 Size는 축별 최소 24px로 제한됩니다.

---

## 11. 정렬과 분배

Align 패널은 여러 Element의 위치를 정리합니다.

### Align

| 버튼 | 동작 | 단축키 |
| --- | --- | --- |
| ⇤ | 왼쪽 정렬 | `Alt+L` |
| ⇔x | 가로 중앙 정렬 | `Alt+C` |
| ⇥ | 오른쪽 정렬 | `Alt+R` |
| ⇡ | 위쪽 정렬 | `Alt+T` |
| ⇕y | 세로 중앙 정렬 | `Alt+M` |
| ⇣ | 아래쪽 정렬 | `Alt+B` |

### 여러 Element를 선택한 경우

선택된 전체 Bounding Box를 기준으로 정렬합니다.

예시:

```text
[Button A] [Button B] [Button C]
```

왼쪽 정렬을 실행하면 세 Element의 왼쪽 좌표가 선택 Bounds의 왼쪽에 맞춰집니다.

### Element 하나만 선택한 경우

Canvas 해상도를 기준으로 정렬합니다.

예시:

- Align Left → Canvas 왼쪽
- Align Center X → Canvas 가로 중앙
- Align Bottom → Canvas 아래쪽

### Distribute

3개 이상 선택해야 활성화됩니다.

| 버튼 | 동작 | 단축키 |
| --- | --- | --- |
| ⋯H | 가로 균등 분배 | `Alt+H` |
| ⋮V | 세로 균등 분배 | `Alt+V` |

분배는 첫 번째와 마지막 Element 사이의 공간을 기준으로 중간 Element를 균등하게 배치합니다.

> [!NOTE]
> 현재는 Figma의 Key Object 방식이 아니라 선택 Bounding Box를 기준으로 동작합니다.

---

## 12. Layer 순서

Element의 순서는 `Metadata.elements` 목록 순서와 연결됩니다.

이 순서는 다음 위치에 반영됩니다.

- Designer Canvas의 겹침 순서
- uGUI Sibling 순서
- UI Toolkit Hierarchy 변환 시 순서 기준

### Layer 명령

| 버튼 | 동작 | 단축키 |
| --- | --- | --- |
| Fwd | 한 단계 앞으로 | `Ctrl+]` |
| Back | 한 단계 뒤로 | `Ctrl+[` |
| Front | 맨 앞으로 | `Ctrl+Shift+]` |
| Bottom | 맨 뒤로 | `Ctrl+Shift+[` |

### 사용 예시

배경 Panel이 Button 위를 가리는 경우:

1. 배경 Panel 선택
2. `Ctrl+Shift+[`
3. 배경을 맨 뒤로 이동

Tooltip이 다른 UI 뒤에 가려지는 경우:

1. Tooltip 선택
2. `Ctrl+Shift+]`
3. Tooltip을 맨 앞으로 이동

---

## 13. Group과 Ungroup

### Group

2개 이상의 Element를 선택한 뒤 다음 명령을 실행합니다.

```text
Ctrl + G
```

또는 우클릭 메뉴에서 `Group`을 선택합니다.

Group을 만들면:

1. 선택 전체의 Bounding Box 크기를 가진 Panel이 생성됩니다.
2. 선택된 Element의 `parentId`가 새 Group의 ID로 변경됩니다.
3. 기존 Canvas 절대 좌표는 유지됩니다.
4. 새 Group이 부모 역할을 합니다.

### Ungroup

Group을 선택하고 다음 명령을 실행합니다.

```text
Ctrl + Shift + G
```

Ungroup을 실행하면:

1. Group의 자식들이 Group의 부모로 이동합니다.
2. Group Element가 삭제됩니다.
3. 자식의 Canvas 위치는 유지됩니다.

> [!WARNING]
> 현재 Group은 일반적인 디자인 툴의 완전한 로컬 좌표 Group과 다를 수 있습니다.  
> 절대 Canvas 좌표를 유지하면서 `parentId`를 재지정하는 방식입니다.

---

## 14. Context Menu

Canvas에서 마우스 오른쪽 버튼을 누르면 현재 위치와 선택 상태에 맞는 메뉴가 표시됩니다.

### Element 메뉴

- Select
- Add to Selection
- Select Children
- Select Parent
- Duplicate
- Delete
- Rename
- Bring Forward
- Send Backward
- Bring To Front
- Send To Back
- Align
- Distribute
- Group
- Ungroup
- Create Motion Clip From Selection

### 빈 Canvas 메뉴

빈 공간을 기준으로 사용할 수 있는 생성 또는 선택 명령이 표시됩니다.

### 겹친 Element 메뉴

클릭 지점에 여러 Element가 있으면 상단에 다음 목록이 추가됩니다.

```text
Select Element
├─ Background
├─ Panel
├─ Button
└─ Label
```

### Rename

Element를 더블클릭하면 인라인 이름 편집 필드가 열립니다.

| 입력 | 동작 |
| --- | --- |
| Enter | 이름 확정 |
| Esc | 취소 |

---

# Part 3. Inspector

## 15. Inspector 개요

Inspector는 선택한 대상에 따라 표시 내용이 달라집니다.

### Element 하나 선택

해당 Element의 모든 편집 가능한 속성을 표시합니다.

### 여러 Element 선택

상단에 `Selection` 요약이 나타납니다.

- 선택 개수
- 공통 위치
- 공통 크기
- Mixed 상태
- Layer 범위

### 선택 없음

화면 전체 설정 또는 안내 정보가 표시될 수 있습니다.

### 주요 Section

- Selection
- Layout
- Auto Layout
- Style
- Constraints
- Binding
- Theme
- Motion
- Accessibility
- Policy
- Capability
- Validation

---

## 16. Layout

Layout은 Canvas에서 Element가 차지하는 기본 사각형을 설정합니다.

| 필드 | 설명 |
| --- | --- |
| Position | Canvas 좌상단을 기준으로 한 X/Y 위치 |
| Size | Element의 Width/Height |
| Anchor | uGUI 스타일 Anchor Preset |
| Locked | Canvas 직접 편집 잠금 |

### Position

좌상단 기준 절대 Canvas 좌표를 사용합니다.

```text
Position X = 왼쪽에서 떨어진 거리
Position Y = 위에서 떨어진 거리
```

### Size

각 축은 최소 24px입니다.

### Anchor

Anchor는 Backend 저장 시 RectTransform 또는 Style 배치에 사용될 수 있습니다.

Stretch Anchor는 해당 축을 부모에 맞추는 의미를 가집니다.

### Locked

Locked를 활성화하면 Canvas에서 실수로 움직이는 것을 방지합니다.

---

## 17. Auto Layout

Auto Layout은 Element를 직계 자식의 배치 Container로 사용합니다.

### 기본 설정

| 필드 | 설명 |
| --- | --- |
| Auto Layout | Container 동작 활성화 |
| Direction | Row 또는 Column |
| Spacing | 자식 사이 간격 |
| Padding | Container 내부 여백 |
| Width | 자신이 부모 Layout 안에서 너비를 정하는 방식 |
| Height | 자신이 부모 Layout 안에서 높이를 정하는 방식 |

### Direction

| 값 | 설명 |
| --- | --- |
| Row | 왼쪽에서 오른쪽 |
| Column | 위에서 아래 |

### Self Sizing

| 값 | 설명 |
| --- | --- |
| Fixed | 현재 Rect 크기 사용 |
| Hug | 내용 크기에 맞춤 |
| Fill | 부모의 남는 공간 사용 |

### Padding

- Left
- Top
- Right
- Bottom

### 현재 지원 범위

> [!WARNING]
> 현재 Auto Layout은 주로 Metadata 편집을 지원합니다.  
> Canvas에서 완전한 Flexbox 배치를 실시간으로 재현하거나 UXML에 자동 적용하는 기능은 후속 작업입니다.

---

## 18. Style

Style은 Element의 시각적 Preview와 식별 정보를 설정합니다.

| 필드 | 설명 |
| --- | --- |
| Element ID | Backend와 Binding에서 사용하는 고유 ID |
| Name | Hierarchy에 표시되는 이름 |
| Type | Component의 의미론적 타입 |
| Text | 정적 Preview 텍스트 |
| Classes | 공백으로 구분된 Style Class |
| Shape | Canvas Preview 모양 |
| Tint | 배경 또는 Graphic 색상 |
| Text Color | 텍스트 색상 |
| Font Size | Preview Font Size |
| Hidden | Designer Canvas에서 숨김 |
| Preview Value | 값 기반 Component Preview 값 |
| Min Value | 최소 값 |
| Max Value | 최대 값 |
| Fill Direction | Progress 계열 채움 방향 |
| Clockwise | Radial 계열 방향 |
| Image | Image/Icon Button Preview Texture |

### Element ID

Element ID는 화면 내부에서 고유해야 합니다.

ID는 다음 기능에 사용됩니다.

- `parentId` 연결
- Binding Target 식별
- Command Source 식별
- Backend Element 매칭
- Validation 이동
- Runtime Lookup

ID를 변경하면 해당 Element를 부모로 참조하는 자식의 `parentId`가 자동으로 갱신됩니다.

> [!WARNING]
> ID 중복은 저장과 Backend 동기화에서 잘못된 요소를 수정하게 만들 수 있습니다.

### Name

Name은 사람이 읽기 위한 표시 이름입니다.

권장 예시:

```text
Inventory Root
Item Grid
Close Button
Player Health Bar
Item Tooltip
```

### Classes

여러 Class는 공백으로 구분합니다.

```text
primary large rounded
```

---

## 19. Shape

Shape는 Canvas Preview에서 Element의 외형을 구분하기 위한 설정입니다.

| 값 | 결과 |
| --- | --- |
| Rectangle | 각진 사각형 |
| Rounded | 8px 정도의 작은 Radius |
| Pill | 높이를 기준으로 완전히 둥근 형태 |
| Circle | 정사각형이면 원, 아니면 타원 |

### Shape 사용 목적

- 버튼과 Panel 구분
- 원형 Progress 확인
- Tooltip과 Toast의 형태 확인
- Icon Button Preview
- 실제 Sprite가 없어도 Layout 검토

> [!IMPORTANT]
> Shape는 현재 **Designer Preview 전용**입니다.  
> 실제 Prefab의 Sprite, USS Radius, Material에는 자동 적용되지 않습니다.

---

## 20. 값 기반 Component Preview

일부 Component는 단순 박스 대신 실제 의미를 보여주는 Preview를 렌더링합니다.

### Progress Bar / Stat Bar

구성:

```text
Background Track
└─ Filled Bar
```

채움 비율:

```text
(Preview Value - Min Value) / (Max Value - Min Value)
```

지원 방향:

- Left To Right
- Right To Left
- Bottom To Top
- Top To Bottom

### Radial Fill

- 배경 Ring
- 12시 방향에서 시작
- Preview Value 비율만큼 Arc 표시
- Clockwise로 방향 변경
- `Painter2D` 기반

### Spinner

- Ring 형태
- 지속 회전
- 별도 Texture 불필요

### Choice List

- Check Row 3개
- 첫 번째 Row는 선택 상태

### List

- 세로 Placeholder Row 3개

### Grid

- 2열 Placeholder Cell 6개

### Skeleton

- Placeholder Bar 3개
- Opacity 기반 Shimmer Animation

### Resize 대응

Preview 내부 구조는 퍼센트 기반이므로 Element 크기를 바꿔도 비율이 유지됩니다.

---

## 21. Image와 Icon Button

Type이 `Image` 또는 `Icon Button`이면 Style에 Image 필드가 나타납니다.

### Texture 설정

1. Project 창에서 `Texture2D`를 찾습니다.
2. Image 필드에 드래그합니다.
3. Canvas Preview를 확인합니다.

### Image Type

Texture가 Element 전체 영역에 `ScaleToFit` 방식으로 표시됩니다.

### Icon Button Type

- Button 배경은 Tint 유지
- Texture는 중앙 24×24 Icon으로 표시

> [!IMPORTANT]
> Image 필드는 현재 Designer Preview 용도입니다.  
> Runtime Sprite나 UXML Background Image를 자동으로 교체하지 않습니다.

---

## 22. Constraints

Constraints는 부모 크기가 변경될 때 자식 Element가 어떻게 반응할지 나타냅니다.

Advanced 모드에서만 표시됩니다.

### Horizontal

- Start
- End
- Center
- Scale

### Vertical

- Start
- End
- Center
- Scale

### 의미

| 값 | 설명 |
| --- | --- |
| Start | 시작 방향에 고정 |
| End | 끝 방향에 고정 |
| Center | 중앙 위치 유지 |
| Scale | 부모 크기 변화에 비례 |

> [!WARNING]
> 현재 Constraints는 Metadata 편집 중심입니다.  
> Responsive Preview에서 완전한 자동 재배치는 후속 작업입니다.

---

## 23. Binding

Binding은 UI와 게임 데이터를 연결하기 위한 설정입니다.

| 필드 | 데이터 형식 | 용도 |
| --- | --- | --- |
| Text Key | String | Text 변경 |
| Value Key | Float | Progress, Slider 계열 값 |
| Visibility Key | Bool | 표시/숨김 |
| Class Key | String | Style Class 변경 |
| Command Key | Command | 클릭 또는 Action 실행 |
| Interactable Key | Bool | 입력 가능 여부 |

### Text Key

```text
player.name
inventory.selectedItem.name
quest.title
```

### Value Key

```text
player.health
player.stamina
loading.progress
```

### Visibility Key

```text
inventory.isOpen
tooltip.isVisible
player.isUnderwater
```

### Command Key

```text
inventory.close
item.use
screen.open.settings
```

Command Key는 `Pick...`을 통해 프로젝트에서 발견된 Command를 선택할 수 있습니다.

### Bindable Property Picker

Text, Value, Interactable 일부 필드의 `Pick...`은 Reflection으로 찾은 `IBindableProperty<T>` Source를 표시합니다.

### Simple 모드

Command Key 중심으로 표시합니다.

### Advanced 모드

모든 Binding Channel을 표시합니다.

---

## 24. Theme

Theme Section은 공통 Design Token과 요소별 Override를 설정합니다.

| 필드 | 설명 |
| --- | --- |
| Theme | Theme Asset 직접 연결 |
| Theme ID | Runtime Theme 식별자 |
| Classes | Theme용 Style Class |
| Token Overrides | 요소별 Token Override |

### Theme Asset

Theme Asset을 선택하면 Theme ID가 자동으로 채워질 수 있습니다.

### Token Override

특정 Element만 공통 Theme 값을 덮어쓸 때 사용합니다.

예시:

```text
color.background = #1A1E24
color.accent = #73B7FF
radius.button = 12
spacing.content = 16
```

### 색상 추가

1. Color Swatch 클릭
2. Unity Color Picker에서 색상 선택
3. 필요하면 Eyedropper 사용
4. `Add as Token` 실행
5. Token Override 목록에 추가

---

## 25. Motion

Motion Section은 Element와 연결된 Motion 설정을 관리합니다.

### 상태별 Variant

- Initial
- Animate
- Exit
- Hover
- Pressed
- Focus

### 편집기 열기

- `Open Motion Graph`
- `Open Motion Clip Editor`

### Motion Graph

이벤트와 조건을 Node로 연결하는 방식입니다.

### Motion Clip

Timeline과 Keyframe 기반 애니메이션 방식입니다.

### Selection 기반 Clip

우클릭 메뉴의 다음 명령으로 선택한 Element를 대상으로 Clip 생성을 시작할 수 있습니다.

```text
Create Motion Clip From Selection
```

---

## 26. Accessibility

Accessibility는 Screen Reader와 입력 접근성을 위한 정보를 설정합니다.

### 주요 필드

- Accessibility Label
- Description
- Role

### Role 예시

- Button
- Toggle
- Slider
- Text
- Image
- List
- List Item

### 작성 기준

좋지 않은 Label:

```text
Button
Image
Icon
```

좋은 Label:

```text
인벤토리 닫기
현재 체력
선택한 아이템 장착
음량 조절
```

---

## 27. 화면 Policy와 Capability

### Policy

Policy는 개별 Element가 아니라 Screen 전체의 동작을 설정합니다.

Advanced 모드에서만 표시됩니다.

예시:

- Input Blocking
- Pause Game Behind
- Close On Back
- Cursor Policy
- Time Policy
- Focus Policy
- Conflict Policy
- Lifetime Policy

### Capability

Capability는 선택한 Component Type이 기대하는 런타임 인터페이스를 읽기 전용으로 표시합니다.

목적:

- Runtime 지원 여부 확인
- Component Type과 실제 Backend 구조 비교
- 미지원 기능 발견
- Validation 참고

---

# Part 4. 작업 환경과 보조 기능

## 28. Simple / Advanced 모드

툴바의 Mode 버튼으로 전환합니다.

### Simple

초기 화면 디자인에 필요한 핵심 기능만 표시합니다.

- Component Palette
- Canvas 편집
- 기본 Layout
- 기본 Style
- Auto Layout
- Command Key
- 기본 Preview

숨겨지는 항목:

- Theme 고급 설정
- Motion
- Policy
- Capability
- Constraints
- 세부 Binding Key

### Advanced

모든 Section과 설정을 표시합니다.

### 모드 저장

선택한 모드는 `EditorPrefs`에 저장됩니다.

### 권장 사용 기준

| 작업 | 권장 모드 |
| --- | --- |
| 화면 초안 | Simple |
| 기본 HUD 배치 | Simple |
| Button Command 연결 | Simple |
| Theme Token 편집 | Advanced |
| 반응형 Constraints | Advanced |
| Motion 연결 | Advanced |
| 화면 Policy | Advanced |
| 전체 Binding 설정 | Advanced |

---

## 29. Preview 상태와 입력 모드

Toolbar의 Preview 관련 필드는 실제 런타임 환경을 간단히 가정하기 위한 값입니다.

### State Preview

예시:

- Normal
- Hover
- Pressed
- Focused
- Disabled

### Input Mode

예시:

- Keyboard
- Gamepad
- Pointer
- Touch

기술적 식별자 성격이 강한 State와 Input 이름은 Designer 언어가 한국어여도 영어로 유지될 수 있습니다.

---

## 30. History 패널

History는 현재 Designer 세션에서 실행된 편집 작업을 최신순으로 표시합니다.

예시:

```text
Edit NexUI Element Tint
Move NexUI Elements
Resize NexUI Element
Create NexUI Group
Delete NexUI Elements
```

### 저장 개수

최대 50개

### History의 역할

- 방금 변경한 작업 확인
- 의도치 않은 변경 추적
- Undo 전 작업 이름 확인
- 여러 수정 사이의 흐름 파악

### 제한

- 특정 과거 지점으로 Jump 불가
- Editor 재시작 시 초기화
- 창을 닫았다 열면 초기화될 수 있음
- 읽기 전용 Log

실제 되돌리기는 Unity Undo를 사용합니다.

```text
Ctrl + Z
Ctrl + Y
```

---

## 31. 언어 변경

다음 메뉴에서 언어를 변경합니다.

```text
Tools/NexUI/Designer/Language/Korean
Tools/NexUI/Designer/Language/English
```

### 번역 대상

- 패널 제목
- Inspector Section
- 필드 이름
- 버튼
- Tooltip
- Validation 메시지

### 번역하지 않는 항목

기술 식별자에 가까운 값은 영어를 유지할 수 있습니다.

예시:

- Panel
- Button
- Normal
- Hover
- Keyboard
- Gamepad

### 적용 방식

언어를 바꾸면 창을 다시 열지 않아도 즉시 다시 그려집니다.

누락된 번역은 다음 순서로 Fallback됩니다.

1. 현재 언어
2. 영어 기본 문자열
3. Localization Key

---

## 32. 패널 크기 조절

주요 영역 사이의 분할선을 드래그할 수 있습니다.

### 조절 가능한 경계

- Toolbar 아래
- 좌측과 Canvas 사이
- Canvas와 우측 Inspector 사이
- 본문과 하단 Preview 사이

### 시각적 표시

분할선에 마우스를 올리면 강조 표시가 나타납니다.

### 저장

조절한 크기는 자동으로 `EditorPrefs`에 저장됩니다.

### 권장 비율

디자인 중심:

```text
좌측 20% / Canvas 55% / 우측 25%
```

Binding·Inspector 중심:

```text
좌측 18% / Canvas 47% / 우측 35%
```

Motion 작업:

```text
하단 영역 높이 확대
```

---

## 33. 키보드 단축키

| 단축키 | 동작 |
| --- | --- |
| `Ctrl+A` | 전체 선택 |
| `Esc` | 선택 해제 |
| `Delete` / `Backspace` | 선택 삭제 |
| `Ctrl+D` | 선택 복제 |
| `Ctrl+C` | 복사 |
| `Ctrl+V` | 붙여넣기 |
| `Ctrl+G` | Group |
| `Ctrl+Shift+G` | Ungroup |
| 방향키 | 1px 이동 |
| `Shift` + 방향키 | 10px 이동 |
| `Ctrl+]` | 한 단계 앞으로 |
| `Ctrl+[` | 한 단계 뒤로 |
| `Ctrl+Shift+]` | 맨 앞으로 |
| `Ctrl+Shift+[` | 맨 뒤로 |
| `Alt+H` | 가로 균등 분배 |
| `Alt+V` | 세로 균등 분배 |
| `Alt+L` | 왼쪽 정렬 |
| `Alt+C` | 가로 중앙 정렬 |
| `Alt+R` | 오른쪽 정렬 |
| `Alt+T` | 위쪽 정렬 |
| `Alt+M` | 세로 중앙 정렬 |
| `Alt+B` | 아래쪽 정렬 |
| `F` | 선택 Element로 이동 |
| `Ctrl+Z` | Undo |
| `Ctrl+Y` | Redo |

단축키 정의:

```text
Editor/Core/Commands/UIDesignerShortcut.cs
```

단축키는 `EditorPrefs`에 저장할 수 있는 구조지만, 현재 재바인딩 UI는 제공되지 않습니다.

---

# Part 5. 검증과 저장

## 34. 화면 검증

Toolbar의 `Validate`를 누르거나 다음 메뉴를 실행합니다.

```text
Tools/NexUI/Designer/Validate Current Screen
```

### 검사 대상

- Screen 선택 누락
- Metadata 누락
- Backend 상태
- Element ID 문제
- 부모 참조 문제
- Localization Link
- Binding 정보
- Responsive Rule
- Contract
- Component Capability
- 저장 불가능한 상태

### Validation 결과

우측 Validation 패널에 표시됩니다.

### 항목 선택

Validation 항목을 클릭하면 관련 Element가 다음 위치에서 선택됩니다.

- Hierarchy
- Canvas
- Inspector

### 권장 처리 순서

1. Error
2. Warning
3. Info

> [!TIP]
> Save 전에 Validation 패널을 확인하는 습관을 권장합니다.

---

## 35. 저장

Toolbar의 `Save`를 누르거나 다음 메뉴를 실행합니다.

```text
Tools/NexUI/Designer/Save Current Screen
```

저장은 현재 Backend Serializer에 위임됩니다.

저장 결과는 `DesignerSaveReport`로 요약됩니다.

### 저장 결과에서 확인할 내용

- 저장된 Metadata
- 수정된 Backend Asset
- 생성된 Component
- 경고
- 오류
- Preview 전용으로 남은 값
- 실제 디스크에 반영되지 않은 값

> [!IMPORTANT]
> Designer는 저장되지 않은 값을 저장된 것처럼 표시하지 않습니다.  
> Backend가 지원하지 않는 항목은 Report 또는 Warning으로 구분합니다.

---

## 36. 저장되는 데이터와 Preview 전용 데이터

### 항상 저장되는 Metadata

- Element ID
- Rect
- Shape
- Preview Value
- Fill Direction
- Preview Image Reference
- Binding
- Class
- Localization Link
- Variant
- Responsive Rule
- Contract
- Snapshot

`UIScreenDefinition`도 Dirty 상태로 표시된 뒤 저장됩니다.

### Preview 전용 항목

현재 다음 항목은 Backend에 자동 반영되지 않을 수 있습니다.

- Shape
- Preview Value
- Fill Direction
- Clockwise
- Preview Image
- 일부 Auto Layout
- 일부 Constraints

### 구분표

| 데이터 | Metadata | uGUI Prefab | UI Toolkit UXML |
| --- | --- | --- | --- |
| Position / Size | 저장 | 지원 시 반영 | 자동 재작성 안 함 |
| Text | 저장 | 지원 시 반영 | 자동 재작성 안 함 |
| Tint | 저장 | 지원 시 반영 | 자동 재작성 안 함 |
| Shape | 저장 | Preview 전용 | Preview 전용 |
| Preview Value | 저장 | Preview 전용 | Preview 전용 |
| Binding Key | 저장 | Runtime 연결 기준 | Runtime 연결 기준 |
| Parent ID | 저장 | Hierarchy 반영 | 검증 기준 |
| Preview Image | 저장 | 자동 Sprite 변경 안 함 | 자동 Background 변경 안 함 |

---

## 37. uGUI 저장 방식

Backend Asset이 Prefab이면 Designer가 소유하는 데이터를 Prefab에 반영합니다.

### 반영 대상

- RectTransform 위치
- RectTransform 크기
- Active 상태
- Text
- Text Color
- Font Size
- Graphic Tint
- Image Component
- Button Component
- 부모-자식 관계
- Sibling 순서

### Text Component 우선순위

1. TMP Text
2. UnityEngine.UI.Text

### 자동 Component 추가

- Graphic이 필요한데 Image가 없으면 Image 추가 가능
- Button 또는 Icon Button인데 Button Component가 없으면 추가 가능

### 부모-자식 구조

`parentId`를 기준으로 Prefab Hierarchy를 구성합니다.

### Prefab 저장 API

```text
LoadPrefabContents
SaveAsPrefabAsset
UnloadPrefabContents
```

이 방식은 기존 Prefab 참조를 최대한 보존하면서 안전하게 수정하기 위한 방식입니다.

### 이름 중복 경고

GameObject 이름이 중복되면 이름 기반 매칭이 불안정해질 수 있습니다.

가능하면 Element ID와 GameObject 이름을 명확하게 유지합니다.

---

## 38. UI Toolkit 저장 방식

현재 UI Toolkit Backend에서는 Designer가 UXML을 다시 쓰지 않습니다.

### 지원되는 것

- Metadata 저장
- Preview 적용
- UXML 이름과 Metadata ID 비교
- 불일치 Validation
- UI Builder 열기
- Backend Asset Ping

### 지원되지 않는 것

- UXML 자동 재작성
- USS 자동 생성
- Auto Layout의 완전한 USS 변환
- Shape의 Border Radius 자동 적용
- Preview Texture의 Background Image 자동 적용

> [!WARNING]
> Canvas Preview에서 변경된 모습이 UXML에 저장되었다고 가정하면 안 됩니다.  
> UI Toolkit의 실제 UXML/USS 제작은 현재 UI Builder가 담당합니다.

---

## 39. Backend 관련 명령

```text
Tools/NexUI/Designer/Backend/Sync Metadata From Backend
Tools/NexUI/Designer/Backend/Apply Metadata To Preview
Tools/NexUI/Designer/Backend/Open Backend Asset In UI Builder
Tools/NexUI/Designer/Backend/Ping Backend Asset
```

### Sync Metadata From Backend

Backend에 존재하는 이름과 구조를 Metadata로 가져옵니다.

사용 시점:

- 기존 Prefab을 Designer에 연결할 때
- 기존 UXML 이름을 Metadata와 맞출 때
- 외부에서 Backend가 수정되었을 때

### Apply Metadata To Preview

Metadata를 현재 Live Preview에만 적용합니다.

> 저장이 아닙니다.

### Open Backend Asset In UI Builder

UI Toolkit Backend Asset을 UI Builder에서 엽니다.

### Ping Backend Asset

Project 창에서 연결된 Backend Asset의 위치를 표시합니다.

---

# Part 6. 고급 기능

## 40. Figma Bridge

다음 메뉴에서 엽니다.

```text
Tools/NexUI/Designer/Advanced/Figma Bridge
```

### 현재 지원

- Personal Access Token 저장
- Connection Test
- File Key 입력
- Figma File 요청
- Raw JSON 확인
- 기본 연결 상태 확인

### Token 저장

Token은 프로젝트별 `EditorPrefs`에 저장됩니다.

- Git에 포함되지 않음
- Project Asset으로 저장되지 않음
- Source Code에 기록되지 않음

### 사용 순서

1. Figma에서 Personal Access Token 발급
2. Token 입력
3. `Save`
4. `Test Connection`
5. Figma URL에서 File Key 추출
6. File Key 입력
7. `Fetch File`
8. Raw JSON 확인

Figma URL 예시:

```text
https://www.figma.com/file/<fileKey>/<fileName>
```

### 현재 미지원

- Frame → NexUI Element 자동 변환
- Auto Layout 변환
- Font Mapping
- Text Style Mapping
- 좌표 변환
- 중첩 Component Mapping
- 이름 기반 자동 Binding
- Image Asset 자동 Import
- Variant 변환

---

## 41. Migration Wizard

다음 메뉴에서 엽니다.

```text
Tools/NexUI/Migration Wizard
```

Migration Wizard는 과거 Package ID나 Namespace 참조를 현재 구조로 변환할 때 사용합니다.

예시:

```text
Hyojun.NexUI
→
emiteat.NexUI
```

### 검사 대상

- C# Script
- Scene
- Prefab
- Asset
- Serialized Text

### 사용 순서

1. Migration Wizard 열기
2. 프로젝트 Scan
3. 변경 대상 파일 확인
4. 파일별 Match Count 확인
5. 변경할 파일 선택
6. Migration 실행
7. Console과 결과 확인
8. Unity Reimport 및 Compile 확인

### Backup

변경 전 원본은 `.bak` 파일로 자동 백업됩니다.

> [!WARNING]
> Migration 후에는 반드시 Git Diff를 확인하고, Scene과 Prefab을 열어 Serialized Reference를 검증합니다.

---

## 42. 권장 작업 흐름

### 기본 화면 제작

1. Runtime에서 `UIScreenDefinition` 준비
2. `DesignerMetadataAsset` 생성
3. Designer 열기
4. Screen과 Metadata 연결
5. `Rebuild`
6. Root Panel 추가
7. 주요 Container 배치
8. Text, Button, Image 추가
9. Hierarchy 이름 정리
10. Element ID 정리
11. Align과 Distribute로 배치 정리
12. Style 설정
13. Validate
14. Save
15. Play Mode 확인

### HUD 제작

1. 기준 해상도 확인
2. Root HUD Panel 생성
3. 체력, 스태미나, 산소 Component 추가
4. Anchor 설정
5. Progress Preview Value 설정
6. Value Binding Key 연결
7. Accessibility Label 작성
8. Gamepad Focus가 필요 없는 항목 확인
9. Validate
10. Save

### 인벤토리 제작

1. Root Modal 또는 Panel 생성
2. Header / Content / Footer Container 구성
3. Grid Component 추가
4. Item Slot Placeholder 배치
5. Close Button 추가
6. Command Key 연결
7. Selection과 Group 기능으로 구조 정리
8. Tooltip Element 준비
9. Layer 순서 확인
10. Validate
11. Save

### 모바일 UI 제작

1. 기준 해상도 설정
2. Safe Area 고려
3. Touch Target 크기 확보
4. Button과 Icon Button 최소 크기 확인
5. Constraints Metadata 설정
6. Portrait/Landscape 조건 계획
7. Validate
8. 실제 Device 또는 Game View 확인

### Motion 적용

1. 대상 Element 선택
2. Advanced 모드 전환
3. Motion Section 열기
4. Motion Clip 또는 Graph 선택
5. 상태별 Variant 입력
6. Motion Editor에서 편집
7. Preview 실행
8. Stop 후 원본 상태 복원 확인
9. Save
10. Play Mode 확인

---

## 43. 문제 해결

### Canvas에 아무것도 표시되지 않음

확인 순서:

1. Screen이 할당되었는가
2. Metadata가 할당되었는가
3. `Rebuild`를 눌렀는가
4. Element가 Hidden 상태인가
5. Zoom이 너무 작거나 큰가
6. Element 위치가 Canvas 밖인가
7. Validation에 Error가 있는가

### Element가 움직이지 않음

- Locked 확인
- 부모 또는 Group 상태 확인
- Canvas가 아닌 다른 패널에 포커스가 있는지 확인
- Snap 간격이 너무 큰지 확인

### Element가 선택되지 않음

- Hidden 상태 확인
- Locked 상태 확인
- 다른 Element가 위에 겹쳐 있는지 확인
- 우클릭 `Select Element` 사용
- Hierarchy에서 직접 선택

### 저장했는데 Prefab이 바뀌지 않음

- Backend가 uGUI Prefab인지 확인
- `DesignerSaveReport` 확인
- Preview 전용 속성인지 확인
- Prefab이 실제 Backend Asset으로 연결되었는지 확인
- Validation Warning 확인

### UI Toolkit 변경이 UXML에 반영되지 않음

현재 정상 동작입니다.

Designer는 UXML을 자동 재작성하지 않습니다.

다음 명령으로 UI Builder를 엽니다.

```text
Tools/NexUI/Designer/Backend/Open Backend Asset In UI Builder
```

### Binding Picker가 비어 있음

- 프로젝트 Compile Error 확인
- `IBindableProperty<T>` 구현 확인
- Assembly Definition 참조 확인
- Reflection 검색 대상 Assembly 확인
- 원하는 Type과 Binding Channel Type 일치 확인

### Undo 후 Preview가 이상함

1. `Rebuild`
2. Selection 해제
3. Metadata 상태 확인
4. Validation 실행

### ID 변경 후 자식이 사라짐

- 자식의 `parentId` 확인
- ID 중복 확인
- Validation 실행
- Hierarchy 검색 해제

---

## 44. 알려진 제한

### 편집

- Figma Key Object 기준 정렬 미지원
- Alt+Drag 복제 미지원
- 단축키 재바인딩 UI 미지원
- Ruler와 Guide 미지원
- Column Grid Overlay 미지원
- 완전한 로컬 좌표 Group 미지원

### Layout

- Auto Layout 실시간 Flexbox Preview 제한
- Constraints 실시간 반응형 재배치 제한
- UXML 자동 Layout 반영 미지원

### Component

- Master Component와 Instance Override 시스템 미완성
- 여러 Screen에 걸친 Component Instance 동기화 미지원

### Backend

- Shape 자동 저장 미지원
- Preview Value 자동 Runtime Component 반영 미지원
- Preview Image 자동 Sprite 적용 미지원
- UI Toolkit UXML/USS 자동 저작 미지원

### Figma

- 연결과 Raw Data 확인까지만 지원
- Frame 변환 엔진 미지원
- Font와 Asset Mapping 미지원
- Variant와 Auto Layout 변환 미지원

---

# 문서와 실제 UI가 다른 경우

NexUI Designer는 개발 중인 프로젝트이므로 문서와 Editor UI 사이에 차이가 생길 수 있습니다.

차이가 발견되면 다음 정보를 함께 기록하는 것이 좋습니다.

- Unity Version
- NexUI Designer Version
- Backend Type
- Screen Asset
- Metadata Asset
- 재현 순서
- Console Error
- 문제 화면 Screenshot

---

# 관련 문서

| 문서 | 설명 |
| --- | --- |
| [`README.md`](../README.md) | 프로젝트 소개 |
| [`FEATURE_SPEC.md`](./FEATURE_SPEC.md) | 전체 목표 기능 명세 |
| `ROADMAP.md` | 구현 순서와 우선순위 |
| `CHANGELOG.md` | 버전별 변경 내역 |
| `CONTRIBUTING.md` | 기여 방법 |
| `API_REFERENCE.md` | 확장 API와 Runtime 연동 |

---

# 최종 확인 목록

화면 작업을 끝내기 전에 다음 항목을 확인합니다.

- [ ] Screen과 Metadata가 올바르게 연결되어 있다.
- [ ] Element ID가 중복되지 않는다.
- [ ] Hierarchy 이름을 보고 구조를 이해할 수 있다.
- [ ] 부모-자식 관계가 올바르다.
- [ ] 화면 밖으로 나간 Element가 없다.
- [ ] Text가 잘리거나 Overflow되지 않는다.
- [ ] Button과 입력 Element에 Command가 연결되어 있다.
- [ ] 필요한 Binding Key가 연결되어 있다.
- [ ] Theme와 색상이 프로젝트 규칙을 따른다.
- [ ] Accessibility Label이 작성되어 있다.
- [ ] Layer 순서가 올바르다.
- [ ] Validation Error가 없다.
- [ ] Save Report에 예상하지 못한 Warning이 없다.
- [ ] Play Mode에서 실제 동작을 확인했다.
