# Motion Clip Editor

Motion Clip Editor는 `UIMotionClip` 에셋의 트랙과 키프레임을 편집하는 도구입니다. 클립은 Metadata 안에 복제되지 않고 독립 에셋으로 유지되며, 화면의 `DesignerMetadataAsset.screenMotion`에서 참조합니다.

## 저장 구조

- 화면 진입/종료: `screenMotion.entryClip`, `screenMotion.exitClip`
- 요소 트리거: `screenMotion.bindings`
- 모션 감소 설정: 각 Binding의 `reducedMotionClip`
- 상태 머신/그래프: 동일한 `screenMotion` 구성의 `stateMachine`, `motionGraph`
- 저장할 때 진입/종료 클립은 Core의 `UIScreenDefinition.motion`에도 동기화됩니다.

Element ID를 바꾸면 Motion target도 함께 바뀝니다. 요소를 삭제해 끊어진 target이 생기거나 Clip 참조가 사라지면 Validator가 Error로 보고합니다.

## 사용법

1. Designer에서 화면과 Metadata를 엽니다.
2. 요소를 선택하고 Motion Inspector에서 `모션 클립 에디터 열기`를 누릅니다.
3. Clip을 생성하거나 선택합니다.
4. 선택 요소로 Track을 추가하고 Property Track, Keyframe을 편집합니다.
5. Motion Inspector의 `트리거 연결`에서 Trigger, Clip, Reduced Motion Clip을 지정합니다.
6. 화면 단위 모션은 `화면 진입 클립`과 `화면 종료 클립`에 지정합니다.
7. Designer 저장 후 Unity를 다시 열어 연결이 유지되는지 확인합니다.

독립 실행 메뉴는 `Tools > NexUI > Designer > Advanced > Motion Clip Editor`입니다. Toolbar에서 Duration, FPS, Loop와 Work Area를 설정할 수 있습니다. Timeline을 클릭하거나 Playhead를 움직이면 Scrub하고, Keyframe의 Easing 메뉴에서 기본 Easing 또는 `AnimationCurve`를 편집합니다. Marker는 제작용 표시이며 Runtime 이벤트로 실행되지 않습니다.

## 구현된 편집 기능

- 다중 Keyframe 선택
- Shift 범위 선택, Ctrl/Command 토글 선택
- FPS Snap과 Auto Key
- Work Area와 Marker
- Reverse와 Timing Scale
- Copy/Paste와 다중 삭제
- Onion Skin과 Motion Path
- Play/Stop 및 Designer Preview 적용
- Unity Undo/Redo

## Runtime 재생

```csharp
using emiteat.NexUI.MotionClip;
using emiteat.NexUI.Core;

await NexUIApp.Manager.PlayMotionClipAsync("Inventory", clip);
```

Publish/Runtime에서 트리거를 자동 배선하는 범위는 Backend별 Adapter 구현 수준에 따릅니다. 저장 데이터와 직접 재생 API는 제공되지만, 모든 Trigger가 두 Backend에서 자동 실행되는 것은 아직 Partial입니다.

## 제한 사항

- `AnchoredPosition`과 `LocalPosition`은 현재 같은 transform capability로 평가됩니다.
- UI Toolkit은 LocalScale의 Z를 사용하지 않습니다.
- `UIMotionClipImporter`/`UIMotionClipExporter`는 Stub이며 실행 UI에서는 비활성 상태로 유지해야 합니다.
- Unity `AnimationClip`은 uGUI GameObject Preview만 지원하고 UI Toolkit 변환은 지원하지 않습니다.

[Motion 비교](overview.md) · [Motion Graph](motion-graph-editor.md)
