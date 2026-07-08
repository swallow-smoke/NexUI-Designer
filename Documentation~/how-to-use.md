# 빠른 시작

## 디자이너 열기

`Tools/NexUI/Designer`를 실행합니다.

창은 툴바, 계층, 뷰포트, 인스펙터, 검증 패널, 상태 패널, 커맨드 패널, 화면 그래프 패널로 구성됩니다.

## 화면 선택하기

1. `UIScreenDefinition`을 만들거나 선택합니다.
2. 툴바의 `Screen` 필드에 할당합니다.
3. `Rebuild`를 누릅니다.

뷰포트에는 현재 프리뷰 셸이 표시됩니다. 계층과 인스펙터는 선택한 화면과 디자이너 메타데이터를 기준으로 갱신됩니다.

## 메타데이터 만들기

디자이너 전용 데이터는 `DesignerMetadataAsset`에 저장합니다.

아래 메뉴에서 만들 수 있습니다.

```text
Create > NexUI > Designer > Metadata
```

메타데이터에는 런타임 UI 로직이 아니라 제작을 돕기 위한 정보를 넣습니다.

- element id
- style class
- binding key
- validation hint
- localization link
- preview row
- screen variant

## 화면 검증하기

툴바의 `Validate`를 누르거나 아래 메뉴를 실행합니다.

```text
Tools/NexUI/Designer/Validate Current Screen
```

검증은 누락된 선택, 지원되지 않는 백엔드 상태, 메타데이터 문제, 로컬라이제이션 링크, 반응형 규칙, 계약 문제 같은 정적 제작 오류를 보여줍니다.

## 저장하기

`Save`를 누르거나 아래 메뉴를 실행합니다.

```text
Tools/NexUI/Designer/Save Current Screen
```

디자이너는 저장 작업을 현재 백엔드의 serializer에 위임합니다. UI Toolkit과 uGUI는 같은 에디터 창을 공유하면서도 각자 다른 저장 규칙을 가질 수 있습니다.

## 언어 바꾸기

아래 메뉴를 사용합니다.

```text
Tools/NexUI/Designer/Language/Korean
Tools/NexUI/Designer/Language/English
```

에디터 UI는 디자이너 로컬라이제이션 테이블을 사용합니다. 누락된 문자열은 창을 깨뜨리지 않고 키 또는 기본 문자열로 표시됩니다.

## 권장 작업 흐름

1. `com.emiteat.nexui`에서 런타임 화면 데이터를 정의합니다.
2. 대응되는 `DesignerMetadataAsset`을 만듭니다.
3. 디자이너를 열고 화면을 할당합니다.
4. element id, class, binding, localization link를 추가합니다.
5. 검증 패널이 깨끗해질 때까지 문제를 수정합니다.
6. 저장한 뒤 Play Mode에서 실제 동작을 확인합니다.
