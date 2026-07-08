# 개발 가이드

## 어셈블리 경계

런타임 안전 메타데이터는 아래에 둡니다.

```text
Runtime/Metadata/
```

에디터 전용 코드는 아래에 둡니다.

```text
Editor/
```

런타임 어셈블리에서 `UnityEditor`를 참조하지 마세요.

## 네임스페이스

네임스페이스 루트는 `emiteat`을 사용합니다.

```csharp
namespace emiteat.NexUI.Designer.Editor
namespace emiteat.NexUI.Designer.Runtime.Metadata
```

## 패널 추가하기

1. `Editor/Panels` 또는 기능별 에디터 폴더에 패널을 만듭니다.
2. UI Toolkit의 `VisualElement` 계열 타입으로 UI를 구성합니다.
3. 중요한 노드에는 USS class를 붙입니다.
4. `Editor/Styles/NexUIDesigner.uss`에 스타일을 추가합니다.
5. `NexUIDesignerWindow`에서 패널을 연결합니다.

패널은 표시 역할에 집중시키세요. 분석, 변경, 검증 로직은 서비스로 옮기는 것이 좋습니다.

## 백엔드 추가하기

1. `INexUIDesignerBackend`를 구현합니다.
2. 백엔드에 맞는 저장 동작이 필요하면 serializer를 추가합니다.
3. 디자이너 context에서 백엔드를 선택하거나 등록합니다.
4. 백엔드 전용 실패 상황을 검증에 추가합니다.

백엔드는 디자이너 모델과 실제 UI 시스템 사이를 번역해야 합니다. 전역 창 레이아웃을 소유하지 않는 것이 좋습니다.

## 메타데이터 추가하기

1. `Runtime/Metadata` 아래에 직렬화 가능한 필드를 추가합니다.
2. Unity가 직렬화할 수 있는 형태를 유지합니다.
3. 에디터 전용 타입을 넣지 않습니다.
4. `Editor/` 쪽에 인스펙터, 검증, cleanup 동작을 추가합니다.

메타데이터는 제작 의도를 설명하는 데이터여야 합니다. 런타임 실행 로직은 런타임 패키지에 둡니다.

## 검증 추가하기

검증 메시지는 바로 행동할 수 있어야 합니다.

- 무엇이 잘못됐는지
- 어떤 화면 또는 요소가 영향을 받는지
- 어떻게 고치면 되는지

`invalid data`처럼 모호한 메시지는 피하세요. `Element 'submitButton' has a localization key but no text component.`처럼 원인과 대상을 같이 적는 편이 좋습니다.

## 스타일 가이드

- 기존 UI Toolkit 스타일을 따릅니다.
- 반복되는 시각 패턴은 inline style보다 USS class로 관리합니다.
- 에디터 텍스트는 짧고 명확하게 씁니다.
- reflection에 크게 의존하기보다 안정적인 id와 명시적인 라벨을 선호합니다.

## 테스트 체크리스트

디자이너 변경을 마치기 전에 확인합니다.

- `Tools/NexUI/Designer` 열기
- 화면 선택
- 프리뷰 rebuild
- validation 실행
- save 실행
- 언어 전환
- Unity Console 컴파일 오류 확인
