# HUD Screen 튜토리얼

## 완성 결과와 사전 준비

Health와 Stamina, Icon과 수치 Label을 가진 HUD를 만듭니다. Sample을 Import한 경우 `Screens/HUD/HUD.Metadata.asset`과 원하는 Backend Screen부터 엽니다.

## 최종 Hierarchy

```text
HUDRoot
└─ StatusContainer
   ├─ HealthBar
   │  ├─ HealthIcon
   │  └─ HealthText
   └─ StaminaBar
      ├─ StaminaIcon
      └─ StaminaText
```

## 단계별 제작

1. Safe Area 안에 `HUDRoot`를 두고 `StatusContainer`를 자식으로 만듭니다.
2. Components에서 `StatBar` 또는 `ProgressBar` 두 개를 추가해 `HealthBar`, `StaminaBar`로 지정합니다.
3. Inspector에서 Min/Max와 Preview Value를 바꿔 0%, 중간, 100%를 확인합니다.
4. Icon은 Image, 숫자는 Label로 분리하고 위 Hierarchy처럼 부모를 지정합니다.
5. Constraint/Anchor를 화면 가장자리에 맞춘 뒤 Canvas Toolbar에서 여러 해상도를 선택합니다.

## Binding과 Scenario

Value Key는 Sample Runtime 모델의 Health/Stamina Key와 정확히 맞추고 Text Key는 표시 문자열 Key에 연결합니다. Empty, Critical, Full 같은 Scenario를 만들 때 Number 최소/최대와 Text 길이를 함께 바꿉니다. Number Scenario Timeline은 선형 보간되므로 값 변화 Preview에 사용할 수 있습니다.

## Validation, Save와 Play Mode

Accessibility Label, 0 Size, Canvas 밖 배치와 작은 상호작용 영역을 확인합니다. Save Report의 Progress `Partial`/Skipped 메시지를 읽습니다. uGUI의 filled Image와 UI Toolkit의 generated style은 Preview의 가상 Track/Label을 완전히 동일하게 저장하지 않을 수 있습니다.

Play Mode에서는 실제 Canvas Scaler 또는 Panel Settings, Safe Area 보정, Runtime Binding 갱신을 확인합니다. Preview만으로 해상도 대응 완료를 판단하지 마세요.

## 현재 제한과 확장 과제

Safe Area 자동 Runtime 보정과 값 변화 애니메이션은 프로젝트 Runtime 정책에 달려 있습니다. 수치가 급격히 변할 때, 21:9와 세로 화면에서 각각 확인해 보세요.

