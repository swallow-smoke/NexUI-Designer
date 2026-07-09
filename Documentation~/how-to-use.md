# 빠른 시작

## 디자이너 열기

`Tools/NexUI/Designer`를 실행합니다.

창은 툴바, 계층, 뷰포트, 인스펙터, 검증 패널, 상태 패널, 커맨드 패널, 화면 그래프 패널로 구성됩니다.

## 화면 선택하기

1. `UIScreenDefinition`을 만들거나 선택합니다.
2. 툴바의 `Screen` 필드에 할당합니다.
3. `Rebuild`를 누릅니다.

뷰포트에는 현재 프리뷰 셸이 표시됩니다. 계층과 인스펙터는 선택한 화면과 디자이너 메타데이터를 기준으로 갱신됩니다.

## 메타데이터 만들기

디자이너 전용 데이터는 `DesignerMetadataAsset`에 저장합니다.

아래 메뉴에서 만들 수 있습니다.

```text
Create > NexUI > Designer > Metadata
```

메타데이터에는 런타임 UI 로직이 아니라 제작을 돕기 위한 정보를 넣습니다.

- element id
- style class
- binding key
- validation hint
- localization link
- preview row
- screen variant

## 화면 검증하기

툴바의 `Validate`를 누르거나 아래 메뉴를 실행합니다.

```text
Tools/NexUI/Designer/Validate Current Screen
```

검증은 누락된 선택, 지원되지 않는 백엔드 상태, 메타데이터 문제, 로컬라이제이션 링크, 반응형 규칙, 계약 문제 같은 정적 제작 오류를 보여줍니다.

## 저장하기

`Save`를 누르거나 아래 메뉴를 실행합니다.

```text
Tools/NexUI/Designer/Save Current Screen
```

디자이너는 저장 작업을 현재 백엔드의 serializer에 위임하고, 무엇이 실제로 디스크에 기록되었는지 콘솔과 툴바 상태에 정직하게 보고합니다. 저장 결과는 `DesignerSaveReport`로 요약됩니다.

### 무엇이 저장되나요 (persisted vs preview-only)

- **항상 저장됨**: `DesignerMetadataAsset` (element id, rect, binding, class, localization link, variant, responsive rule, contract, snapshot). `UIScreenDefinition`도 dirty로 표시 후 저장됩니다.
- **uGUI (프리팹 백엔드)**: 백엔드 에셋이 프리팹이면 디자이너 소유 데이터를 프리팹에 기록합니다.
  - RectTransform 위치/크기(좌상단 기준), active 상태
  - Label/Button/Toast 등의 텍스트(TMP 우선, 없으면 UnityEngine.UI.Text), 텍스트 색상/크기
  - Graphic/Image 틴트, Image가 없으면 추가
  - Button/IconButton에 Button 컴포넌트가 없으면 추가
  - `parentId` 기준 부모-자식 계층
  - 프리팹은 `LoadPrefabContents/SaveAsPrefabAsset/UnloadPrefabContents`로 안전하게 저장되어 기존 참조를 보존합니다. GameObject 이름이 중복되면 이름 기반 매칭이 불안정해질 수 있어 경고합니다.
- **UI Toolkit (UXML 백엔드)**: **UXML은 다시 쓰지 않습니다 (프리뷰/메타데이터 전용).** UXML/USS 저작은 UI Builder가 담당합니다. 저장 시 메타데이터만 기록하고, 메타데이터 element id와 UXML의 `name` 불일치를 검증해 경고로 보고합니다. 프리뷰에서 본 UXML 변경이 저장되었다고 절대 가장하지 않습니다.

### UI Toolkit 관련 명령

```text
Tools/NexUI/Designer/Backend/Sync Metadata From Backend    # UXML/프리팹의 이름을 메타데이터로 가져오기
Tools/NexUI/Designer/Backend/Apply Metadata To Preview     # 메타데이터를 라이브 프리뷰에만 반영 (저장 아님)
Tools/NexUI/Designer/Backend/Open Backend Asset In UI Builder
Tools/NexUI/Designer/Backend/Ping Backend Asset
```

## 언어 바꾸기

아래 메뉴를 사용합니다.

```text
Tools/NexUI/Designer/Language/Korean
Tools/NexUI/Designer/Language/English
```

