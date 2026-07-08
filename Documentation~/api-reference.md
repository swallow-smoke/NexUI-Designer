# API 레퍼런스

이 문서는 주요 확장 지점을 요약합니다. 네임스페이스 루트는 `emiteat`입니다.

## Editor Window

### `emiteat.NexUI.Designer.Editor.Core.NexUIDesignerWindow`

메인 Unity Editor 창입니다.

담당 역할:

- 전체 레이아웃 소유
- 활성 `NexUIDesignerContext` 보관
- 툴바, 계층, 뷰포트, 인스펙터, 검증, 상태, 커맨드, 그래프 패널 연결
- rebuild, validation, save 실행

### `emiteat.NexUI.Designer.Editor.Core.NexUIDesignerContext`

현재 디자이너 세션의 공유 상태입니다.

대표 데이터:

- 선택된 `UIScreenDefinition`
- 선택된 메타데이터 에셋
- 활성 백엔드
- 현재 선택
- 검증 결과
- 프리뷰 모드

## Backend

### `emiteat.NexUI.Designer.Editor.Backend.INexUIDesignerBackend`

UI 구현을 디자이너에 연결하기 위한 백엔드 계약입니다.

새 화면 렌더링 경로를 지원할 때 이 인터페이스를 구현합니다.

기대 역할:

- 화면 지원 여부 보고
- 프리뷰 생성 또는 갱신
- 계층 항목 노출
- 선택된 요소 해석
- serializer와 저장 흐름 협력

## Serialization

### `emiteat.NexUI.Designer.Editor.Serialization.IDesignerAssetSerializer`

백엔드별 구현이 사용하는 저장 계약입니다.

UI Toolkit, uGUI 또는 다른 UI 표현 방식에 맞춘 저장 동작이 필요할 때 구현합니다.

## Metadata

### `emiteat.NexUI.Designer.Runtime.Metadata.DesignerMetadataAsset`

디자이너 전용 화면 메타데이터의 루트 에셋입니다.

element id, binding, localization link, responsive data, variant, contract, snapshot data 등 제작 메타데이터를 저장합니다.

## Panels

패널은 창에서 context를 받아 UI를 그리는 에디터 모듈입니다. 패널은 작게 유지하고, 복잡한 동작은 서비스로 분리하는 것을 권장합니다.

주요 패널:

- `NexUIDesignerToolbar`
- `NexUIDesignerHierarchy`
- `NexUIDesignerViewport`
- `NexUIDesignerInspector`
- `NexUIDesignerValidationPanel`
- `NexUIDesignerStatePanel`
- `NexUIDesignerCommandPanel`
- `NexUIDesignerScreenGraphPanel`

## Services

서비스는 검증, snapshot, diff, contract, cleanup, responsive rule, localization check, profiling, refactoring 같은 재사용 가능한 에디터 로직을 담습니다.

패널 클래스를 키우기 전에, 기능을 작고 명확한 서비스로 분리할 수 있는지 먼저 확인하세요.
