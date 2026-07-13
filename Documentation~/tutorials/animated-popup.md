# 애니메이션 Popup 튜토리얼

Dim Background, Modal Panel, 제목, 본문과 확인/닫기 Button을 가진 Popup을 구성합니다.

1. 전체 화면 `Panel`을 `dim`으로 만들고 그 아래 `Modal`을 `popup`으로 추가합니다.
2. Modal의 header/content/footer Slot에 Label과 Button을 배치합니다.
3. 확인과 닫기 Button에 프로젝트 Command Key를 연결합니다.
4. `UIMotionClip`을 만들어 `popup`의 Scale과 Alpha Keyframe을 편집합니다.
5. Screen Entry Clip에 열기 Clip, Exit Clip에 닫기 Clip을 연결합니다.
6. Reduced Motion Clip은 짧은 Alpha 변화만 사용하도록 선택적으로 지정합니다.
7. Validation과 Save 후 Runtime에서 Screen Open/Close와 Clip 재생을 확인합니다.

> [!IMPORTANT]
> Entry/Exit 에셋 참조는 저장되지만 모든 Element Trigger가 두 Backend에서 자동 배선되는 것은 아닙니다. 필요한 경우 `PlayMotionClipAsync`를 Runtime Command에서 직접 호출하세요.

