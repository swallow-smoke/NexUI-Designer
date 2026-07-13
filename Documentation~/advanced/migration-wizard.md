# Migration Wizard

Migration Wizard는 NexUI Core의 `NexUIMigrationWindow`로, `Assets/` 안에서 알려진 과거 Namespace와 Package ID 문자열을 찾고 선택한 파일을 치환합니다.

`Tools > NexUI > 유틸리티`에서 **Migration**을 선택합니다. `NexUIMigrationWindow` 자체에 직접 `MenuItem`은 없고 Utilities가 선택적으로 창을 연결합니다. 창에서는 **Scan Project**, Select All/None, **Apply Selected Fixes**를 사용합니다.

적용 전 각 원본 옆에 `.bak` 파일을 만듭니다. 그래도 작업 전 Git commit을 만들고 Preview 목록에서 대상 파일과 발생 횟수를 확인해 주세요. Scene/Prefab YAML 외의 의미까지 이해하는 Migration은 아니므로 치환 후 컴파일과 Asset Reload를 검증해야 합니다.
