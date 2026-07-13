# 애니메이션 Popup 튜토리얼

## 완성 결과

Dim Background, Modal Panel, 제목, 본문과 Confirm/Close Button을 만들고 Open/Close Motion을 연결합니다. Sample의 `Screens/ConfirmDialog/`를 구조 참고용으로 사용할 수 있습니다.

## 최종 Hierarchy

```text
PopupRoot
├─ Dim
└─ Popup
   ├─ Header/Title
   ├─ Content/Message
   └─ Footer/ConfirmButton, CloseButton
```

## 단계별 제작

1. 전체 화면 Panel `Dim`과 그 위 Modal `Popup`을 추가합니다.
2. Modal Template이 제공하는 경우 Header/Content/Footer Slot에 자식을 배치합니다. `invalid-slot` Validation이 나오면 Parent가 허용하는 Slot 이름을 다시 선택합니다.
3. Confirm과 Close Button의 Command Key를 Runtime Registry와 같은 값으로 입력합니다.
4. Motion Clip Editor에서 Open Clip을 만들고 Popup의 `CanvasGroupAlpha` 0→1, `LocalScale` 0.96→1 Keyframe을 둡니다.
5. Close Clip은 별도로 만들고 값을 반대로 둡니다. Screen Entry/Exit 또는 Popup Trigger Binding에 각각 연결합니다.
6. Reduced Motion Clip에는 큰 이동과 반동을 빼고 짧은 `CanvasGroupAlpha` 변화만 둡니다.

## Preview와 Scenario

Motion Editor에서 0초과 끝을 Scrub합니다. 긴 Message, 빈 Message, Confirm Disabled 같은 상태는 지원되는 Binding/Scenario 값으로 확인합니다. Scenario Apply는 Popup을 실제 Runtime Stack에 Push하지 않습니다.

## Validation, Save와 Play Mode

Missing Clip, Missing Target, Screen Motion Target과 Modal Backend 구성을 Validate합니다. Save 후 Metadata를 다시 열어 Asset Reference가 유지되는지 확인합니다. Play Mode에서는 Open/Close Lifecycle이 `Enter`, `Exit` 또는 필요한 Trigger 통지를 호출하는지 확인합니다.

## Backend별 차이와 제한

uGUI는 CanvasGroup/Graphic 구성이 Opacity와 입력 차단에 영향을 줍니다. UI Toolkit은 수동 UXML을 일반 Save로 재작성하지 않습니다. Entry/Exit 참조가 저장된다는 사실만으로 프로젝트의 Screen Stack에 자동 배선되었다고 가정하지 마세요. Runtime 연결이 없다면 프로젝트 코드에서 Trigger/Lifecycle을 연결해야 합니다.

## 확장 과제

CommandFailed용 오류 메시지, 키보드 Focus 순서와 Reduced Motion을 두 Backend에서 확인해 보세요.
