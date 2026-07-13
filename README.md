NexUI Designer는 NexUI 화면을 Unity Editor 안에서 시각적으로 설계하고 검증하기 위한 에디터 확장 패키지입니다.

컴포넌트 배치, 부모-자식 계층 구성, 정렬, 레이어 관리, 데이터 바인딩, 테마, 접근성, 모션, 검증 과정을 하나의 작업 공간에서 처리할 수 있습니다.

NexUI Designer는 런타임 UI 시스템을 대체하지 않습니다.
	•	com.emiteat.nexui는 화면 생성, 상태, 입력, 모션과 같은 런타임 동작을 담당합니다.
	•	com.emiteat.nexui.designer는 화면 편집, 프리뷰, 검증과 같은 에디터 제작 도구를 담당합니다.

⸻

주요 기능

Visual UI Authoring
	•	Unity Editor 안에서 동작하는 시각적 UI 캔버스
	•	21개의 기본 NexUI 컴포넌트
	•	컴포넌트 검색 및 카테고리별 팔레트
	•	드래그 이동 및 리사이즈
	•	다중 선택과 영역 선택
	•	복사, 붙여넣기, 복제, 삭제
	•	요소 그룹화 및 그룹 해제
	•	캔버스 줌, 스냅, 그리드
	•	선택 요소 포커스
	•	인라인 이름 변경
	•	Unity Undo/Redo 지원

Hierarchy & Layer
	•	실제 부모-자식 계층 구조
	•	형제 요소 순서 기반의 z-order 관리
	•	요소별 잠금 및 에디터 전용 숨김
	•	계층과 캔버스 선택 상태 양방향 동기화
	•	부모 및 자식 요소 빠른 선택
	•	앞으로/뒤로 이동
	•	최상단/최하단 이동
	•	부모 컴포넌트의 named slot에 자식 배치
	•	컨테이너 자식 클리핑과 내부 패딩 메타데이터

Layout Tools
	•	왼쪽, 가운데, 오른쪽 정렬
	•	위, 가운데, 아래 정렬
	•	수평 및 수직 균등 분배
	•	캔버스 기준 단일 요소 정렬
	•	선택 영역 기준 다중 요소 정렬
	•	Anchor Preset
	•	Auto Layout 메타데이터
	•	Figma 스타일 Constraints 메타데이터
	•	고정 크기, Hug, Fill 크기 정책

Inspector

선택한 요소에 따라 다음 설정을 편집할 수 있습니다.
	•	Layout
	•	Auto Layout
	•	Style
	•	Shape
	•	Binding
	•	Theme
	•	Motion
	•	Accessibility
	•	Constraints
	•	Screen Policy
	•	Component Capability
	•	Validation 정보

Component Preview

컴포넌트 타입별 전용 프리뷰 렌더러를 제공합니다.
	•	텍스트 및 버튼
	•	이미지와 아이콘 버튼
	•	Progress Bar
	•	Stat Bar
	•	Radial Fill
	•	Spinner
	•	Skeleton
	•	Choice List
	•	List
	•	Grid
	•	Slot
	•	Hotbar
	•	사용자 정의 컴포넌트용 Generic Preview

프리뷰 전용 값을 사용해 런타임 데이터가 없어도 컴포넌트의 예상 모습을 확인할 수 있습니다.
	•	진행률 값
	•	최소값과 최대값
	•	Fill Direction
	•	회전 방향
	•	이미지 텍스처
	•	리스트 및 그리드 아이템 수
	•	Hotbar 슬롯 수
	•	Choice List 옵션
	•	Normal, Hover, Pressed, Disabled 등의 상태

Backend Support

하나의 Designer 작업 공간에서 두 UI 백엔드를 다룹니다.
	•	Unity UI Toolkit
	•	Unity uGUI

백엔드별 프리뷰와 저장 정책은 서로 분리되어 있습니다.

Motion Authoring

두 종류의 모션 편집 시스템을 제공합니다.
	•	Motion Graph Editor
	•	스텝 기반 노드 그래프
	•	From / To / Duration / Delay / Easing
	•	노드 의존성 연결
	•	자동 레이아웃
	•	타임라인 프리뷰
	•	Motion Clip Editor
	•	다중 요소 키프레임 타임라인
	•	Position
	•	Rotation
	•	Scale
	•	Size
	•	Alpha
	•	Easing 및 AnimationCurve
	•	실시간 스크러빙
	•	Play / Stop / Loop

Validation

Play Mode에 들어가기 전에 제작 과정에서 발생한 문제를 검사합니다.
	•	누락된 화면 참조
	•	잘못된 element id
	•	중복되거나 손상된 계층
	•	지원되지 않는 부모-자식 관계
	•	slot 규칙 위반
	•	잘못된 binding
	•	누락된 localization link
	•	잘못된 responsive rule
	•	누락된 contract
	•	백엔드 지원 상태
	•	삭제된 요소를 가리키는 메타데이터

