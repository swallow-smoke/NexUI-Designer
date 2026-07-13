# 알려진 제한

| 영역 | 증상/원인 | 현재 우회 방법 |
|---|---|---|
| 공통 | 일부 고급 기능은 Metadata/Preview까지만 지원합니다. | Save Report와 Play Mode를 확인합니다. |
| Canvas | Key Object를 지정하지 않은 다중 정렬은 선택 Bounding Box를 기준으로 합니다. | Layers/Canvas 메뉴에서 기준 Element를 Key Object로 지정합니다. |
| Layout | Auto Layout은 Row/Column/Grid를 저장하지만 복잡한 Constraints 조합은 Preview 중심입니다. | Backend별 Save Report와 실제 Layout을 확인합니다. |
| Binding | 프로젝트 Runtime Key 존재 여부를 모두 확정할 수 없습니다. | `UIStateStore`와 `UIActionResolver` 등록을 테스트합니다. |
| uGUI | Custom Component와 복잡한 Animator/TMP를 자동 소유하지 않습니다. | Prefab에서 수동 구성하되 Save 덮어쓰기 범위를 확인합니다. |
| UI Toolkit | Generated Marker 없는 사용자 UXML/USS는 일반 Save가 다시 쓰지 않습니다. | UI Builder로 관리하거나 별도 `.g.uxml/.g.uss` Generation을 사용합니다. |
| Motion Clip | Position capability 일부가 같은 Runtime 값으로 처리됩니다. | 대상 Backend에서 실제 재생을 확인합니다. |
| Motion Graph | Legacy와 v2가 서로 다른 모델입니다. | 에셋 Type과 실행기를 혼용하지 않습니다. |
| Figma | 첫 Frame Import는 지원하지만 Component Variant, Effect, Image 다운로드와 양방향 Sync는 지원하지 않습니다. | 가져온 Metadata를 Validation하고 복잡한 Style은 Backend에서 보완합니다. |
| 저장 | 읽기 전용 Package 경로에는 생성 파일을 쓸 수 없습니다. | Import된 Sample 또는 `Assets/` 경로를 사용합니다. |
| Preview | 실제 입력·폰트·해상도 결과와 다를 수 있습니다. | Play Mode와 Player Build로 재검증합니다. |
# 생산성 기능 제한사항

- Constraints의 복합 관계와 비선형 배치는 Backend별 수동 검토가 필요합니다.
- UI Toolkit 사용자 UXML은 Auto Fix가 직접 수정하지 않습니다.
- Command 생성과 Motion Track 삭제는 자동 수정하지 않습니다.
- 전환 미리보기는 실제 Runtime Command와 화면 Stack을 실행하지 않습니다.
