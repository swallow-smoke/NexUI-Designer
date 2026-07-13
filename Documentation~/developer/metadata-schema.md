# Metadata Schema

NexUI Designer의 데이터는 Runtime 화면 정의, Designer 제작 데이터와 독립 고급 도구 Asset으로 나뉩니다. Runtime 폴더의 타입은 `UnityEditor`를 참조하지 않지만, 모든 타입이 게임 실행에 자동 사용되는 것은 아닙니다.

```mermaid
flowchart LR
  Screen["UIScreenDefinition"] --> Backend["Prefab 또는 VisualTreeAsset"]
  Metadata["DesignerMetadataAsset"] --> Elements["DesignerElementMetadata[]"]
  Metadata --> ScreenMotion["DesignerScreenMotionMetadata"]
  ScreenMotion --> Clip["UIMotionClip"]
  ScreenMotion --> Graph["UIMotionGraphAsset"]
  ScreenMotion --> Machine["UIMotionStateMachine"]
  Scenario["DesignerScenarioAsset"] -. "Preview Key" .-> Metadata
  Manifest["DesignerPublishManifest"] -. "screenId/hash" .-> Generated[".g.uxml / .g.uss"]
  Flow["DesignerScreenFlowAsset"] -. "screenId" .-> Screen
```

## 주요 타입

| 타입 | Namespace / 파일 | 책임과 주요 필드 | 수정자 | Runtime |
| --- | --- | --- | --- | --- |
| `UIScreenDefinition` | `emiteat.NexUI.Core`, Core `Runtime/Core/UIScreenDefinition.cs` | identity, backendAsset, layer, motion, policy, focus, relations, validation, variants | 사용자/Designer 연결 도구 | UIManager가 읽음 |
| `DesignerMetadataAsset` | `emiteat.NexUI.Designer`, `Runtime/Metadata/DesignerMetadataAsset.cs` | screenId, elements, screenMotion, 고급 제작 Metadata | Designer | Runtime-safe 직렬화 형식, 주 용도는 제작 |
| `DesignerElementMetadata` | 같은 Namespace, `Runtime/Metadata/DesignerElementMetadata.cs` | ID, parent/sibling/slot, rect, style, Binding, Motion, Layout, Accessibility | Designer | Backend 변환/Preview 계약 |
| `DesignerScreenMotionMetadata` | `Runtime/Metadata/DesignerScreenMotionMetadata.cs` | entry/exit Clip, Trigger Binding, State Machine, Graph 참조 | Designer/Motion Inspector | Runtime 연결 시 사용 가능 |
| `DesignerScenarioAsset` | `Runtime/Metadata/DesignerScenarioAsset.cs` | Preview Binding 값, 상태, 언어, 환경, Timeline | Scenario Editor | 실제 게임 상태에는 사용 안 함 |
| `DesignerTokenSetAsset` | `Runtime/Metadata/DesignerTokenSetAsset.cs` | Token literal/alias | Token 도구 | Element Style 직접 연결 없음 |
| `DesignerScreenFlowAsset` | `Runtime/Metadata/DesignerScreenFlowAsset.cs` | Node, Transition, 시작 Node, Guard Key | Flow Editor | UIManager 자동 연결 없음 |
| `DesignerPublishManifest` | `Runtime/Metadata/DesignerPublishManifest.cs` | Screen별 UXML/USS 마지막 Publish Hash | Publish Service | Editor 동기화 기준 |

## Motion 참조

`DesignerMotionBinding`은 `bindingId`, `targetElementId`, Trigger, 선택적인 `stateId`/`commandId`, 기본/Reduced Motion Clip 참조를 가집니다. Clip 데이터를 Metadata 안에 복제하지 않습니다. Screen 단위 Entry/Exit은 `entryClip`/`exitClip`에 저장합니다. Element ID 변경은 참조 갱신 경로를 사용해야 하며 Serialized 필드를 직접 문자열 치환하면 안 됩니다.

## Companion JSON

`DesignerMetadataJsonSerializer`는 Metadata 교환용 JSON을 처리합니다. `.asset`이 Unity 직렬화의 기준이며 JSON을 별도의 Runtime 진실 원본으로 간주하면 안 됩니다. JSON Import 전에는 대상과 Diff를 확인하세요.

## Version과 Migration

`DesignerMetadataAsset.CurrentSchemaVersion`은 현재 1입니다. 0은 명시적 sibling index 이전 Asset이며 `DesignerHierarchyMigration`이 한 번 마이그레이션한 뒤 Version을 기록합니다. 기존 필드 기본값은 이전 Asset을 안전하게 읽도록 설계되어 있습니다.

새 직렬화 필드를 추가할 때는 기존 Asset의 기본값, Undo/Dirty 처리, JSON Serializer, Validator, 두 Backend Serializer와 Migration을 함께 검토합니다. Runtime 타입에 `UnityEditor` 참조를 추가하지 않습니다.

## Assembly 경계

Metadata 타입은 Designer Runtime Assembly에 있습니다. Window, AssetDatabase, Undo, Serializer와 Migration은 Editor Assembly에 있습니다. Core 책임은 실행 화면 계약이고, Designer 전용 Preview/편집 책임을 Core에 넣지 않습니다.

