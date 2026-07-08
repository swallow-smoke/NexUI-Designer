# 핵심 개념

## 런타임 패키지와 디자이너 패키지

`com.emiteat.nexui`는 런타임 동작을 담당합니다. `com.emiteat.nexui.designer`는 Unity Editor 도구를 담당합니다.

게임플레이와 런타임 UI 코드는 런타임 패키지에 둡니다. 제작 보조 도구, 에디터 패널, 검증, 프리뷰, 메타데이터 편집은 디자이너 패키지에 둡니다.

## Screen Definition

`UIScreenDefinition`은 런타임에서 사용하는 화면 에셋입니다. 디자이너는 이 에셋을 프리뷰, 검증, 저장 작업의 루트 객체로 사용합니다.

## Designer Metadata

`DesignerMetadataAsset`은 에디터에서 작성한 제작용 데이터를 저장합니다.

자주 저장하는 메타데이터:

- 안정적인 element id
- binding 이름
- style class
- localization key
- responsive rule
- variant
- validation expectation
- 프리뷰 전용 메모

메타데이터는 일반 Unity 에셋으로 직렬화될 수 있도록 런타임 안전 어셈블리에 둡니다. 이 데이터를 편집하거나 검증하는 에디터 동작은 에디터 어셈블리에 둡니다.

## Backend

디자이너는 `INexUIDesignerBackend`를 통해 UI 구현과 통신합니다.

백엔드가 담당하는 일:

- 특정 화면을 처리할 수 있는지 판단
- 프리뷰 콘텐츠 생성
- 계층 데이터 노출
- 선택 항목을 메타데이터와 연결
- serializer와 협력해 저장

이 구조 덕분에 UI Toolkit과 uGUI가 같은 디자이너 창을 공유하면서도 서로 다른 렌더링 모델을 유지할 수 있습니다.

## Validation

검증은 정적 제작 오류를 찾는 단계입니다. Play Mode에 들어가기 전에 누락된 화면 참조, 깨진 element id, 잘못된 localization link, 누락된 contract, 잘못된 responsive rule, 삭제된 객체를 가리키는 메타데이터를 잡는 것이 목표입니다.

## Localization

디자이너 UI 문자열과 게임 UI 로컬라이제이션은 분리됩니다. 디자이너 로컬라이제이션은 에디터 라벨과 메뉴 텍스트를 제어합니다. 게임 로컬라이제이션 메타데이터는 화면 요소와 런타임 localization key를 연결합니다.

## Preview

뷰포트는 에디터 프리뷰 표면입니다. 구조, 상태, 계층, 제작 규칙을 확인하는 데 유용하지만 최종 애니메이션 타이밍, 입력, 플랫폼별 동작은 Play Mode에서 다시 확인해야 합니다.
