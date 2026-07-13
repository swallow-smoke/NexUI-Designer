# Preview와 Scenario

Preview는 Metadata를 편집 중인 Backend Surface에 적용해 결과를 빠르게 확인하는 기능입니다. **Rebuild Preview**는 Surface와 Hierarchy를 다시 구성하고, Preview State는 Hover, Pressed, Selected, Disabled 같은 상태를 강제로 표시합니다.

Interactive Preview는 실제 게임 Command를 실행하지 않고 Preview Log에 시뮬레이션 결과를 남깁니다. Theme, 해상도와 입력 모드도 제작 확인용이며 실제 Player 설정과 차이가 날 수 있습니다.

Scenario Editor는 `DesignerScenarioAsset`의 Text, Value, Visibility Binding 값을 Metadata Preview에 적용하고 Timeline을 Scrub할 수 있습니다. Apply는 Undo를 지원하지만 실제 게임 상태 저장이나 Play Mode 자동화가 아닙니다.

Preview가 비어 있으면 Screen, Backend Asset, Metadata 선택과 Validation Error를 순서대로 확인해 주세요.

> [!IMPORTANT]
> 폰트 측정, 실제 입력 이벤트, Runtime Binding, Safe Area와 Backend별 Layout은 Play Mode에서 재검증해야 합니다.

