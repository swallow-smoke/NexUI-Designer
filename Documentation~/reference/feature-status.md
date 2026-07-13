# 현재 기능 상태

이 문서는 현재 코드 기준입니다. **지원**은 일반 사용 가능, **부분 지원**은 Backend/저장 범위 제한, **실험적**은 검증이 더 필요한 도구, **진행 중**은 작업 경로 일부만 존재, **미지원**은 사용할 수 없음을 뜻합니다.

| 기능 | 상태 | uGUI | UI Toolkit | 비고 |
|---|---|---|---|---|
| 단일·다중·박스 선택 | 지원 | 지원 | 지원 | Metadata Canvas 공통 |
| 이동·크기·정렬·Group | 지원 | 지원 | 지원 | Undo 지원 |
| Auto Layout/Constraints | 부분 지원 | 부분 지원 | 부분 지원 | Metadata/Preview 중심 |
| Component Registry | 지원 | 부분 지원 | 부분 지원 | Component별 Backend support 확인 |
| Text/Value/Visibility/Class/Command Binding | 부분 지원 | 지원 | 지원 | Runtime Key 등록 필요 |
| Designer Preview/Interactive Log | 부분 지원 | 지원 | 지원 | 실제 Command는 실행하지 않음 |
| Scenario Apply/Timeline | 실험적 | 공통 | 공통 | Preview Metadata 변경 |
| Validation/Save Report | 지원 | 지원 | 지원 | 일부 프로젝트 계약은 Warning |
| uGUI Prefab 저장 | 부분 지원 | 부분 지원 | 해당 없음 | 기본 Rect/Image/Text/Button/Fill |
| UI Toolkit 일반 Save | 부분 지원 | 해당 없음 | 부분 지원 | Metadata 저장·UXML 이름 검증 |
| `.g.uxml/.g.uss` 생성 | 지원 | 해당 없음 | 지원 | 별도 Generation 도구 |
| Motion Clip 편집/직접 재생 | 지원 | 지원 | 지원 | 일부 Property 차이 |
| Motion Trigger 자동 Runtime 연결 | 부분 지원 | 부분 지원 | 부분 지원 | 데이터 저장은 지원 |
| Legacy Motion Graph | 부분 지원 | 공통 | 공통 | 단일 Preset Dependency Graph |
| Motion Graph (v2) | 실험적 | 공통 | 공통 | Event/Flow 실행 Preview |
| Motion State Machine | 실험적 | 공통 | 공통 | Transition Clip Preview |
| AnimationClip Import/Export | 미지원 | 미지원 | 미지원 | `NotImplementedException` Stub |
| Figma 인증/JSON 조회 | 실험적 | 공통 | 공통 | 변환/Import/Sync 없음 |
| Migration Scan/Apply | 부분 지원 | 공통 | 공통 | `.bak`, 직접 메뉴 없음 |
| Runtime Snapshot/Overlay | 부분 지원 | 공통 | 공통 | Core 구현, 직접 메뉴 없음 |
| Screen Flow/Design Token | 실험적 | 공통 | 공통 | 고급 도구, 수동 검증 필요 |

장기 목표는 [목표 기능 명세](feature-specification.md), 구체적인 제약은 [알려진 제한](known-limitations.md)을 확인해 주세요.

