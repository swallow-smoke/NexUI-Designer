NexUI Designer

Design and ship in-game UI without leaving Unity.

NexUI Designer는 Unity Editor 안에서 인게임 UI를 직접 디자인하고, 완성된 결과를 실제 게임 화면으로 사용할 수 있게 만드는 비주얼 UI 제작 도구입니다.

게임 UI를 만들기 위해 Figma, UI Builder, Prefab Inspector, Animator, 코드 사이를 반복해서 이동하는 과정을 줄이고, 하나의 환경에서 UI 제작을 끝내는 것을 목표로 합니다.

NexUI Designer는 단순한 화면 목업 도구나 메타데이터 편집기가 아닙니다.

Design
→ Bind
→ Animate
→ Preview
→ Use in Game

Designer에서 만든 결과가 실제 게임 화면으로 이어지고, 제작 중인 UI의 구조와 상태, 상호작용, 연출을 Unity 안에서 직접 확인할 수 있는 작업 흐름을 지향합니다.

⸻

Why NexUI Designer?

게임 UI는 디자인과 구현 사이의 간격이 큽니다.

외부 디자인 도구에서 만든 화면을 Unity에서 다시 배치하고, 상태와 입력을 코드로 연결하고, 애니메이션을 별도로 작성하는 과정에서 같은 작업이 반복됩니다.

NexUI Designer는 이 간격을 줄이기 위해 만들어졌습니다.

외부에서 디자인
→ Unity에서 재구현
→ 코드 연결
→ 다시 수정

이 흐름을 다음처럼 단순화하는 것이 목표입니다.

Unity 안에서 디자인
→ 바로 연결
→ 바로 확인
→ 게임에서 사용



⸻

NexUI Ecosystem

NexUI는 제작 환경과 런타임을 분리합니다.

NexUI Designer

UI를 제작합니다.

NexUI Core

제작된 UI를 게임에서 실행하고 관리합니다.

NexUI Designer
UI를 만든다.
        ↓
NexUI Core
게임에서 실행한다.

NexUI Core는 화면의 열기와 닫기, 상태 연결, 입력, 수명주기와 같은 런타임 역할을 담당합니다.

NexUI Designer는 그 위에서 실제 게임 UI를 더 빠르고 편하게 만드는 제작 환경입니다.

⸻

Project Vision

NexUI Designer가 지향하는 것은 단순히 기능이 많은 Unity Editor Window가 아닙니다.

목표는 다음과 같습니다.

게임 UI를 디자인하고 구현하는 전체 과정을 하나의 제작 환경으로 통합한다.

최종적으로 사용자는 다음 과정을 자연스럽게 수행할 수 있어야 합니다.

UI를 만든다
→ 게임 데이터와 연결한다
→ 상호작용과 애니메이션을 구성한다
→ 실제 상태로 확인한다
→ 게임에서 그대로 사용한다

Designer에서 보이는 결과와 Runtime에서 보이는 결과의 차이를 최소화하는 것이 핵심 원칙입니다.

Designer Canvas ≈ Runtime Result



⸻

Built for Game UI

NexUI Designer는 일반적인 앱 UI보다 게임 UI 제작에 초점을 둡니다.

HUD, 인벤토리, Hotbar, 상태 표시, 모달, 대화창, 퀘스트, 제작 화면처럼 게임에서 반복적으로 사용되는 UI를 더 빠르게 만들고 관리할 수 있는 환경을 목표로 합니다.

마우스뿐 아니라 키보드, 게임패드, 터치 입력을 고려하고, 다양한 해상도와 실제 게임 상태에서 UI가 어떻게 동작하는지를 제작 과정 안에서 확인할 수 있도록 설계됩니다.

⸻

Core Philosophy

Create visually.
Connect without friction.
Preview immediately.
Ship directly.

NexUI Designer는 UI 제작 과정에서 불필요한 도구 이동과 반복 구현을 줄이고, 개발자가 실제 게임 화면에 더 집중할 수 있게 만드는 것을 목표로 합니다.

⸻

Status

NexUI Designer는 현재 개발 중인 프로젝트입니다.

구조와 기능은 계속 확장되고 있으며, 최종적으로 Unity 안에서 게임 UI 제작 전반을 처리할 수 있는 통합 제작 환경을 지향합니다.

⸻

Related Project

NexUI Core

NexUI Designer에서 제작한 UI를 게임에서 실행하고 관리하는 런타임 시스템입니다.

⸻

Build the UI in NexUI Designer. Run it with NexUI Core.
