# 알려진 제한

| 영역 | 증상/원인 | 현재 우회 방법 |
|---|---|---|
| 공통 | 일부 고급 기능은 Metadata/Preview까지만 지원합니다. | Save Report와 Play Mode를 확인합니다. |
| Canvas | Align 기준은 선택 Bounding Box이며 Key Object가 아닙니다. | 기준 Element를 별도로 정렬합니다. |
| Layout | Auto Layout/Constraints Backend 변환이 완전하지 않습니다. | uGUI Layout Group 또는 USS를 보완합니다. |
| Binding | 프로젝트 Runtime Key 존재 여부를 모두 확정할 수 없습니다. | `UIStateStore`와 `UIActionResolver` 등록을 테스트합니다. |
| uGUI | Custom Component와 복잡한 Animator/TMP를 자동 소유하지 않습니다. | Prefab에서 수동 구성하되 Save 덮어쓰기 범위를 확인합니다. |
| UI Toolkit | 일반 Save가 UXML을 다시 쓰지 않습니다. | UI Builder 또는 별도 `.g.uxml/.g.uss` Generation을 사용합니다. |
| Motion Clip | Position capability 일부가 같은 Runtime 값으로 처리됩니다. | 대상 Backend에서 실제 재생을 확인합니다. |
| Motion Graph | Legacy와 v2가 서로 다른 모델입니다. | 에셋 Type과 실행기를 혼용하지 않습니다. |
| Figma | Import/변환/Sync가 없습니다. | JSON 조회를 접근 확인 용도로만 사용합니다. |
| 저장 | 읽기 전용 Package 경로에는 생성 파일을 쓸 수 없습니다. | Import된 Sample 또는 `Assets/` 경로를 사용합니다. |
| Preview | 실제 입력·폰트·해상도 결과와 다를 수 있습니다. | Play Mode와 Player Build로 재검증합니다. |

