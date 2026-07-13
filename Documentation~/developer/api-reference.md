# NexUI Designer 확장 API

이 문서는 주요 확장 지점을 요약합니다. 네임스페이스 루트는 `emiteat`입니다.

## Editor Window

### `emiteat.NexUI.Designer.Editor.NexUIDesignerWindow`

메인 Unity Editor 창입니다. (네임스페이스는 `emiteat.NexUI.Designer.Editor` 이며 `.Core` 하위가 아닙니다.)

담당 역할:

- 전체 레이아웃 소유
- 활성 `NexUIDesignerContext` 보관
- 툴바, 계층, 뷰포트, 인스펙터, 검증, 상태, 커맨드, 그래프 패널 연결
- rebuild, validation, save 실행

### `emiteat.NexUI.Designer.Editor.NexUIDesignerContext`

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

백엔드별 구현이 사용하는 저장 계약입니다. `Save(UIScreenDefinition, DesignerMetadataAsset)`는 무엇이 디스크에 기록되었고 무엇이 프리뷰 전용으로 건너뛰어졌는지를 담은 `DesignerSaveReport`를 반환합니다.

- `UIToolkitAssetSerializer` — companion-save 모드. 메타데이터만 저장하고 UXML은 다시 쓰지 않습니다. UXML/USS 저작은 UI Builder가 담당합니다. 메타데이터와 UXML 이름 불일치를 검증해 보고합니다.
- `UGUIAssetSerializer` — 프리팹 기반 저장. `PrefabUtility.LoadPrefabContents → SaveAsPrefabAsset → UnloadPrefabContents` 패턴으로 RectTransform/텍스트/틴트/Button 등 디자이너 소유 데이터를 프리팹에 반영합니다.
- `DesignerSerializerRegistry.Get(backend)` — 백엔드에 맞는 serializer를 반환합니다.

`DesignerSaveReport`는 `Changed`, `Skipped`, `Warnings`, `Errors` 리스트와 `Summary()`, `Details()`를 제공합니다.

## Metadata

### `emiteat.NexUI.Designer.DesignerMetadataAsset`

디자이너 전용 화면 메타데이터의 루트 에셋입니다. (런타임 안전 메타데이터의 실제 네임스페이스는 `emiteat.NexUI.Designer` 이며, 파일 위치는 `Runtime/Metadata/` 입니다. 이 어셈블리는 `UnityEditor`를 참조하지 않습니다.)

element id, binding, localization link, responsive data, variant, contract, snapshot data 등 제작 메타데이터를 저장합니다.

## Validation

### `emiteat.NexUI.Designer.Editor.Validation.DesignerValidationService`

`Validate(UIScreenDefinition, DesignerMetadataAsset)`는 구조화된 `DesignerValidationIssue` 리스트를 반환합니다. 각 이슈는 `Severity(Info/Warning/Error)`, 안정적인 `Code`, `ScreenId`, `ElementId`, 사람이 읽는 `Message`, 제안 `Fix`를 담습니다.

주요 규칙(코드): `no-screen`, `empty-screen-id`, `backend-asset-missing`, `backend-type-mismatch`, `unsupported-backend`, `no-metadata`, `metadata-screen-mismatch`, `empty-element-id`, `duplicate-element-id`, `invalid-element-id`, `missing-backend-element`, `orphan-backend-element`, `button-without-command`, `button-without-text`, `small-touch-target`, `hidden-but-interactive`, `localization-target-missing`, `localization-key-missing`, `variant-target-missing`, `responsive-target-missing`, `duplicate-gameobject-name`, `ugui-missing-button`, `ugui-missing-text`, `ugui-missing-graphic`, `ugui-modal-without-canvasgroup`.

검증 패널에서 `ElementId`가 있는 이슈를 클릭하면 해당 요소가 선택됩니다.

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
