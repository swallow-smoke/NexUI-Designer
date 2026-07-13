# 10분 빠른 시작

**대상:** NexUI Designer를 처음 여는 사용자  
**목표:** Screen을 연결하고 Button 하나를 추가한 뒤 저장합니다.

1. `Tools > NexUI > Designer`를 엽니다. Global Toolbar, 좌측 Sidebar, 중앙 Canvas와 우측 Inspector가 보이면 정상입니다.
2. Project 창에서 `UIScreenDefinition`을 준비하고 Global Toolbar의 **Screen**에 지정합니다. Screen은 Backend와 런타임 Screen ID를 결정합니다.
3. 좌측 Metadata 필드에서 기존 `DesignerMetadataAsset`을 선택하거나 생성합니다. Screen ID가 서로 다르면 Validation 오류가 날 수 있습니다.
4. **Rebuild Preview**를 실행합니다. Canvas가 비어 있다면 Backend Asset 연결을 먼저 확인합니다.
5. 좌측 **Components**에서 `Panel`, `Label`, `Button`을 추가합니다. 새 Element가 Canvas와 **Layers**에 함께 나타나야 합니다.
6. Canvas에서 위치와 크기를 조정하고 우측 Inspector에서 `Name`, `Element Id`, `Text`, `Tint`를 변경합니다.
7. Button의 **Prototype > Binding**에서 `Command Key`를 선택합니다. 이는 Command 구현 자체가 아니라 런타임이 해석할 문자열 계약입니다.
8. **Validate**를 눌러 Error를 해결합니다.
9. **Save** 또는 `Ctrl+S`(macOS는 `Command+S`)를 누릅니다. 저장 결과에서 Written, Skipped, Warning, Error를 확인합니다.
10. Play Mode에서 실제 Backend 결과를 확인합니다.

> [!IMPORTANT]
> Designer Preview는 제작 보조 화면입니다. 입력, 폰트, Layout과 Runtime Binding은 Play Mode에서 다시 확인해야 합니다.

자주 하는 실수는 Screen에 Backend Asset을 지정하지 않거나, UI Toolkit UXML의 `name`과 Metadata `elementId`를 다르게 쓰는 것입니다.

다음 단계는 [첫 화면 만들기](first-screen.md)와 [Validation과 Save](../user-guide/validation-and-save.md)를 참고해 주세요.

