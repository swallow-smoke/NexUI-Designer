# Motion Clip Editor

## 이것은 무엇인가

**Motion Clip Editor**는 NexUI Designer 안에서 선택한 UI Element에 대해 Position / Rotation /
Scale / Size / Alpha를 타임라인 기반 키프레임으로 편집하는 도구입니다.

기존의 `UIMotionPreset`/`UIMotionGraph`(Motion Graph Editor)는 from→to→duration 형태의
"스텝" 기반이며 단일 대상만 다룹니다. Motion Clip Editor는 그와 별개의, 병행하는 시스템으로
동작합니다:

- 여러 UI Element를 하나의 클립(`UIMotionClip`)에 담을 수 있습니다 (Element마다 하나의
  `UIMotionClipTrack`).
- 각 Element는 여러 Property(`UIMotionClipPropertyTrack`)를 가질 수 있습니다.
- 각 Property는 여러 개의 키프레임(`UIMotionClipKeyframe`)을 가지며, Easing 또는
  `AnimationCurve`로 보간됩니다.
- 씬을 스크러빙(scrub)하며 실시간으로 미리볼 수 있습니다.

기존 Motion Graph 시스템은 이 작업으로 전혀 변경되지 않았습니다. 두 시스템은 서로 다른
용도(스텝형 프리셋 vs. 조밀한 키프레임 타임라인)로 계속 공존합니다.

## Motion Clip 생성 방법

1. `Tools/NexUI/Designer/Motion Clip Editor`를 실행합니다.
   - 또는 NexUI Designer에서 요소를 선택한 뒤 Motion 인스펙터의 **"Open Motion Clip Editor"**
     버튼을 누르면 현재 선택된 요소와 프리뷰 서페이스가 자동으로 연결된 채로 열립니다.
2. 툴바에서 **"Create Motion Clip"**을 누르면 `Assets/`에 `UIMotionClip` 에셋이 생성됩니다.
   (`Create > NexUI > Motion Clip`으로 직접 만들 수도 있습니다.)
3. Duration과 Loop 여부를 설정합니다.

## Track / Keyframe 추가 방법

1. NexUI Designer에서 애니메이션을 줄 요소를 선택합니다.
2. Motion Clip Editor에서 **"Add Track From Selection"**을 누릅니다 — 선택된 요소를 대상으로
   하는 Track이 추가됩니다.
3. Track 아래에서 애니메이션할 Property(AnchoredPosition / LocalPosition / LocalRotationZ /
   LocalScale / SizeDelta / CanvasGroupAlpha)를 고르고 **"Add Property Track"**을 누릅니다.
4. 상단 Time 슬라이더로 원하는 시간으로 이동한 뒤 **"Add Keyframe At Time"**을 누릅니다.
5. 생성된 키프레임 행에서 Time / Value / Easing을 직접 수정하거나, 타임라인 위의 점을
   드래그해 시간을 조정합니다. **Delete**로 키프레임을 삭제합니다.

## Preview 방법

- 상단 Time 슬라이더를 움직이면 NexUI Designer의 현재 프리뷰 서페이스에 즉시 값이
  적용됩니다 (Designer가 열려 있고 화면이 로드되어 있어야 합니다).
- **Play**를 누르면 Duration 동안 재생되며(Loop 설정 시 반복), **Stop**으로 멈춥니다.

## Runtime에서 재생하는 방법

```csharp
using emiteat.NexUI.Core;
using emiteat.NexUI.MotionClip;

// UIManager 확장 메서드 (emiteat.NexUI.MotionClip.UIManagerMotionClipExtensions)
await uiManager.PlayMotionClipAsync("Inventory", myClip);

// 또는 직접 IUISurface + IUIMotionClipPlayer를 사용:
var surface = uiManager.GetSurface("Inventory");
var player = new UIMotionClipPlayer();
await player.PlayAsync(surface, myClip);
```

`IUIMotionClipPlayer`는 Editor 의존성이 없는 순수 Runtime 코드입니다 (`Runtime/MotionClip/`,
`UniTask` 기반, 코드베이스 전반의 async 관례와 동일). `PlayMotionClipAsync`는 기존
`UIManager`를 수정하지 않고 확장 메서드로 추가되었습니다 — 기존 `OpenAsync`/`CloseAsync`/
Motion 시스템은 전혀 영향받지 않습니다.

## Unity AnimationClip 연동 상태

- **Preview/Play (구현됨)**: `UnityAnimationClipAdapter.Evaluate(clip, gameObject, time)`로
  기존 Unity `AnimationClip`을 `AnimationClip.SampleAnimation`을 통해 그대로 재생/프리뷰할 수
  있습니다. UGUI(GameObject) 대상만 지원하며, UI Toolkit `VisualElement`에는 Animator 개념이
  없어 적용되지 않습니다.
- **Export (`UIMotionClipExporter`) / Import (`UIMotionClipImporter`) — 미구현**: 인터페이스와
  스텁만 존재하며 호출 시 `NotImplementedException`을 던집니다. 아래 "향후 계획" 참고.

## 현재 제한사항

- `AnchoredPosition`과 `LocalPosition`은 현재 동일한 값으로 취급됩니다. 런타임
  `IUITransformCapability`가 두 개념을 구분하지 않기 때문입니다 (확장 시 더 넓은 범위의 변경이
  필요해 이번 작업 범위에서는 보류했습니다).
- `LocalScale`의 Z축은 UI Toolkit 백엔드에서 무시됩니다 (2D 렌더링 특성상 X/Y만 반영).
- 타임라인 UI는 드래그로 시간을 옮기는 정도의 최소 기능만 제공합니다 — 곡선 핸들 편집, 다중
  선택, 스냅 등은 아직 없습니다.
- Motion Clip은 아직 `DesignerElementMetadata`/화면 저장 스키마에 연결되어 있지 않습니다 —
  클립 에셋은 독립적으로 저장/할당되며, Designer의 Save에 자동으로 포함되지 않습니다.
- `UIMotionClipImporter`/`UIMotionClipExporter`는 스텁입니다.

## 향후 계획

- AnimationClip ↔ UIMotionClip 양방향 변환 (Import/Export) 구현.
- `DesignerElementMetadata`/`UIScreenMotionConfig`에 Motion Clip 참조를 연결해 Designer 저장
  플로우에 완전히 포함시키기.
- AnchoredPosition/LocalPosition을 구분하는 Transform Capability 확장.
- 타임라인 UI 고도화 (다중 선택, 곡선 편집, 스냅, 트랙 재정렬).