검증 항목을 클릭하면 문제가 발생한 요소를 Hierarchy와 캔버스에서 바로 선택할 수 있습니다.

Additional Tools
	•	한국어 및 영어 에디터 UI
	•	전체 UI 툴팁 로컬라이제이션
	•	History 패널
	•	UI Builder에서 백엔드 에셋 열기
	•	백엔드에서 메타데이터 동기화
	•	마이그레이션 위저드
	•	Figma API 연결 테스트
	•	Snapshot 및 Diff 서비스
	•	Cleanup 및 Profiling 도구
	•	Responsive Rule 및 Contract 도구

⸻

요구 사항

항목	요구 사항
Unity	6000.4 이상
Runtime Package	com.emiteat.nexui
UI System	UI Toolkit 또는 uGUI
Text	TextMeshPro
Designer Package	com.emiteat.nexui.designer

NexUI Designer는 NexUI 런타임 어셈블리를 참조하므로 반드시 NexUI를 먼저 설치해야 합니다.

⸻

설치

Package Manager에서 Git URL로 설치

먼저 NexUI 런타임 패키지를 설치합니다.

https://github.com/swallow-smoke/NexUI.git

그다음 NexUI Designer를 설치합니다.

https://github.com/swallow-smoke/NexUI-Designer.git

설치 순서:
	1.	Unity에서 Window > Package Manager를 엽니다.
	2.	좌측 상단의 + 버튼을 누릅니다.
	3.	Install package from git URL...을 선택합니다.
	4.	NexUI 저장소 URL을 입력합니다.
	5.	설치가 끝나면 같은 방법으로 NexUI Designer 저장소 URL을 입력합니다.

manifest.json에 직접 추가

프로젝트의 Packages/manifest.json에 다음 항목을 추가할 수도 있습니다.

{
  "dependencies": {
    "com.emiteat.nexui": "https://github.com/swallow-smoke/NexUI.git",
    "com.emiteat.nexui.designer": "https://github.com/swallow-smoke/NexUI-Designer.git"
  }
}

기존 dependencies 항목이 있다면 두 패키지만 해당 객체 안에 추가합니다.

배포 환경에서는 브랜치 최신 상태를 그대로 참조하는 대신 태그나 커밋 SHA로 버전을 고정하는 방식을 권장합니다.

{
  "com.emiteat.nexui.designer": "https://github.com/swallow-smoke/NexUI-Designer.git#COMMIT_SHA"
}

로컬 패키지로 설치

저장소를 직접 내려받았다면 다음 순서로 설치합니다.
	1.	Window > Package Manager를 엽니다.
	2.	+ 버튼을 누릅니다.
	3.	Add package from disk...를 선택합니다.
	4.	NexUI의 package.json을 선택합니다.
	5.	같은 방법으로 NexUI Designer의 package.json을 선택합니다.

프로젝트 내부에 직접 배치하는 경우 다음 구조를 사용할 수 있습니다.

Packages/
├── com.emiteat.nexui/
│   └── package.json
└── com.emiteat.nexui.designer/
    └── package.json

설치 확인

설치가 완료되면 다음 메뉴와 어셈블리가 표시되어야 합니다.

Tools/NexUI/Designer
Create/NexUI/Designer/Metadata

emiteat.NexUI.Designer.Runtime
emiteat.NexUI.Designer.Editor

메뉴가 나타나지 않는 경우:
	1.	Console의 컴파일 오류를 확인합니다.
	2.	NexUI 런타임 패키지가 먼저 설치되었는지 확인합니다.
	3.	Assets > Reimport All을 실행합니다.
	4.	Unity Editor를 다시 시작합니다.

⸻

Sample 가져오기

패키지에는 Designer Sample이 포함되어 있습니다.
	1.	Window > Package Manager를 엽니다.
	2.	NexUI Designer를 선택합니다.
	3.	Samples 탭을 엽니다.
	4.	Designer Sample의 Import 버튼을 누릅니다.

샘플에는 Designer에서 확인할 수 있는 화면과 메타데이터 예제가 포함되어 있습니다.

⸻

빠른 시작

1. Screen Definition 준비

NexUI 런타임 패키지에서 UIScreenDefinition 에셋을 생성하거나 기존 화면 에셋을 준비합니다.

UIScreenDefinition은 런타임 화면의 루트 정의이며, Designer는 이 에셋을 기준으로 프리뷰와 검증을 수행합니다.

2. Designer Metadata 생성

다음 메뉴에서 메타데이터 에셋을 생성합니다.

Create > NexUI > Designer > Metadata

