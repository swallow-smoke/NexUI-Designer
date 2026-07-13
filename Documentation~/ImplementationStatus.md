# 구현 상태

이 문서는 외부 링크 호환을 위한 요약입니다. 상세 상태와 제한은 [현재 기능 상태](reference/feature-status.md)와 [알려진 제한](reference/known-limitations.md)을 기준으로 합니다.

| 기능 | 상태 | uGUI | UI Toolkit | 테스트 | 비고 |
|---|---|---:|---:|---:|---|
| Canvas Multi Selection / Key Object | Complete | Common | Common | Yes | Undo 지원 |
| Session Registry / View Lifecycle | Complete | Common | Common | Yes | Provider 교체 가능 |
| Motion Clip Save Binding | Complete | Yes | Yes | Yes | Reduced Motion 포함 |
| Motion Trigger Runtime Binder | Complete | Yes | Yes | Yes | Capability 자동 구독, Lifecycle Notify |
| AnimationClip Import/Export | Complete | Yes | Common Asset | Yes | 지원 Curve만 변환 |
| Auto Layout Row/Column/Grid | Complete | Yes | Yes | Yes | LayoutGroup / Flex Wrap |
| 복합 Constraints 변환 | Partial | Partial | Partial | Partial | Preview 중심 |
| Generated UXML/USS Save | Complete | N/A | Yes | Yes | Marker 없는 사용자 파일 보존 |
| Scenario Static/Timeline | Complete | Common | Common | Yes | Bool/Number/Text/Sprite/List |
| Figma First Frame Import | Beta | Common | Common | Yes | Variant/Image Download/Sync 제외 |
| Legacy Motion Graph | Partial | Common | Common | Partial | 단일 Preset 의존 그래프 |
| Motion Graph v2 | Beta | Common | Common | Partial | Event/Flow Preview |
| Motion State Machine | Beta | Common | Common | Partial | Transition Clip Preview |
| Component Registry Backend 변환 | Partial | Partial | Partial | Yes | Component별 지원 등급 확인 |

상태 값은 `Complete`, `Beta`, `Partial`, `Stub`, `Planned`, `Unsupported`를 사용합니다. 구현되지 않은 장기 목표는 [목표 기능 명세](reference/feature-specification.md)에만 기록하며 현재 지원으로 간주하지 않습니다.
