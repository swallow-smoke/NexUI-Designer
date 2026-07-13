# Validation Auto Fix

Validation Drawer는 문제 설명, 대상과 해결 방법을 함께 보여 줍니다. 대상 문구를 누르면 Element를 선택하거나 에셋을 Ping합니다. 수정 가능한 항목에는 `Fix`가 표시됩니다.

안전한 자동 수정은 빈/중복 Element ID, 잘못된 부모 연결, 0 또는 작은 크기, Canvas 밖 요소, 장식 Graphic Raycast, 보이지 않는 CanvasGroup 입력 차단과 Button Target Graphic 연결을 포함합니다. `안전한 문제 모두 수정`은 이 항목만 하나의 Undo 그룹으로 처리합니다.

Open Motion에서 Close Motion 생성처럼 새 에셋이나 참조를 만드는 수정은 확인 대화상자를 거칩니다. Command 생성, Motion Track 삭제처럼 의도가 필요한 작업은 자동 수정하지 않습니다. uGUI Component 수정은 Prefab 에셋을 저장하므로 버전 관리 Diff를 확인하세요.
