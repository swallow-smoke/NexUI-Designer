# Inventory Screen 튜토리얼

완성 결과는 3×2 Slot, Item Name/Count Binding, 빈 Slot, 선택 상태, Hover/Selected Motion과 Equip Command를 가진 화면입니다. Package Manager에서 **Designer Sample**을 Import하면 완성본을 확인할 수 있습니다.

1. `Panel` 아래에 `Grid`를 만들고 Slot 여섯 개를 자식으로 둡니다.
2. Slot ID를 `slot1`부터 `slot6`까지 고정합니다.
3. 각 Slot에 `inventory.slotN.name`, `.count`, `.empty` 또는 `.selected` Key를 연결합니다.
4. Command Key를 `inventory.select.slotN`으로 설정합니다.
5. `equipButton`에 `inventory.equip`을 연결합니다.
6. Slot 1에 Hover/Selected Motion Clip을 연결해 Preview합니다.
7. Validation 후 두 Backend로 저장하고 Play Mode에서 `TemplateCommands.RegisterInventory`를 호출합니다.

Drag & Drop과 Tooltip 자동 연결은 이 Sample에 포함되지 않습니다. 빈 Slot과 선택 Class의 실제 시각 표현은 Backend USS/Prefab 구성을 확인해 주세요.