에디터 UI는 디자이너 로컬라이제이션 테이블을 사용합니다. 누락된 문자열은 창을 깨뜨리지 않고 키 또는 기본 문자열로 표시됩니다.

## 다중 선택 & 정렬 (Figma/Photoshop 스타일)

뷰포트가 이제 단일 선택뿐 아니라 다중 선택, 드래그 박스 선택, 우클릭 메뉴, 정렬/분배, 레이어 순서 변경을 지원합니다.

### 선택하기

- 빈 캔버스에서 드래그하면 사각형 영역과 겹치는 요소가 모두 선택됩니다.
- `Shift` + 클릭/드래그: 기존 선택에 추가
- `Ctrl` + 클릭/드래그: 선택 토글
- 계층(Hierarchy) 패널도 다중 선택을 지원하며 캔버스 선택과 양방향으로 동기화됩니다.

### 그룹 이동

- 다중 선택된 요소 중 하나를 드래그하면 선택된 요소 전체가 함께 이동합니다.
- 드래그 중 `Shift`를 누르면 이동량이 더 큰 축으로 고정됩니다(축 고정 이동).
- 리사이즈 핸들(우하단)을 드래그하면 그 요소 하나만 리사이즈됩니다.

### 우클릭 메뉴

빈 캔버스와 요소 각각에 대해 다른 메뉴가 뜹니다. 클릭 지점에 요소가 여러 개 겹쳐 있으면 메뉴 상단에 `Select Element/<이름>` 목록이 추가되어, 가장 위에 있는 요소만이 아니라 겹친 요소 중 원하는 것을 정확히 고를 수 있습니다.

요소 메뉴에는 Select/Add to Selection/Select Children/Select Parent, Duplicate/Delete/Rename, Bring Forward/Send Backward/Bring To Front/Send To Back, Align, Distribute, Group/Ungroup, Create Motion Clip From Selection이 포함됩니다.

### 정렬 / 분배

툴바 하단 행의 정렬/분배/레이어 버튼, 또는 우클릭 메뉴의 Align/Distribute 서브메뉴, 또는 단축키로 실행합니다. 2개 이상 선택 시 선택된 요소들의 bounding box를 기준으로 정렬하고, 1개만 선택했을 때는 기존처럼 캔버스 해상도를 기준으로 정렬합니다. Distribute는 3개 이상 선택했을 때 동작합니다.

### 레이어 순서

`Metadata.elements` 리스트 순서가 곧 z-order이며, uGUI/UI Toolkit 저장 시 sibling 순서에도 그대로 반영됩니다. Bring Forward/Send Backward/Bring To Front/Send To Back으로 조작합니다.

### 그룹

2개 이상 선택 후 Group을 실행하면 선택 영역의 bounding box 크기를 가진 새 Panel 요소가 만들어지고, 선택된 요소들의 `parentId`가 그 그룹으로 재지정됩니다(좌표는 절대 캔버스 좌표를 그대로 유지). Ungroup은 그룹의 자식들을 그룹의 부모로 되돌리고 그룹 요소를 삭제합니다.

### 단축키

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
| `Ctrl+Z` / `Ctrl+Y` | Undo / Redo (Unity 기본 단축키를 그대로 사용) |

단축키는 `UIDesignerShortcutRegistry`(`Editor/Core/Commands/UIDesignerShortcut.cs`)에 정의되어 있고 `EditorPrefs`에 저장되어 재바인딩이 가능한 구조입니다. 다만 재바인딩 UI 패널은 아직 없습니다(TODO).

### 알려진 제한 (TODO)

- 정렬 기준은 항상 선택 bounding box이며, Figma처럼 마지막 선택 요소(key object)를 기준으로 정렬하는 기능은 아직 없습니다.
- `Alt` 드래그로 이동 중 복제하는 기능은 아직 없습니다.
- 단축키 재바인딩 UI 패널은 아직 없습니다.

## 권장 작업 흐름

1. `com.emiteat.nexui`에서 런타임 화면 데이터를 정의합니다.
2. 대응되는 `DesignerMetadataAsset`을 만듭니다.
3. 디자이너를 열고 화면을 할당합니다.
4. element id, class, binding, localization link를 추가합니다.
5. 검증 패널이 깨끗해질 때까지 문제를 수정합니다.
6. 저장한 뒤 Play Mode에서 실제 동작을 확인합니다.
