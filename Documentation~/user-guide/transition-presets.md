# 전환 프리셋

선택 요소에서 기존 Motion Clip 에셋을 빠르게 만드는 기능입니다. Canvas Toolbar의 `Transition` 또는 우클릭 메뉴 `생산성/전환 프리셋 적용`에서 엽니다.

Fade, Slide 4방향, Scale Pop, Modal, Dropdown, Tooltip, Toast, Stagger List를 지원합니다. Duration, Delay, Easing, 이동 거리, Alpha, Scale, Overshoot, 자식 포함과 Stagger 순서를 조절할 수 있습니다.

`열기 미리보기`와 `닫기 미리보기`는 Motion Clip Editor를 열지 않고 현재 Preview Surface에서 재생합니다. 적용 시 Open Clip과 시간을 뒤집은 Close Clip을 별도 에셋으로 저장하고 Screen Motion에 연결합니다. 연결 변경은 Undo할 수 있습니다.

삭제된 Element ID를 가리키는 Track은 Validation에서 검출됩니다. Preview는 실제 Runtime Command를 실행하지 않습니다.
