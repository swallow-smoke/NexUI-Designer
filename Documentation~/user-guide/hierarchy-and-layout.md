# Hierarchy와 Layout

Hierarchy는 중첩 객체가 아니라 Metadata의 평면 Element 목록과 `parentId`, `siblingIndex`로 저장됩니다. Layers에서 드래그하거나 Reparent 명령을 사용하면 이 참조가 바뀝니다.

- **Reparent:** 부모를 바꾸되 Canvas 위치를 유지합니다.
- **Layer order:** 같은 부모의 `siblingIndex`를 변경합니다.
- **Group/Ungroup:** Container를 만들거나 해제하고 자식 관계를 갱신합니다.
- **Align/Distribute:** 현재 선택의 Bounding Box를 기준으로 정렬·분배합니다. Figma식 Key Object 기준 정렬은 아닙니다.
- **Auto Layout:** Container 방향, 간격, Padding과 자식 크기 모드를 Metadata에 저장하고 Preview Layout에 적용합니다.
- **Constraints:** 부모 크기 변경 시 가로·세로 Start/Center/End/Scale 관계를 기록합니다.

> [!NOTE]
> Auto Layout과 Constraints는 Metadata 및 Preview 중심의 부분 지원 기능입니다. Backend가 표현하지 못한 값은 Save Report 또는 Validation에서 확인해야 합니다.

