# Motion 개요

NexUI에는 목적이 다른 Motion 모델이 공존합니다. 새 작업에서는 필요한 편집 방식과 Runtime 연결 범위를 먼저 선택해 주세요.

| 비교 | Motion Clip | Legacy Motion Graph | Motion Graph (v2) |
|---|---|---|---|
| 데이터 | `UIMotionClip` | `UIMotionPreset`의 `UIMotionGraph`/`UIMotionStep` | `UIMotionGraphAsset`/typed node |
| 편집 | Timeline/Keyframe | Dependency Node Graph | Event/Flow Node Graph |
| 대상 수 | 여러 Element Track | 기본적으로 단일 대상 Preset | Node 설정에 따름 |
| Property 수 | Track마다 여러 Property | Step마다 Property | Node type별 입력 |
| Timeline | 지원 | 컴파일 결과 Preview | 없음 |
| Node Graph | 없음 | 지원 | 지원 |
| Preview | Designer Surface에 Scrub/Play | 컴파일 Timeline 값 확인 | Designer Surface에서 Entry Event 실행 |
| 적합한 용도 | 화면 등장처럼 여러 Element가 시간 순서대로 움직이는 연출 | 단일 Element From/To 단계 | Click/Screen event 기반 흐름 실험 |
| 상태 | 지원, Runtime trigger 자동 배선은 부분 지원 | 부분 지원 | 실험적 |

Button Hover처럼 한 대상의 단순 From/To 변화는 Legacy Motion Preset이 이해하기 쉽습니다. 여러 Element가 순서대로 등장하는 화면은 Motion Clip이 적합합니다. Event에서 분기되는 실행 흐름을 시험하려면 Motion Graph (v2)를 사용할 수 있지만 Production 전 Runtime 결과를 별도로 검증해야 합니다.

Motion State Machine은 위 모델과 별개로 상태 전환과 Transition Clip을 저장합니다.

