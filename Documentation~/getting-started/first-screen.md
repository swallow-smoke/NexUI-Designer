# 첫 번째 Screen 만들기

이 튜토리얼은 배경, 제목과 세 개의 Button으로 메인 메뉴를 구성합니다. Runtime Command 구현은 프로젝트마다 다르므로 여기서는 Command Key 연결까지만 진행합니다.

## 준비

- `MainMenu` Screen ID를 가진 `UIScreenDefinition`
- 선택한 Backend의 Prefab 또는 `VisualTreeAsset`
- `MainMenu`용 `DesignerMetadataAsset`

## 제작 순서

1. `Panel`을 추가하고 `elementId`를 `background`로 지정합니다. 화면을 채우도록 크기를 조정합니다.
2. `Label`을 추가하고 `title`로 이름을 정한 뒤 Text를 게임 제목으로 바꿉니다.
3. `Container`를 추가해 `menuButtons`로 지정하고 `Auto Layout`을 Vertical로 켭니다.
4. `Button` 세 개를 추가해 각각 `startButton`, `settingsButton`, `quitButton`으로 정합니다.
5. Layers에서 Button을 `menuButtons` 아래로 Reparent합니다. 세 Button이 같은 부모 아래 연속해서 보여야 합니다.
6. Button Text와 `Command Key`를 각각 `menu.start`, `menu.settings`, `menu.quit`로 연결합니다.
7. Align 도구로 Container를 정렬하고 Preview State에서 Hover/Pressed 표현을 확인합니다.
8. Validation을 실행해 중복 ID, 잘못된 Parent와 Button Command 경고를 해결합니다.
9. Save 후 결과 보고서를 확인합니다. UI Toolkit 일반 Save는 UXML 구조를 다시 쓰지 않는다는 점에 유의합니다.
10. Play Mode에서 프로젝트의 `UIActionResolver`가 세 Command Key를 등록했는지 확인합니다.

> [!NOTE]
> Canvas의 `Shape`와 일부 Preview 값은 Backend에 완전히 저장되지 않을 수 있습니다. Inspector의 Backend support와 Save Report를 함께 확인해 주세요.