Designer 창의 Metadata 필드 옆에 있는 New 버튼을 사용해 현재 화면용 메타데이터를 생성할 수도 있습니다.

3. Designer 열기

Tools > NexUI > Designer

4. 화면 연결

Designer 상단 툴바에서 다음 필드를 지정합니다.
	•	Screen: 편집할 UIScreenDefinition
	•	Metadata: 화면에 대응하는 DesignerMetadataAsset

화면을 지정한 뒤 Rebuild를 누릅니다.

5. 컴포넌트 추가

왼쪽 Components 팔레트에서 원하는 컴포넌트를 선택해 화면에 추가합니다.

추가된 컴포넌트는 자동으로 선택되며, Hierarchy와 Inspector가 해당 요소를 기준으로 갱신됩니다.

6. 화면 편집

캔버스와 Inspector를 사용해 다음 항목을 편집합니다.
	•	위치와 크기
	•	부모와 자식 관계
	•	sibling 순서
	•	slot
	•	텍스트
	•	색상
	•	Shape
	•	Binding
	•	Theme
	•	Motion
	•	Accessibility
	•	Preview Value

7. 검증

상단 툴바에서 Validate를 누릅니다.

검증 결과가 남아 있다면 항목을 클릭해 문제가 발생한 요소로 이동합니다.

8. 저장

상단 툴바에서 Save를 누릅니다.

저장이 끝나면 Play Mode에서 실제 런타임 화면, 입력, 애니메이션을 확인합니다.

⸻

Designer 레이아웃

┌──────────────────────────────────────────────────────────────────┐
│ Toolbar                                                          │
│ Screen / Metadata / Mode / Preview State / Input / Snap / Zoom   │
│ Rebuild / Save / Validate                                        │
├────────────────┬───────────────────────────┬─────────┬────────────┤
│ Components     │                           │ Align   │ Inspector  │
│ Palette        │                           │         │ Validation │
├────────────────┤        Viewport           │ Layer   │ History    │
│ Hierarchy      │        Canvas             │         │            │
│                │                           │         │            │
├────────────────┴───────────────────────────┴─────────┴────────────┤
│ State Preview │ Command Preview │ Screen Graph                    │
└──────────────────────────────────────────────────────────────────┘

주요 영역의 경계는 마우스로 드래그해 크기를 조절할 수 있습니다.

변경한 패널 크기와 일부 UI 상태는 EditorPrefs에 저장되며, 창을 닫았다 다시 열어도 유지됩니다.

⸻

기본 컴포넌트

NexUI Designer는 21개의 기본 컴포넌트를 제공합니다.

분류	컴포넌트	용도
Container	Panel	범용 시각적 컨테이너
Container	Container	기본 그래픽이 없는 레이아웃 컨테이너
Container	Card	Header, Content, Footer를 가진 콘텐츠 표면
Overlay	Modal	Backdrop을 포함하는 모달 화면
Overlay	Popover	특정 요소에 연결되는 오버레이
Text	Label	일반 텍스트 및 로컬라이제이션 텍스트
Media	Image	이미지 또는 텍스처 표시
Input	Button	텍스트와 아이콘을 포함할 수 있는 버튼
Input	Icon Button	아이콘 중심 버튼
Input	Choice List	단일 또는 다중 선택 목록
Feedback	Progress Bar	선형 진행률 표시
Feedback	Stat Bar	HP, Stamina 등의 게임 스탯 표시
Feedback	Radial Fill	원형 진행률 표시
Feedback	Spinner	회전형 로딩 표시
Feedback	Skeleton	로딩용 Skeleton UI
Overlay	Toast	일시적인 알림
Overlay	Tooltip	보조 설명 표시
Collection	List	반복되는 세로 목록
Collection	Grid	반복되는 그리드 목록
Collection	Slot	인벤토리 등의 단일 슬롯
Collection	Hotbar	여러 슬롯으로 구성된 단축바

알 수 없는 사용자 정의 타입은 삭제하거나 오류를 발생시키지 않고 Generic 컴포넌트로 표시합니다.

⸻

계층과 Slot

모든 요소는 다음 계층 정보를 가질 수 있습니다.

elementId
parentId
siblingIndex
parentSlotId

	•	elementId: 요소를 식별하는 고유 ID
	•	parentId: 부모 요소의 ID
	•	siblingIndex: 같은 부모 아래에서의 순서
	•	parentSlotId: 부모 컴포넌트 내부에서 요소가 들어갈 named slot

Slot 예시

Card
├── header
├── content
└── footer

Button
├── icon
└── content

Icon Button
├── icon
└── badge

Choice List
├── option
└── empty

List, Grid, Choice List, Hotbar와 같은 컬렉션 컴포넌트는 템플릿 또는 생성 콘텐츠용 slot을 가질 수 있습니다.

