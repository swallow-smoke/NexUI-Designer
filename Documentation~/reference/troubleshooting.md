# 문제 해결

## `Tools > NexUI > Designer` 메뉴가 보이지 않습니다

가능한 원인은 Core/Designer dependency 또는 컴파일 오류입니다. Console의 첫 오류를 해결하고 `package.json`의 Unity `6000.4` 요구 사항과 UniTask 설치를 확인한 뒤 Domain Reload를 기다리세요.

## Designer 창 또는 Preview가 비어 있습니다

Global Toolbar의 `UIScreenDefinition`, Metadata와 Backend Asset을 확인하고 **Rebuild Preview**를 실행합니다. Validation Error와 Backend Asset Type도 확인하세요.

## 저장했지만 Prefab/UXML이 바뀌지 않습니다

Save Report의 Skipped를 확인합니다. uGUI에는 Prefab이 연결되어야 합니다. UI Toolkit 일반 Save는 UXML을 쓰지 않으므로 UI Builder 또는 Generation 도구를 사용해야 합니다.

## Element가 선택되지 않거나 중복 ID 오류가 납니다

Layers에서 Element를 선택하고 Element ID가 비어 있거나 중복되지 않았는지 확인합니다. UI Toolkit은 UXML `name`과도 같아야 합니다.

## Motion Preview가 적용되지 않습니다

Motion Clip, 선택 Element의 target ID와 Designer Preview Surface 연결을 확인합니다. 끊어진 target이나 Missing Clip은 Validation에서 먼저 해결하세요.

## NexUI Core를 찾을 수 없다는 컴파일 오류가 납니다

Core를 Designer보다 먼저 설치하고 버전 `0.1.0`, UniTask `2.5.10`, `com.unity.ugui` dependency를 확인합니다. 계속 실패하면 Unity 버전, 첫 Console 오류, `Packages/manifest.json`과 Package Manager 목록을 함께 수집해 주세요.

