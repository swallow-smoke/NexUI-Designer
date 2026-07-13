# Backend 지원 범위

이 표는 Designer에서 보이는 값이 어디까지 저장되는지 구분합니다. **일반 Save**와 UI Toolkit의 **Generate/Publish**는 서로 다른 작업입니다.

| 기능 | Designer Preview | Metadata | uGUI Save | UI Toolkit 일반 Save | Generated UXML/USS | Runtime 확인 필요 |
| --- | --- | --- | --- | --- | --- | --- |
| Position / Size | 지원 | 지원 | 지원 | Metadata만 | 지원 | Auto Layout 충돌 |
| Parent / Layer | 지원 | 지원 | 지원 | Metadata만 | 지원 | Template/Slot |
| Text / Tint | 지원 | 지원 | 지원 | Metadata만 | 지원 | Font/Style 우선순위 |
| Image | 부분 지원 | Preview 참조 | 부분 지원 | Metadata만 | 부분 지원 | Sprite/Texture 연결 |
| Shape | Preview만 | 지원 | 미지원 | Metadata만 | 미지원 | 직접 제작 필요 |
| Progress | 지원 | 지원 | 부분 지원 | Metadata만 | 부분 지원 | Track/Label/애니메이션 |
| Button | 지원 | 지원 | 지원 | Metadata만 | 지원 | Event 연결 |
| Binding / Command Key | 지원 | 지원 | Metadata만 | Metadata만 | Metadata만 | Runtime Registry 필수 |
| Auto Layout | 지원 | 지원 | 부분 지원 | Metadata만 | 부분 지원 | Backend별 계산 차이 |
| Constraints | 지원 | 지원 | 부분 지원 | Metadata만 | 부분 지원 | 해상도별 확인 |
| Theme | 부분 지원 | 지원 | Metadata만 | Metadata만 | 부분 지원 | Runtime Theme 연결 |
| Motion | 지원 | Asset 참조 | Metadata만 | Metadata만 | Metadata만 | Trigger/Lifecycle 연결 |
| Accessibility | 부분 지원 | 지원 | 부분 지원 | Metadata만 | 부분 지원 | Player/보조 기술 확인 |
| Variant / Responsive | 부분 지원 | 지원 | 부분 지원 | Metadata만 | 부분 지원 | 실제 해상도 확인 |
| Scenario | 지원 | 별도 Asset | 해당 없음 | 해당 없음 | 해당 없음 | Preview 전용 |
| Preview State | 지원 | Preview만 | 해당 없음 | 해당 없음 | 해당 없음 | Runtime 상태와 별개 |

## uGUI Save

Backend Asset은 Prefab이어야 합니다. Serializer는 Element 이름으로 대상을 찾고 Rect, 계층, 일부 Graphic/Text/Button/Auto Layout을 반영합니다. 지원하지 않는 컴포넌트나 Preview 전용 값은 Save Report의 **Skipped** 또는 Warning으로 남습니다. Prefab에 같은 이름이 중복되면 이름 기반 매칭이 불안정합니다.

## UI Toolkit 일반 Save

일반 Save는 Metadata와 Screen Definition을 저장하지만, 사용자가 UI Builder에서 만든 UXML을 임의로 다시 쓰지 않습니다. UXML의 named `VisualElement`와 Metadata Element ID 불일치를 검사합니다. 즉, 일반 Save만 누르고 수동 UXML 구조가 바뀔 것으로 기대하면 안 됩니다.

## Generated UXML/USS

Generate/Publish는 Metadata에서 `.g.uxml`과 `.g.uss`를 만듭니다. Generated Marker가 없는 사용자 파일은 덮어쓰지 않습니다. 생성 결과가 같으면 파일을 다시 쓰지 않습니다. 지원되지 않거나 부분 지원인 값은 Diff와 결과 메시지를 확인하세요.

## Runtime 확인이 필요한 이유

Designer는 Key와 에셋 참조를 저장합니다. 실제 데이터, Command, Screen Lifecycle, Theme, 접근성 동작은 Runtime 코드와 Backend Capability가 공급합니다. 따라서 두 Backend 모두 Play Mode 검증이 최종 단계입니다.

- [uGUI Backend](../user-guide/ugui-backend.md)
- [UI Toolkit Backend](../user-guide/ui-toolkit-backend.md)
- [Asset Ownership](asset-ownership.md)

