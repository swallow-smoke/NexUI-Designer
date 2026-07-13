# Preview Scenario와 Mock Data

Scenario는 게임을 실행하지 않고 Binding 결과를 확인하기 위한 독립 에셋입니다. Global Toolbar의 `Scenario` 필드에서 바로 적용하거나 Scenario Editor에서 생성·편집·Capture할 수 있습니다.

지원 값은 Bool, Number, Text, Sprite, List입니다. Text는 텍스트 채널, Bool은 표시 상태, Number는 값 프리뷰, Sprite는 이미지 프리뷰, List는 Collection의 프리뷰 아이템 수에 적용됩니다. `현재 상태를 Scenario로 저장`은 Canvas 우클릭 메뉴에서 사용할 수 있습니다.

Scenario 적용은 Metadata Preview 값만 변경하고 실제 Command나 게임 상태를 실행하지 않습니다. 적용은 한 Undo 그룹으로 묶입니다. Timeline은 Bool, Number, Text, Sprite, List를 편집하며 Number만 선형 보간하고 나머지 값은 이전 Key를 유지하다 다음 Key에서 전환합니다. Capture/Apply는 해상도, 입력 장치와 Theme도 함께 복원합니다.

Scenario Editor에서는 이전/다음 이동, 복제, 이름 변경, 초기화, 삭제와 현재 상태 Capture를 제공합니다. 삭제는 에셋을 휴지통으로 이동하며 Unity Undo 대상이 아니므로 확인 대화상자가 표시됩니다.

선택 요소의 Canvas 우클릭 메뉴 `생산성/빠른 테스트`에서 다음 경계값을 즉시 적용할 수 있습니다.

- Text: 빈 값, 짧은/긴 값, 여러 줄, 256자, 한국어/영어/일본어, 숫자, 특수문자
- Value: 0, 최소, 중간, 최대, 범위 초과
- Collection: 0, 1, 5, 20, 100개
