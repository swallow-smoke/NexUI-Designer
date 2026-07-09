# 설치

## 설치 순서 (중요)

1. 먼저 런타임 패키지 `com.emiteat.nexui` (NexUI)를 설치합니다.
2. 그 다음 에디터 패키지 `com.emiteat.nexui.designer` (NexUI Designer)를 설치합니다.

디자이너는 런타임 패키지의 어셈블리(`emiteat.NexUI.Abstractions`, `emiteat.NexUI.Core`, `emiteat.NexUI.State`, `emiteat.NexUI.Motion`, `emiteat.NexUI.Theme`, `emiteat.NexUI.Components`, `emiteat.NexUI.Query`, `emiteat.NexUI.Accessibility`, `emiteat.NexUI.Localization`, `emiteat.NexUI.Prompt`, `emiteat.NexUI.Templates`, `emiteat.NexUI.Integrations.UIToolkit`, `emiteat.NexUI.Integrations.UGUI`)와 `Unity.TextMeshPro`, `UnityEngine.UI`를 참조합니다. 런타임 패키지가 없으면 디자이너는 컴파일되지 않습니다.

## 이 저장소에서 사용하기

1. Unity 프로젝트를 엽니다.
2. `Packages/` 아래에 두 패키지가 있는지 확인합니다.

```text
Packages/com.emiteat.nexui
Packages/com.emiteat.nexui.designer
```

3. Unity가 스크립트를 다시 임포트할 때까지 기다립니다.
4. `Tools/NexUI/Designer`를 엽니다.

## 로컬 패키지 경로로 설치하기

디자이너 패키지가 프로젝트 밖에 있다면 Unity Package Manager에서 추가합니다.

1. `Window > Package Manager`를 엽니다.
2. `+` 버튼을 누릅니다.
3. `Add package from disk...`를 선택합니다.
4. `Packages/com.emiteat.nexui.designer/package.json`을 선택합니다.

## 의존성

`com.emiteat.nexui.designer`는 `com.emiteat.nexui`에 의존합니다. 디자이너는 런타임 화면 정의, 테마 데이터, 모션 데이터, 런타임 계약을 읽기 때문에 런타임 패키지가 먼저 설치되어 있어야 합니다.

## 설치 확인

임포트 후 아래 항목이 보여야 합니다.

- `Tools/NexUI/Designer`
- `Create > NexUI > Designer > Metadata`
- `emiteat.NexUI.Designer.Runtime` 어셈블리
- `emiteat.NexUI.Designer.Editor` 어셈블리

메뉴가 보이지 않으면 `Assets > Reimport All`을 실행하거나 Unity Editor를 재시작해 스크립트 컴파일을 다시 실행합니다.
