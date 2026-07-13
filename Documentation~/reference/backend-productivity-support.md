# Backend별 생산성 기능 지원

| 기능 | uGUI | UI Toolkit | 비고 |
|---|---:|---:|---|
| 화면 생성 마법사 | 지원 | 지원 | Prefab / UXML 생성 |
| 전환 프리셋 | 지원 | 지원 | 공통 Motion Clip 모델 |
| Preview Scenario | 지원 | 지원 | Designer Preview 데이터 |
| Auto Layout 변환 | 지원 | 지원 | Layout Group / Flex 변환 |
| Grid 자동 변환 | 지원 | 지원 | GridLayoutGroup / Flex Wrap |
| Anchor 추천 | 지원 | 지원 | 공통 Metadata |
| Metadata Validation/Fix | 지원 | 지원 | 공통 규칙 |
| Graphic/CanvasGroup Fix | 지원 | 해당 없음 | uGUI Prefab 전용 |
| Generated UXML 구조 저장 | 해당 없음 | 지원 | Marker 있는 UXML/USS만 트랜잭션 갱신 |

지원하지 않는 Backend 작업은 숨기지 않고 비활성화하거나 Validation 안내로 이유를 표시하는 것을 원칙으로 합니다.
