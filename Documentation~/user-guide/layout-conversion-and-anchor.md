# Auto Layout 변환과 Anchor 추천

Canvas에서 둘 이상의 요소를 선택한 뒤 `Layout`을 누르면 현재 배치를 Vertical, Horizontal 또는 Grid 후보로 분석합니다. 감지된 Spacing과 Padding을 적용 전에 수정할 수 있습니다.

적용하면 기존 요소를 Group하고 `DesignerAutoLayoutMetadata`를 설정합니다. uGUI Publish는 Horizontal/Vertical/Grid Layout Group, UI Toolkit Publish는 Flex Direction·Wrap·간격·패딩으로 변환합니다. Grid는 열 수와 Cell 크기도 Metadata에 저장합니다.

`추천 Anchor 적용`은 현재 화면 내 위치에서 3×3 Anchor 또는 Stretch를 추천하며 Rect를 보존합니다. Layout과 Anchor 변경은 Undo 가능합니다. 이미 다른 Layout Group 아래에 있는 복잡한 중첩 구조는 적용 전 수동 확인하세요.