부모가 될 수 없는 Label, Image, Progress Bar 등의 leaf 컴포넌트에는 자식을 배치할 수 없습니다.

⸻

캔버스 조작

선택
	•	클릭: 단일 선택
	•	Shift + 클릭: 기존 선택에 추가
	•	Ctrl + 클릭: 선택 상태 토글
	•	빈 캔버스 드래그: 영역 선택
	•	Ctrl + A: 전체 선택
	•	Esc: 선택 해제

Hierarchy와 캔버스의 선택은 서로 동기화됩니다.

이동
	•	드래그: 선택 요소 이동
	•	다중 선택 후 드래그: 선택 요소 전체 이동
	•	드래그 중 Shift: 주 이동축으로 고정
	•	방향키: 1px 이동
	•	Shift + 방향키: 10px 이동

리사이즈

선택 요소의 리사이즈 핸들을 드래그해 크기를 변경합니다.

잠긴 요소는 캔버스에서 이동하거나 리사이즈할 수 없습니다.

줌과 스냅
	•	마우스 휠: 캔버스 줌
	•	툴바 -, +: 줌 조절
	•	Snap: 그리드 스냅 활성화
	•	Grid Size: 스냅 단위 지정

기본 그리드 크기는 8px입니다.

우클릭 메뉴

요소를 우클릭하면 다음 기능을 사용할 수 있습니다.
	•	Select
	•	Add to Selection
	•	Select Parent
	•	Select Children
	•	Rename
	•	Duplicate
	•	Delete
	•	Bring Forward
	•	Send Backward
	•	Bring To Front
	•	Send To Back
	•	Align
	•	Distribute
	•	Group
	•	Ungroup
	•	Create Motion Clip From Selection

여러 요소가 겹쳐 있는 위치를 우클릭하면 Select Element 메뉴를 통해 원하는 요소를 정확히 선택할 수 있습니다.

⸻

정렬과 레이어

Align

두 개 이상의 요소를 선택하면 선택 영역의 bounding box를 기준으로 정렬합니다.

요소 하나만 선택하면 캔버스 영역을 기준으로 정렬합니다.

기능	단축키
Align Left	Alt + L
Align Center X	Alt + C
Align Right	Alt + R
Align Top	Alt + T
Align Center Y	Alt + M
Align Bottom	Alt + B

Distribute

세 개 이상의 요소를 선택했을 때 사용할 수 있습니다.

기능	단축키
Distribute Horizontal	Alt + H
Distribute Vertical	Alt + V

Layer

메타데이터의 형제 순서는 실제 z-order와 백엔드의 sibling 순서에 사용됩니다.

