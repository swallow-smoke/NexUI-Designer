# Inspector와 Style

Inspector는 선택 상태에 따라 Screen 또는 Element 설정을 보여 줍니다.

- **Design:** Position, Size, Anchor, Auto Layout, Constraints, Element Type, Text, Classes, Shape, Tint, Text Color, Font Size, Image와 값 Preview를 편집합니다.
- **Prototype:** Text/Value/Visibility/Class/Interactable/Command Binding과 Preview State를 다룹니다.
- **Motion:** Screen Entry/Exit Clip, Element Trigger, Reduced Motion Clip, State/Command 조건을 연결합니다.
- **Advanced:** Theme, Accessibility Label/Role, Policy, Capability와 Screen Definition을 노출합니다.

ProgressBar, StatBar, RadialFill은 Min/Max/Preview Value와 Fill Direction을 제공합니다. ChoiceList는 Preview Options, List/Grid/Hotbar는 Preview Item Count를 사용합니다. 이 값 중 `preview*` 필드는 Runtime 데이터가 아니라 제작 확인용일 수 있습니다.

현재 Component Registry에는 Panel, Container, Card, Modal, Popover, Label, Image, Button, IconButton, ChoiceList, ProgressBar, StatBar, RadialFill, Spinner, Skeleton, Toast, Tooltip, List, Grid, Slot, Hotbar와 Custom fallback이 등록되어 있습니다.

