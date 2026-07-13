# Validation과 Save

Validation은 저장 전에 Screen, Metadata, Backend와 Motion 참조를 검사합니다. 결과는 Info, Warning, Error로 구분되며 Element ID가 있는 항목을 클릭하면 해당 Element를 선택합니다. Asset이 연결된 문제는 Project 창에서 Ping합니다.

주요 규칙은 Screen/Metadata ID 불일치, 중복 Element ID, 없는 Parent와 순환, 잘못된 Binding 채널, Button Command 누락, Backend 미지원 Component, 접근성 Label과 터치 크기, Motion target/clip/state/track/keyframe 오류입니다.

Save는 Metadata를 Dirty Asset으로 기록한 뒤 Backend Serializer를 실행합니다. `DesignerSaveReport`는 다음 목록을 구분합니다.

- **Changed:** 실제 기록한 항목
- **Skipped:** Preview 전용이거나 Backend가 쓰지 않은 항목
- **Warnings:** 저장했지만 확인이 필요한 항목
- **Errors:** 저장 실패 항목

`Ctrl/Command+S`와 Toolbar Save를 지원합니다. 미저장 상태에서 Screen을 전환하거나 창을 닫으면 저장 여부를 묻습니다. Error가 남으면 저장 결과와 Console을 확인하고 읽기 전용 Package 경로가 아닌 `Assets/` 아래 에셋을 사용해 주세요.

