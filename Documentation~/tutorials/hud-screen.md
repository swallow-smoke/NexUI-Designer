# HUD Screen 튜토리얼

Designer Sample의 HUD를 바탕으로 체력·스태미나 표시를 구성합니다.

1. Safe Area 안에 HUD Root Container를 배치합니다.
2. `StatBar` 또는 `ProgressBar` 두 개를 추가해 Health와 Stamina로 이름을 정합니다.
3. Value Key를 게임 상태 Key에 연결하고 Min/Max와 Preview Value로 제작 상태를 확인합니다.
4. Icon은 `Image`, 숫자는 `Label`로 분리하고 Text Key를 연결합니다.
5. Constraints로 화면 가장자리에 고정하고 여러 Preview 해상도에서 확인합니다.
6. Accessibility Label과 너무 작은 터치 영역 경고를 확인합니다.
7. Save 후 실제 Canvas Scaler 또는 Panel Settings를 사용하는 Play Mode에서 다시 검증합니다.

Safe Area 자동 런타임 보정은 프로젝트 설정에 따라 다르므로 Preview만으로 배포 여부를 판단하지 마세요.

