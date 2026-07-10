# NexUI Designer Sample

이 샘플은 디자이너 패키지를 빠르게 확인하기 위한 작은 에셋과 스크립트를 포함합니다.

## 스타터 템플릿 화면

`Screens/`에 바로 열어볼 수 있는 5개의 완성된 화면이 들어 있습니다 (각각 UI Toolkit / uGUI 백엔드 둘 다 제공):

- **Settings** — 볼륨 행 + Back/Apply 버튼이 있는 설정 창 (Window 레이어, single-instance).
- **Inventory** — Grid/Slot 엘리먼트로 구성된 3x2 인벤토리 그리드.
- **ConfirmDialog** — Modal 레이어의 확인/취소 팝업 (trap focus, additive open policy).
- **Loading** — ProgressBar 자리표시자가 있는 전체화면 로딩 오버레이.
- **HUD** — StatBar(배경+채움) 쌍으로 만든 체력/자원 바 HUD.

각 화면 폴더의 `README.md`를 참고하세요. 버튼의 `commandKey` 바인딩은 `Screens/TemplateCommands.cs`의 no-op 스텁으로 연결되어 있습니다 — Command Pipeline 연결 패턴만 보여주는 자리표시자이므로 실제 프로젝트에서는 로직으로 교체하세요.

## 처음부터 만들어보기

1. `com.emiteat.nexui`에서 `UIScreenDefinition`을 만들거나 가져옵니다.
2. `Create > NexUI > Designer > Metadata`로 메타데이터 에셋을 만듭니다.
3. `Tools/NexUI/Designer`를 엽니다.
4. 툴바에 화면을 할당합니다.
5. `Rebuild`를 누른 뒤 `Validate`를 실행합니다.

## 파일

- `Screens/`는 5개의 스타터 템플릿 화면(위 참고)과 `TemplateCommands.cs`를 포함합니다.
- `AdvancedFeaturesExample.cs`는 메타데이터를 localization, prompt glyph, contrast 도구와 연결하는 예시입니다.
- `ADVANCED_FEATURES.md`는 커밋 전에 화면을 검토할 때 유용한 에디터 도구를 정리합니다.
