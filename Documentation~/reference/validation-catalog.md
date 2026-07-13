# Validation Catalog

Global Toolbar의 **Validate** 또는 `Tools > NexUI > Designer > Validate Current Screen`으로 검사합니다. 결과 행을 선택하면 가능한 경우 관련 Element나 Asset으로 이동합니다. Auto Fix는 실제 `DesignerAutoFixService`에 등록된 항목만 표시합니다.

## Screen과 Backend

| Code | 심각도 | 발생 조건 | 해결 방법 | Auto Fix |
| --- | --- | --- | --- | --- |
| `no-screen` | Info | Screen 미선택 | Global Toolbar에서 선택 | 없음 |
| `no-metadata` | Info | Metadata 미선택 | Metadata 생성/연결 | 없음 |
| `metadata-screen-mismatch` | Warning | Screen ID 불일치 | 두 Asset의 ID 확인 | 없음 |
| `empty-screen-id` | Error | Screen ID 비어 있음 | 유효한 ID 입력 | 없음 |
| `unsupported-backend` | Error | 알 수 없는 Backend | 지원 Backend 선택 | 없음 |
| `backend-asset-missing` | Warning | Backend Asset 없음 | Prefab/UXML 연결 | 없음 |
| `backend-type-mismatch` | Error | Backend와 Asset 형식 불일치 | 올바른 Asset 연결 | 없음 |

## Element와 Hierarchy

| Code | 심각도 | 발생 조건 | 해결 방법 | Auto Fix |
| --- | --- | --- | --- | --- |
| `empty-element-id` | Error | ID가 비어 있음 | 고유 ID 생성 | 있음 |
| `duplicate-element-id` | Error | 같은 ID가 둘 이상 | 중복 이름 변경 | 있음 |
| `invalid-element-id` | Warning | Backend에 부적합한 ID | 안전한 문자로 변경 | 없음 |
| `missing-backend-element` | Warning | Metadata Element가 Backend에 없음 | Backend 이름/동기화 확인 | 없음 |
| `zero-size-element` | Error | 너비/높이 0 | 기본 크기로 복원 | 있음 |
| `outside-canvas` | Warning | Canvas 밖 | Canvas 안으로 이동 | 있음 |
| `self-parent` / `circular-parent` | Error | 자기 자신/순환 부모 | 부모 연결 제거 | 있음 |
| `missing-parent` | Error | Parent ID 대상 없음 | 부모 수정 또는 제거 | 있음 |
| `leaf-with-children` | Warning | Leaf 아래 Child | Container로 이동 | 없음 |
| `excessive-depth` | Warning | Hierarchy가 너무 깊음 | 계층 단순화 | 없음 |
| `invalid-slot` | Error | Parent에 없는 Slot | 유효 Slot 선택 | 없음 |
| `template-slot-multiple` | Warning | 단일 Slot에 여러 Child | Slot 구성 수정 | 없음 |

## Binding, Command와 접근성

| Code | 심각도 | 발생 조건 | 해결 방법 | Auto Fix |
| --- | --- | --- | --- | --- |
| `button-without-command` | Warning | Button Command Key 없음 | Prototype에서 Key 설정 | 없음 |
| `button-without-text` | Info | Button Text 없음 | Label 또는 접근성 이름 확인 | 없음 |
| `small-touch-target` | Warning | 터치 영역이 작음 | 32×32 이상으로 확대 | 있음 |
| `hidden-but-interactive` | Info | 숨김 요소가 상호작용 가능 | Visibility/Interaction 검토 | 없음 |
| `unsupported-binding` | Info | 컴포넌트가 Channel 미지원 | 지원 Channel 사용 | 없음 |
| `localization-target-missing` | Warning | 번역 대상 Element 없음 | Target ID 수정 | 없음 |
| `localization-key-missing` | Info | Localization Key 없음 | Key 등록 | 없음 |

## Variant, Responsive와 Motion

| Code | 심각도 | 발생 조건 | 해결 방법 | Auto Fix |
| --- | --- | --- | --- | --- |
| `variant-target-missing` | Warning | Variant 대상 없음 | Target ID 수정 | 없음 |
| `responsive-target-missing` | Warning | Responsive 대상 없음 | Target ID 수정 | 없음 |
| `motion-close-missing` | Warning | Open은 있고 Close 없음 | Close Clip 연결 | 있음(역전 생성) |
| `motion-target-missing` | Error | Motion Target ID 없음 | Target 수정 | 없음 |
| `screen-motion-has-target` | Warning | Screen Motion에 Element Target | Target 비우기 | 없음 |
| `motion-clip-missing` | Error | Clip Reference 누락 | Clip 다시 연결 | 없음 |
| `motion-state-id-missing` | Error | State Trigger에 State ID 없음 | State ID 입력 | 없음 |
| `motion-command-id-missing` | Warning | Command Trigger에 Command ID 없음 | Command ID 입력 | 없음 |
| `motion-duplicate-track-target` | Error | Clip Track Target 중복 | Track 구성 수정 | 없음 |
| `motion-duplicate-keyframe-time` | Error | 같은 시간 Keyframe 중복 | 시간 조정 | 없음 |
| `motion-negative-keyframe` | Error | 음수 시간 | 0 이상으로 이동 | 없음 |
| `motion-keyframe-after-duration` | Error | Duration 뒤 Keyframe | Duration/시간 수정 | 없음 |
| `motion-keyframes-unsorted` | Error | 시간 정렬 안 됨 | Keyframe 정렬 | 없음 |
| `motion-start-keyframe-missing` | Warning | 0초 Keyframe 없음 | 시작 값 추가 | 없음 |
| `motion-end-keyframe-missing` | Warning | 끝 Keyframe 없음 | 종료 값 추가 | 없음 |

## uGUI와 동기화

| Code | 심각도 | 발생 조건 | 해결 방법 | Auto Fix |
| --- | --- | --- | --- | --- |
| `orphan-backend-element` | Info | Backend에만 이름 존재 | Metadata 동기화/정리 | 없음 |
| `duplicate-gameobject-name` | Warning | Prefab 이름 중복 | 이름 고유화 | 없음 |
| `ugui-decorative-raycast` | Warning | 장식 Graphic이 Raycast | Raycast Target 끄기 | 있음 |
| `ugui-invisible-canvasgroup-blocks-input` | Error | 투명 CanvasGroup이 입력 차단 | Blocks Raycasts 해제 | 있음 |
| `ugui-button-target-graphic-missing` | Warning | Button Target Graphic 없음 | Graphic 연결 | 있음 |
| `ugui-missing-button` | Warning | Button Element에 Button 없음 | Component 추가 | 없음 |
| `ugui-missing-text` | Warning | Text Element에 Text 없음 | TMP/Text 추가 | 없음 |
| `ugui-missing-graphic` | Warning | 시각 Element에 Graphic 없음 | Graphic 추가 | 없음 |
| `ugui-modal-without-canvasgroup` | Info | Modal에 CanvasGroup 없음 | 필요 시 추가 | 없음 |

Motion Graph, Motion State Machine, Scenario와 Generated Asset의 일부 검사는 별도 편집기/Publish 결과 메시지로 제공되며 위 Core Validation Code 목록에 Stable Code로 등록되지 않은 항목도 있습니다. 문서가 임의 Code를 만들지 않도록 사용자에게 보이는 메시지를 기준으로 확인하세요.
