# 레시피

## 선택한 화면 열기

1. Project 창에서 `UIScreenDefinition` 에셋을 선택합니다.
2. `Tools/NexUI/Designer/Open Selected Screen`을 실행합니다.

디자이너가 열리고 선택한 화면이 할당됩니다.

## 메타데이터 에셋 만들기

1. Project 창에서 우클릭합니다.
2. `Create > NexUI > Designer > Metadata`를 선택합니다.
3. 화면 이름에 맞춰 에셋 이름을 짓습니다. 예: `InventoryScreen.Metadata`
4. 해당 화면을 편집할 때 디자이너에 할당합니다.

## Localization Link 추가하기

1. 메타데이터에서 element id를 추가하거나 선택합니다.
2. 해당 요소에 localization key를 추가합니다.
3. validation을 실행합니다.
4. 누락된 element id 또는 key 경고가 있으면 수정합니다.

안정적인 key 예시:

```text
menu.start
menu.settings
hud.health.label
```

## 커스텀 검증 규칙 추가하기

1. `Editor/` 아래에 작은 validator 또는 service를 만듭니다.
2. `UIScreenDefinition`과 `DesignerMetadataAsset`을 읽습니다.
3. 영향을 받는 id와 함께 명확한 메시지를 반환합니다.
4. 검증 패널에 메시지를 표시합니다.

좋은 검증 메시지는 짧고 바로 고칠 수 있어야 합니다.

## 커스텀 백엔드 추가하기

1. `INexUIDesignerBackend`를 구현합니다.
2. 프리뷰 생성을 추가합니다.
3. 계층 추출을 추가합니다.
4. 선택 매핑을 추가합니다.
5. 백엔드가 별도 데이터를 저장해야 하면 `IDesignerAssetSerializer` 구현을 추가합니다.

기존 UI Toolkit과 uGUI 백엔드를 참고하면 됩니다.

## 죽은 참조 정리하기

메타데이터가 더 이상 존재하지 않는 요소, localization key, contract 항목을 가리킬 때 cleanup 도구를 사용합니다.

정리 후 validation을 다시 실행해 에셋이 깨끗한지 확인합니다.

## 커밋 전 화면 검토하기

1. 디자이너에서 화면을 엽니다.
2. 프리뷰를 rebuild합니다.
3. validation을 실행합니다.
4. 계층 이름을 확인합니다.
5. localization link를 확인합니다.
6. 저장합니다.
7. Play Mode에서 테스트합니다.
