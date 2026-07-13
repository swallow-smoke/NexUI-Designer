# Binding

Binding은 Element 표시 값과 게임 상태의 Key를 연결합니다. Designer는 Key를 저장하고 Preview에서 일부 값을 시뮬레이션하지만, 실제 데이터 공급과 Command 실행은 NexUI Runtime 책임입니다.

| 채널 | Metadata 필드 | 용도 |
|---|---|---|
| Text | `textKey` | Label/Button 문자열 |
| Value | `valueKey` | Progress, 수량 등 값 |
| Visibility | `visibilityKey` | 표시 여부 |
| Class | `classKey` | 상태 Class 표현 |
| Interactable | `interactableKey` | 입력 가능 여부 |
| Command | `commandKey` | 클릭 시 Runtime Action Key |

Binding Picker는 프로젝트에서 발견한 `IBindableProperty<T>` 후보를, Command Picker는 기본 Key와 다른 Metadata에서 사용 중인 Key를 보여 줍니다. Image 전용 Object Binding과 일반 Collection Binding 계약은 현재 Inspector에서 독립 채널로 완성되어 있지 않습니다.

존재하지 않는 Key는 Designer가 게임 코드 전체를 확정적으로 해석할 수 없어 일부는 Warning으로만 보고됩니다. 개발자는 Runtime `UIStateStore`, `UIActionResolver` 등록과 타입 호환성을 함께 확인해야 합니다.