기능	단축키
Bring Forward	Ctrl + ]
Send Backward	Ctrl + [
Bring To Front	Ctrl + Shift + ]
Send To Back	Ctrl + Shift + [

Group

두 개 이상의 요소를 선택하고 Ctrl + G를 누르면 선택 영역을 감싸는 새로운 Panel이 생성됩니다.

선택 요소들은 새 Panel의 자식으로 이동합니다.

Ctrl + G

그룹을 해제하려면 다음 단축키를 사용합니다.

Ctrl + Shift + G



⸻

Inspector

Layout
	•	Position
	•	Size
	•	Anchor
	•	Locked
	•	Parent
	•	Sibling Index
	•	Parent Slot
	•	Clip Children
	•	Content Padding

Auto Layout

요소를 직계 자식들을 자동 배치하는 컨테이너로 설정합니다.
	•	Direction
	•	Row
	•	Column
	•	Spacing
	•	Padding
	•	Width
	•	Fixed
	•	Hug
	•	Fill
	•	Height
	•	Fixed
	•	Hug
	•	Fill

현재 Auto Layout은 메타데이터 작성 중심의 기능입니다. 백엔드 에셋에 완전한 flexbox 구조를 자동 생성하지는 않습니다.

Style
	•	Element Id
	•	Display Name
	•	Component Type
	•	Text
	•	Classes
	•	Shape
	•	Tint
	•	Text Color
	•	Font Size
	•	Hidden
	•	Preview Value
	•	Min Value
	•	Max Value
	•	Fill Direction
	•	Clockwise
	•	Preview Image
	•	Preview Item Count
	•	Preview Options

Shape

값	설명
Rectangle	각진 사각형
Rounded	작은 반경의 둥근 사각형
Pill	높이를 기준으로 완전히 둥근 형태
Circle	원 또는 타원

Shape는 Designer 프리뷰용 설정입니다.

실제 uGUI 스프라이트나 UI Toolkit USS의 모서리 스타일을 자동으로 변경하지는 않습니다.

Binding

NexUI 상태와 요소를 연결하는 키를 지정합니다.
	•	Text Key
	•	Value Key
	•	Visibility Key
	•	Class Key
	•	Command Key
	•	Interactable Key

프로젝트에서 발견된 command와 bindable property는 Pick... 기능을 사용해 선택할 수 있습니다.

Theme
	•	Theme Asset
	•	Theme Id
	•	Style Classes
	•	Token Overrides
	•	Color Eyedropper
	•	Add as Token

Motion

다음 상태별 variant와 Motion Preset을 지정할 수 있습니다.
	•	Initial
	•	Animate
	•	Exit
	•	Hover
	•	Pressed
	•	Focus

Inspector에서 Motion Graph Editor 또는 Motion Clip Editor를 바로 열 수 있습니다.

Accessibility
	•	Screen-reader Label
	•	Accessibility Role

Button, Dialog, Image, List, Progress Indicator 등의 의미론적 역할을 지정할 수 있습니다.

Policy

화면 전체에 적용되는 동작 정책을 설정합니다.
	•	Input Blocking
	•	Pause Game Behind
	•	Close On Back
	•	Cursor Policy
	•	Time Policy
	•	Focus Policy
	•	Conflict Policy
	•	Lifetime Policy

⸻

Simple / Advanced Mode

툴바에서 Inspector 표시 수준을 변경할 수 있습니다.

Simple

기본적인 화면 제작에 필요한 기능만 표시합니다.
	•	Layout
	•	Style
	•	기본 Auto Layout
	•	Command Key

Advanced

NexUI의 전체 제작 설정을 표시합니다.
	•	모든 Binding
	•	Theme
	•	Motion
	•	Constraints
	•	Policy
	•	Capability
	•	Token Override

선택한 모드는 EditorPrefs에 저장됩니다.

⸻

Preview State와 Interaction

Designer는 런타임 실행 없이 컴포넌트 상태를 시각적으로 확인할 수 있습니다.

대표적인 상태:
	•	Normal
	•	Hover
	•	Pressed
	•	Focused
	•	Disabled
	•	Selected
	•	Loading
	•	Empty
	•	Error
	•	Indeterminate

Interactive Preview에서는 버튼과 같은 상호작용 컴포넌트의 command를 실제 게임 로직으로 전달하지 않고 Preview Log에 기록합니다.

이를 통해 에디터에서 실수로 런타임 로직을 실행하지 않으면서 command 연결 상태를 확인할 수 있습니다.

⸻

값 기반 컴포넌트 프리뷰

Progress Bar / Stat Bar

다음 값을 사용해 실제 채움 상태를 확인합니다.
	•	Preview Value
	•	Min Value
	•	Max Value
	•	Fill Direction

지원 방향:
	•	Left To Right
	•	Right To Left
	•	Bottom To Top
	•	Top To Bottom

Radial Fill
	•	원형 트랙
	•	Preview Value 기반 Arc
	•	Clockwise 방향 설정

Spinner

Painter2D 기반의 회전 링을 표시합니다.

Collection

List, Grid, Hotbar는 Preview Item Count를 사용해 가상 아이템을 표시합니다.

가상 아이템은 프리뷰에만 사용되며 실제 authored element로 저장되지 않습니다.

Choice List는 Preview Options를 사용해 옵션 텍스트를 미리 확인할 수 있습니다.

⸻

이미지 프리뷰

Image 또는 Icon Button 컴포넌트를 선택하면 Inspector에 Preview Image 필드가 표시됩니다.
	•	Image: 요소 영역에 맞춰 텍스처 표시
	•	Icon Button: 버튼 중앙에 아이콘 표시

Preview Image는 Designer 캔버스 확인용입니다.

실제 런타임 Sprite 또는 Texture 설정은 uGUI Prefab이나 UI Toolkit 백엔드 에셋에서 별도로 관리합니다.

⸻

저장 동작

Designer는 저장 결과를 DesignerSaveReport로 요약하고 Console과 툴바에 표시합니다.

공통 저장 데이터

다음 데이터는 DesignerMetadataAsset에 저장됩니다.
	•	element id
	•	rect
	•	parent id
	•	sibling index
	•	parent slot
	•	shape
	•	preview data
	•	binding
	•	class
	•	localization link
	•	variant
	•	responsive rule
	•	contract
	•	snapshot
	•	accessibility
	•	layout metadata

uGUI Backend

백엔드 에셋이 Prefab인 경우 다음 정보를 Prefab에 반영할 수 있습니다.
	•	RectTransform 위치와 크기
	•	active 상태
	•	텍스트
	•	텍스트 색상
	•	폰트 크기
	•	Graphic 및 Image Tint
	•	Button 컴포넌트
	•	부모-자식 계층
	•	sibling 순서

Prefab은 다음 Unity API를 사용해 안전하게 저장됩니다.

LoadPrefabContents
SaveAsPrefabAsset
UnloadPrefabContents

다음 값은 Designer 프리뷰 전용이므로 Prefab 런타임 값으로 자동 저장되지 않습니다.
	•	Shape
	•	Preview Value
	•	Preview Fill
	•	Clockwise
	•	Preview Image
	•	Preview Item Count
	•	Preview Options

UI Toolkit Backend

UI Toolkit 백엔드에서는 UXML을 자동으로 다시 작성하지 않습니다.

Designer는 다음 작업을 담당합니다.
	•	UXML 기반 프리뷰
	•	Designer Metadata 저장
	•	UXML name과 metadata element id 검증
	•	백엔드 요소를 메타데이터로 동기화
	•	UI Builder에서 백엔드 에셋 열기

실제 UXML과 USS 저작은 Unity UI Builder가 담당합니다.

Backend 메뉴

Tools/NexUI/Designer/Backend/Sync Metadata From Backend
Tools/NexUI/Designer/Backend/Apply Metadata To Preview
Tools/NexUI/Designer/Backend/Open Backend Asset In UI Builder
Tools/NexUI/Designer/Backend/Ping Backend Asset

Apply Metadata To Preview는 현재 프리뷰에만 적용되며 디스크 저장 작업이 아닙니다.

⸻

화면 검증

툴바의 Validate 버튼 또는 다음 메뉴를 사용합니다.

Tools/NexUI/Designer/Validate Current Screen

Validation 패널의 각 문제를 클릭하면 해당 요소가 자동으로 선택됩니다.

검증 결과가 없더라도 최종 입력 동작과 애니메이션은 Play Mode에서 다시 확인해야 합니다.

⸻

Motion Graph Editor

Motion Graph Editor는 UIMotionPreset의 UIMotionGraph를 편집하는 노드 기반 도구입니다.

Tools/NexUI/Designer/Motion Graph

각 노드는 하나의 UIMotionStep을 나타냅니다.

Property
From
To
Duration
Delay
Easing

노드의 Output을 다른 노드의 Input에 연결하면 앞 노드가 끝난 뒤 다음 노드가 실행되는 의존성이 생성됩니다.

지원 기능:
	•	Add Node
	•	Duplicate Node
	•	Dependency Connection
	•	Auto Layout
	•	Save Now
	•	Undo/Redo
	•	Timeline Preview

새로운 빈 그래프에는 start와 end 노드가 자동으로 생성됩니다.

Motion Graph Preview는 보간값과 전체 Duration을 확인하는 읽기 전용 프리뷰입니다.

⸻

Motion Clip Editor

Motion Clip Editor는 여러 UI 요소를 하나의 키프레임 타임라인에서 편집하는 도구입니다.

Tools/NexUI/Designer/Motion Clip Editor

지원 Property
	•	Anchored Position
	•	Local Position
	•	Local Rotation Z
	•	Local Scale
	•	Size Delta
	•	Canvas Group Alpha

기본 작업 흐름
	1.	Create Motion Clip으로 UIMotionClip 에셋을 생성합니다.
	2.	Duration과 Loop를 설정합니다.
	3.	Designer에서 애니메이션할 요소를 선택합니다.
	4.	Add Track From Selection을 누릅니다.
	5.	Property Track을 추가합니다.
	6.	원하는 시간으로 이동합니다.
	7.	Add Keyframe At Time을 누릅니다.
	8.	Time, Value, Easing을 수정합니다.
	9.	Play 또는 Time Slider로 결과를 확인합니다.

Runtime 재생

using emiteat.NexUI.Core;
using emiteat.NexUI.MotionClip;

await uiManager.PlayMotionClipAsync("Inventory", myClip);

직접 IUISurface와 IUIMotionClipPlayer를 사용할 수도 있습니다.

using emiteat.NexUI.MotionClip;

var surface = uiManager.GetSurface("Inventory");
var player = new UIMotionClipPlayer();

await player.PlayAsync(surface, myClip);



⸻

History

History 패널은 현재 에디터 세션에서 발생한 편집 작업을 최신순으로 표시합니다.

예시:

Edit NexUI Element Tint
Move NexUI Elements
Duplicate NexUI Elements
Change Parent

History는 읽기 전용 작업 로그입니다.

특정 기록으로 직접 이동하는 기능은 제공하지 않으며, 실제 실행 취소와 다시 실행은 Unity 기본 기능을 사용합니다.

Ctrl + Z
Ctrl + Y

창을 닫거나 Unity Editor를 재시작하면 History가 초기화됩니다.

⸻

언어 변경

Tools/NexUI/Designer/Language/Korean
Tools/NexUI/Designer/Language/English

언어를 변경하면 창을 다시 열지 않아도 UI가 즉시 갱신됩니다.

다음 요소가 로컬라이제이션됩니다.
	•	패널 이름
	•	Inspector 섹션
	•	필드 이름
	•	버튼
	•	메뉴
	•	툴팁
	•	컴포넌트 표시 이름

기술적 식별자로 사용되는 일부 상태 이름이나 타입 ID는 번역되지 않을 수 있습니다.

⸻

키보드 단축키

단축키	동작
Ctrl + A	전체 선택
Esc	선택 해제
Delete / Backspace	선택 요소 삭제
Ctrl + D	선택 요소 복제
Ctrl + C	복사
Ctrl + V	붙여넣기
Ctrl + G	그룹 생성
Ctrl + Shift + G	그룹 해제
방향키	1px 이동
Shift + 방향키	10px 이동
Ctrl + ]	한 단계 앞으로
Ctrl + [	한 단계 뒤로
Ctrl + Shift + ]	맨 앞으로
Ctrl + Shift + [	맨 뒤로
Alt + H	수평 균등 분배
Alt + V	수직 균등 분배
Alt + L	왼쪽 정렬
Alt + C	수평 가운데 정렬
Alt + R	오른쪽 정렬
Alt + T	위쪽 정렬
Alt + M	수직 가운데 정렬
Alt + B	아래쪽 정렬
F	선택 요소로 캔버스 이동
Ctrl + Z	실행 취소
Ctrl + Y	다시 실행

단축키 정의는 재바인딩 가능한 구조로 구현되어 있지만, 현재 별도의 단축키 설정 UI는 제공하지 않습니다.

⸻

주요 메뉴

Tools/NexUI/Designer
Tools/NexUI/Designer/Open Selected Screen
Tools/NexUI/Designer/Rebuild Preview
Tools/NexUI/Designer/Validate Current Screen
Tools/NexUI/Designer/Save Current Screen

Tools/NexUI/Designer/Motion Graph
Tools/NexUI/Designer/Motion Clip Editor

Tools/NexUI/Designer/Language/Korean
Tools/NexUI/Designer/Language/English

Tools/NexUI/Designer/Backend/Sync Metadata From Backend
Tools/NexUI/Designer/Backend/Apply Metadata To Preview
Tools/NexUI/Designer/Backend/Open Backend Asset In UI Builder
Tools/NexUI/Designer/Backend/Ping Backend Asset

Tools/NexUI/Designer/Advanced/Figma Bridge
Tools/NexUI/Migration Wizard



⸻

Figma Bridge

Tools/NexUI/Designer/Advanced/Figma Bridge

현재 지원 기능:
	•	Personal Access Token 저장
	•	프로젝트별 EditorPrefs 저장
	•	Figma API 연결 확인
	•	Figma File Key 입력
	•	원본 파일 JSON 가져오기

현재 지원하지 않는 기능:
	•	Figma Frame을 NexUI 요소로 자동 변환
	•	Auto Layout 자동 변환
	•	텍스트와 폰트 자동 매핑
	•	중첩 컴포넌트 변환
	•	이름 기반 Binding 자동 생성

Figma Bridge는 현재 연결과 원본 데이터 확인 단계입니다.

⸻

Migration Wizard

Tools/NexUI/Migration Wizard

패키지 ID 또는 네임스페이스가 변경되었을 때 프로젝트 파일에 남은 이전 참조를 검색하고 치환합니다.

지원 대상:
	•	C# Script
	•	Scene
	•	Prefab
	•	ScriptableObject 및 기타 Asset

변경할 파일을 선택해 일괄 처리할 수 있으며, 변경 전 원본은 .bak 파일로 백업됩니다.

⸻

패키지 구조

com.emiteat.nexui.designer/
├── Editor/
│   ├── Backend/
│   ├── Components/
│   ├── Core/
│   ├── Inspectors/
│   ├── Panels/
│   ├── Services/
│   ├── Validation/
│   └── Windows/
├── Runtime/
│   ├── Metadata/
│   └── MotionClip/
├── Samples~/
│   └── DesignerSample/
├── Documentation~/
├── package.json
└── README.md

Runtime

UnityEditor에 의존하지 않는 직렬화 가능한 데이터와 런타임 모션 기능을 포함합니다.
	•	Designer Metadata
	•	Layout Metadata
	•	Binding Metadata
	•	Theme Metadata
	•	Motion Metadata
	•	Motion Clip Runtime

Editor

Unity Editor에서만 동작하는 제작 기능을 포함합니다.
	•	Designer Window
	•	Component Registry
	•	Preview Renderer
	•	Inspector
	•	Backend Adapter
	•	Serializer
	•	Validation
	•	Motion Editor
	•	Migration Tool

런타임 코드와 에디터 코드를 분리하여 빌드에 불필요한 UnityEditor 의존성이 포함되지 않도록 구성되어 있습니다.

⸻

설계 구조

NexUI Designer는 중앙 컴포넌트 레지스트리를 컴포넌트 정의의 단일 진실 공급원으로 사용합니다.

각 컴포넌트 descriptor는 다음 정보를 정의합니다.
	•	Type ID
	•	Display Name
	•	Category
	•	Default Size
	•	Minimum Size
	•	Default Color
	•	Default Shape
	•	자식 허용 여부
	•	Container 여부
	•	지원 상태
	•	지원 Binding
	•	지원 Event
	•	Slot
	•	Accessibility Role
	•	uGUI 지원 수준
	•	UI Toolkit 지원 수준

Palette, Inspector, Preview, Validation, Hierarchy, Serializer가 동일한 descriptor를 참조하므로 각 시스템에 타입별 switch 문이 중복되는 것을 줄입니다.

등록되지 않은 사용자 정의 컴포넌트는 Generic descriptor로 처리되어 기존 데이터를 보존합니다.

⸻

현재 제한 사항

NexUI Designer는 현재 활발히 개발 중인 0.1.0 버전입니다.

Designer
	•	정렬 시 Figma의 Key Object 기준 정렬을 지원하지 않습니다.
	•	Alt + Drag 복제를 지원하지 않습니다.
	•	룰러와 가이드를 지원하지 않습니다.
	•	Column Grid Overlay를 지원하지 않습니다.
	•	단축키 재바인딩 UI가 없습니다.
	•	컴포넌트 인스턴스와 마스터 컴포넌트 시스템은 아직 없습니다.

Auto Layout / Constraints
	•	메타데이터 편집을 중심으로 지원합니다.
	•	Designer 캔버스에서 완전한 flexbox 동작을 재현하지 않습니다.
	•	UXML 또는 Prefab 구조를 자동 생성하지 않습니다.

UI Toolkit
	•	UXML 파일을 자동으로 다시 작성하지 않습니다.
	•	UXML 및 USS 제작은 UI Builder에서 수행해야 합니다.

Motion Graph
	•	복잡한 병렬 그룹이나 Sequence 시스템은 아직 제공하지 않습니다.
	•	기본적으로 선형 의존성 체인을 중심으로 동작합니다.

Motion Clip
	•	곡선 핸들 편집을 지원하지 않습니다.
	•	키프레임 다중 선택을 지원하지 않습니다.
	•	타임라인 스냅과 트랙 재정렬을 지원하지 않습니다.
	•	UIMotionClip과 Unity AnimationClip 사이의 Import/Export는 아직 구현되지 않았습니다.
	•	Motion Clip은 화면 메타데이터 저장 흐름에 자동으로 포함되지 않습니다.

Screen Graph
	•	Screen Graph 패널은 현재 기본 UI만 존재하며 실제 화면 전환 그래프 편집 기능은 구현 중입니다.

Figma Bridge
	•	Figma API 연결과 원본 데이터 확인만 지원합니다.
	•	Frame을 NexUI 요소로 변환하는 매핑 엔진은 아직 없습니다.

⸻

권장 작업 흐름
	1.	NexUI에서 UIScreenDefinition을 작성합니다.
	2.	화면에 대응하는 DesignerMetadataAsset을 생성합니다.
	3.	NexUI Designer를 열어 Screen과 Metadata를 연결합니다.
	4.	컴포넌트를 추가하고 부모-자식 계층을 구성합니다.
	5.	Layout과 Style을 편집합니다.
	6.	Preview State와 Preview Value로 각 상태를 확인합니다.
	7.	Binding, Theme, Localization, Accessibility를 설정합니다.
	8.	필요한 Motion을 작성합니다.
	9.	Validation 문제를 모두 수정합니다.
	10.	저장합니다.
	11.	Play Mode에서 실제 입력, 렌더링, 애니메이션을 검증합니다.

⸻

문서

패키지 내부의 Documentation~ 폴더에서 상세 문서를 확인할 수 있습니다.
	•	Overview
	•	Installation
	•	How to Use
	•	Core Concepts
	•	API Reference
	•	Development Guide
	•	Recipes
	•	Motion Graph Editor
	•	Motion Clip Editor

⸻

관련 저장소
	•	NexUI Runtime
	•	NexUI Designer

⸻

버전

현재 패키지 버전:

0.1.0

Unity Package Manager 식별자:

com.emiteat.nexui.designer

런타임 패키지 식별자:

com.emiteat.nexui



⸻

License

현재 저장소에는 별도의 라이선스가 명시되어 있지 않습니다.

외부 프로젝트에서 사용하거나 수정본을 재배포하기 전에 저장소 소유자의 사용 정책을 확인해 주세요.
:::

이 버전은 상단 소개부터 설치, 전체 기능, 저장 정책, 단축키, 아키텍처, 제한 사항까지 포함한 메인 README 용도다. 기존 README.md를 그대로 교체해도 된다.
