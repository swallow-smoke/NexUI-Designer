NexUI Designer 기능 명세서

1. 프로젝트 관리

1.1 UI 화면 관리
	•	새 UI 화면 생성
	•	기존 UI 화면 열기
	•	UI 화면 복제
	•	UI 화면 이름 변경
	•	UI 화면 삭제
	•	최근 작업 화면 목록
	•	즐겨찾기 화면
	•	화면 검색
	•	화면 카테고리 분류
	•	화면 태그
	•	화면별 Backend 선택
	•	화면별 기준 해상도 설정
	•	화면별 Safe Area 설정
	•	화면별 UI Scale 설정
	•	화면별 Preview Scenario 설정
	•	화면별 기본 Theme 설정
	•	화면별 기본 Input Mode 설정

1.2 지원 Backend
	•	uGUI
	•	UI Toolkit
	•	Backend별 Preview
	•	Backend별 Capability 표시
	•	Backend별 지원하지 않는 속성 경고
	•	Backend별 Fallback 표시
	•	Backend 변경
	•	Backend 간 화면 변환
	•	Designer와 Backend 동기화
	•	Backend 변경 감지
	•	동기화 충돌 감지
	•	충돌 비교
	•	Designer 버전 사용
	•	Backend 버전 사용
	•	수동 병합

1.3 에셋 관리
	•	UIScreenDefinition 연결
	•	DesignerMetadataAsset 연결
	•	Motion Clip 연결
	•	Motion Graph 연결
	•	Theme 연결
	•	Component Definition 연결
	•	Scenario 연결
	•	Backend Asset 연결
	•	누락된 에셋 자동 탐색
	•	잘못된 참조 정리
	•	에셋 이름 자동 정리
	•	연관 에셋 일괄 이동
	•	연관 에셋 일괄 복제

⸻

2. 작업 공간

2.1 Workspace
	•	Design
	•	Bind
	•	Prototype
	•	Animate
	•	Graph
	•	Preview
	•	Debug

2.2 공통 Workspace 상태
	•	현재 화면 유지
	•	현재 선택 유지
	•	Zoom 유지
	•	Scroll 위치 유지
	•	Preview State 유지
	•	Input Mode 유지
	•	작업 패널 크기 유지
	•	패널 표시 상태 저장
	•	Workspace별 레이아웃 저장
	•	Workspace 초기화
	•	사용자 정의 Workspace 저장

2.3 공통 패널
	•	Component Library
	•	Hierarchy
	•	Canvas
	•	Inspector
	•	Align
	•	Layer
	•	Binding
	•	Interaction
	•	Timeline
	•	Graph
	•	Preview
	•	Validation
	•	History
	•	Runtime Debugger

⸻

3. Canvas 편집

3.1 선택
	•	단일 선택
	•	다중 선택
	•	Shift 추가 선택
	•	Ctrl 선택 토글
	•	박스 선택
	•	전체 선택
	•	선택 해제
	•	부모 선택
	•	직계 자식 선택
	•	모든 자손 선택
	•	동일 타입 선택
	•	동일 클래스 선택
	•	동일 컴포넌트 선택
	•	동일 Variant 선택
	•	겹친 요소 목록에서 선택
	•	선택 요소로 화면 이동
	•	선택 기록 복원
	•	Named Selection 저장

3.2 이동
	•	마우스 드래그 이동
	•	다중 요소 이동
	•	방향키 1px 이동
	•	Shift+방향키 단위 이동
	•	축 고정 이동
	•	Snap 이동
	•	부모 내부 이동
	•	부모 밖 이동
	•	다른 부모로 Drag 이동
	•	Layout 컨테이너 내부 순서 변경
	•	절대 위치 자식 설정

3.3 크기 변경
	•	Canvas Handle 크기 변경
	•	Inspector 수치 입력
	•	다중 요소 크기 변경
	•	비율 고정
	•	중앙 기준 크기 변경
	•	한쪽 축 크기 변경
	•	최소 크기 제한
	•	최대 크기 제한
	•	Hug Contents
	•	Fill Container
	•	Fixed Size

3.4 회전 및 Transform
	•	Rotation 편집
	•	Scale 편집
	•	Pivot 편집
	•	Anchor 편집
	•	Anchor Preset
	•	Transform 초기화
	•	위치 초기화
	•	크기 초기화
	•	회전 초기화
	•	Scale 초기화

3.5 복사와 생성
	•	복사
	•	붙여넣기
	•	잘라내기
	•	복제
	•	이동 간격 유지 복제
	•	Alt+Drag 복제
	•	같은 부모에 붙여넣기
	•	원래 위치에 붙여넣기
	•	스타일만 복사
	•	Binding만 복사
	•	Motion만 복사
	•	전체 속성 복사

3.6 삭제 및 이름 변경
	•	선택 요소 삭제
	•	자식 포함 삭제
	•	자식 유지 후 부모만 삭제
	•	빠른 이름 변경
	•	일괄 이름 변경
	•	ID 자동 생성
	•	ID Prefix 적용
	•	ID 중복 자동 해결
	•	구조 기반 ID 추천

⸻

4. 시각적 편집 도구

4.1 Grid
	•	Pixel Grid
	•	Column Grid
	•	Row Grid
	•	Baseline Grid
	•	사용자 정의 Grid
	•	Grid 표시
	•	Grid 숨김
	•	Grid Snap
	•	Grid 크기 변경
	•	Grid Offset 변경
	•	Column 개수
	•	Column Margin
	•	Column Gutter
	•	Column Width
	•	Stretch Column

4.2 Ruler와 Guide
	•	수평 Ruler
	•	수직 Ruler
	•	수평 Guide
	•	수직 Guide
	•	Ruler에서 Guide 생성
	•	Guide 이동
	•	Guide 잠금
	•	Guide 숨김
	•	Guide 삭제
	•	Guide 이름 설정
	•	Guide Snap
	•	전역 Guide
	•	화면별 Guide

4.3 Smart Guide
	•	부모 중앙 정렬 Guide
	•	형제 중앙 정렬 Guide
	•	Left 정렬 Guide
	•	Right 정렬 Guide
	•	Top 정렬 Guide
	•	Bottom 정렬 Guide
	•	Text Baseline Guide
	•	동일 간격 Guide
	•	동일 크기 Guide
	•	부모 Padding Guide
	•	Safe Area Guide
	•	Anchor Guide
	•	Smart Guide Snap

4.4 거리 측정
	•	부모와의 거리 표시
	•	형제 요소와의 거리 표시
	•	선택 요소 간 거리 표시
	•	Padding 표시
	•	Margin 표시
	•	Gap 표시
	•	Width/Height 표시
	•	Alt 거리 측정 모드

4.5 Canvas 보기
	•	Zoom In
	•	Zoom Out
	•	Zoom Reset
	•	Fit Screen
	•	Fit Selection
	•	실제 픽셀 보기
	•	Outline Mode
	•	Isolation Mode
	•	Layout Bounds 표시
	•	Clip Bounds 표시
	•	Safe Area 표시
	•	Anchor 표시
	•	Pivot 표시
	•	숨김 요소 표시
	•	잠금 요소 표시
	•	Backend 구조 표시

⸻

5. Hierarchy

5.1 계층 관리
	•	부모-자식 구조 표시
	•	Foldout
	•	Drag로 부모 변경
	•	Drag로 순서 변경
	•	Sibling Index 변경
	•	Parent Slot 변경
	•	Root 요소 표시
	•	계층 검색
	•	ID 검색
	•	이름 검색
	•	타입 검색
	•	클래스 검색

5.2 상태 제어
	•	요소 숨김
	•	요소 잠금
	•	자식 전체 숨김
	•	자식 전체 잠금
	•	Solo 표시
	•	펼치기
	•	모두 접기
	•	모두 펼치기

5.3 Breadcrumb
	•	현재 선택 경로 표시
	•	Breadcrumb 부모 선택
	•	Breadcrumb 자식 목록
	•	Component Instance 내부 경로 표시
	•	Isolation Mode 진입
	•	Main Component 이동

⸻

6. 정렬 및 레이어

6.1 정렬
	•	왼쪽 정렬
	•	수평 중앙 정렬
	•	오른쪽 정렬
	•	위쪽 정렬
	•	수직 중앙 정렬
	•	아래쪽 정렬
	•	부모 기준 정렬
	•	선택 Bounds 기준 정렬

6.2 분배
	•	수평 균등 분배
	•	수직 균등 분배
	•	고정 수평 간격
	•	고정 수직 간격
	•	행 정렬
	•	열 정렬

6.3 크기 통일
	•	동일 너비
	•	동일 높이
	•	동일 크기
	•	가장 큰 요소 기준
	•	가장 작은 요소 기준
	•	첫 선택 기준
	•	마지막 선택 기준

6.4 Layer
	•	한 단계 앞으로
	•	한 단계 뒤로
	•	맨 앞으로
	•	맨 뒤로
	•	Sibling Index 직접 입력
	•	Layer 목록
	•	Z-Index 표시
	•	Layer 잠금
	•	Layer 그룹

⸻

7. Group과 Container

7.1 Group
	•	선택 요소 Group
	•	Ungroup
	•	Group 이름 설정
	•	Group Bounds 자동 계산
	•	Group 해제 시 좌표 유지
	•	중첩 Group
	•	Group 잠금
	•	Group 숨김

7.2 Frame 및 Container
	•	Frame 생성
	•	Panel 생성
	•	Container 생성
	•	선택 요소를 Frame으로 감싸기
	•	Clip Children
	•	Content Padding
	•	Background
	•	Layout 적용
	•	Component Slot 적용

⸻

8. Layout 시스템

8.1 Layout 유형
	•	Free Layout
	•	Horizontal Layout
	•	Vertical Layout
	•	Grid Layout
	•	Wrap Layout
	•	Overlay Layout

8.2 공통 Layout 속성
	•	Direction
	•	Reverse
	•	Alignment
	•	Justification
	•	Gap
	•	Row Gap
	•	Column Gap
	•	Padding
	•	Margin
	•	Wrap
	•	Overflow
	•	Clip
	•	Grow
	•	Shrink
	•	Basis
	•	Min Size
	•	Max Size
	•	Preferred Size

8.3 크기 정책
	•	Fixed
	•	Hug Contents
	•	Fill Container
	•	Fit Content
	•	Stretch
	•	Percentage
	•	Aspect Ratio
	•	Min/Max Constraint

8.4 Grid Layout
	•	Fixed Columns
	•	Fixed Rows
	•	Auto Fit
	•	Auto Fill
	•	Cell Size
	•	Min Cell Size
	•	Max Cell Size
	•	Row Gap
	•	Column Gap
	•	Cell Alignment
	•	Column Span
	•	Row Span

8.5 Canvas Layout Handle
	•	Gap Handle
	•	Padding Handle
	•	Margin Handle
	•	Grid Cell Handle
	•	Layout 방향 변경
	•	Alignment 빠른 변경
	•	자식 순서 Drag 변경

⸻

9. 스타일 편집

9.1 Fill
	•	단색
	•	선형 Gradient
	•	방사형 Gradient
	•	각도 Gradient
	•	이미지
	•	Sprite
	•	Texture
	•	다중 Fill
	•	Fill 순서 변경
	•	Fill 활성화
	•	Fill 불투명도

9.2 Border
	•	전체 Border Width
	•	Side별 Border Width
	•	Border Color
	•	Corner별 Radius
	•	Inside Border
	•	Center Border
	•	Outside Border
	•	Dashed Border
	•	Dotted Border

9.3 Shadow
	•	Drop Shadow
	•	Inner Shadow
	•	다중 Shadow
	•	Offset
	•	Blur
	•	Spread
	•	Color
	•	Shadow 활성화
	•	Shadow 순서 변경

9.4 Opacity 및 Effect
	•	Opacity
	•	Blur
	•	Background Blur
	•	Glow
	•	Outline
	•	Grayscale
	•	Brightness
	•	Contrast
	•	Saturation
	•	Material
	•	Shader

9.5 Image
	•	Sprite 선택
	•	Texture 선택
	•	Preserve Aspect
	•	Fit
	•	Fill
	•	Crop
	•	Tile
	•	Slice
	•	Nine Slice
	•	Pixels Per Unit
	•	UV Offset
	•	UV Scale
	•	Flip Horizontal
	•	Flip Vertical
	•	Tint
	•	Fill Amount
	•	Fill Direction
	•	Fill Origin
	•	Clockwise

9.6 Text
	•	Text 내용
	•	Localization Key
	•	Font Asset
	•	Font Family
	•	Font Weight
	•	Font Style
	•	Font Size
	•	Auto Size
	•	Line Height
	•	Letter Spacing
	•	Word Spacing
	•	Paragraph Spacing
	•	Horizontal Alignment
	•	Vertical Alignment
	•	Wrapping
	•	Overflow
	•	Rich Text
	•	Text Color
	•	Outline
	•	Shadow
	•	Gradient

⸻

10. 컴포넌트 라이브러리

10.1 기본 컴포넌트
	•	Panel
	•	Container
	•	Card
	•	Modal
	•	Label
	•	Image
	•	Button
	•	Icon Button
	•	Choice List
	•	Toast
	•	Tooltip
	•	Popover
	•	Progress Bar
	•	Stat Bar
	•	Radial Fill
	•	Spinner
	•	Skeleton
	•	List
	•	Grid
	•	Slot
	•	Hotbar

10.2 라이브러리 기능
	•	컴포넌트 검색
	•	카테고리
	•	태그
	•	Thumbnail
	•	Preview
	•	즐겨찾기
	•	최근 사용
	•	Built-in
	•	Project
	•	Package
	•	Custom
	•	Variant Preview
	•	Drag & Drop 추가
	•	Quick Insert
	•	삽입 Slot Preview
	•	Component 교체

10.3 Quick Insert
	•	단축키로 검색창 열기
	•	이름 검색
	•	태그 검색
	•	한국어 별칭 검색
	•	영어 별칭 검색
	•	최근 항목
	•	즐겨찾기 항목
	•	선택 부모에 추가
	•	현재 위치에 추가

⸻

11. 사용자 정의 컴포넌트

11.1 Component 생성
	•	선택 요소에서 Component 생성
	•	Component 이름 지정
	•	카테고리 지정
	•	태그 지정
	•	Thumbnail 생성
	•	설명 작성
	•	Component Library 등록

11.2 Component Definition
	•	Root
	•	내부 Hierarchy
	•	Slot
	•	Exposed Property
	•	Variant
	•	기본 Binding
	•	기본 Interaction
	•	기본 Motion
	•	기본 Accessibility
	•	기본 Theme
	•	Preview
	•	Thumbnail

11.3 Exposed Property
	•	Text
	•	Integer
	•	Float
	•	Bool
	•	Color
	•	Sprite
	•	Texture
	•	Font
	•	Binding Key
	•	Command
	•	Motion Clip
	•	Motion Graph
	•	Variant
	•	Enum
	•	Object
	•	Element Reference

11.4 Component Slot
	•	Slot 이름
	•	허용 Component 타입
	•	최소 자식 수
	•	최대 자식 수
	•	Default Content
	•	Slot Layout
	•	Slot Validation
	•	Slot별 Padding
	•	Slot별 Clip

11.5 Component Instance
	•	원본 Component 연결
	•	Property Override
	•	Override 확인
	•	Override 초기화
	•	전체 Override 초기화
	•	Main Component 이동
	•	Main Component 변경 반영
	•	Instance 변경을 원본에 적용
	•	Component 교체
	•	Instance 분리
	•	Nested Component
	•	순환 참조 검사

⸻

12. Variant

12.1 Variant Property
	•	Bool
	•	Enum
	•	String
	•	Integer

12.2 Variant 변경 대상
	•	Hierarchy
	•	Visibility
	•	Layout
	•	Style
	•	Text
	•	Image
	•	Icon
	•	Binding
	•	Command
	•	Motion
	•	Accessibility
	•	Slot
	•	Component

12.3 Variant 기능
	•	Variant 추가
	•	Variant 삭제
	•	Variant 이름 변경
	•	Variant 조합 생성
	•	Variant 기본값
	•	Variant Preview
	•	Variant 일괄 비교
	•	Variant 교체
	•	Variant Transition
	•	Instance별 Variant 선택

⸻

13. Binding

13.1 Binding Channel
	•	Text Binding
	•	Value Binding
	•	Visibility Binding
	•	Interactable Binding
	•	Class Binding
	•	Image Binding
	•	Color Binding
	•	Variant Binding
	•	Collection Binding
	•	Selection Binding
	•	Command Binding

13.2 Binding Picker
	•	State Key 검색
	•	Command 검색
	•	타입 필터
	•	현재 화면 Scope
	•	전역 Scope
	•	Item Scope
	•	Graph Parameter
	•	Local State
	•	최근 Binding
	•	즐겨찾기 Binding

13.3 Binding 표시
	•	연결 상태 표시
	•	현재 Preview 값 표시
	•	타입 표시
	•	Source 표시
	•	마지막 갱신 시간
	•	잘못된 Binding 경고
	•	사용되지 않는 Binding 검사

13.4 Binding Expression
	•	사칙연산
	•	Compare
	•	Bool AND
	•	Bool OR
	•	Bool NOT
	•	Min
	•	Max
	•	Clamp
	•	Lerp
	•	Round
	•	Floor
	•	Ceil
	•	String Join
	•	String Format
	•	Null Check
	•	Conditional
	•	Collection Count
	•	Any
	•	All
	•	Color Lerp
	•	Vector 연산

13.5 Format Builder
	•	문자열 Template
	•	Named Argument
	•	숫자 Format
	•	소수점
	•	천 단위 구분
	•	Prefix
	•	Suffix
	•	Localization Argument

⸻

14. Collection UI

14.1 Collection Binding
	•	Source Binding
	•	Item Template
	•	Key Binding
	•	Selection Binding
	•	Empty View
	•	Loading View
	•	Error View
	•	Header Template
	•	Footer Template

14.2 Item Scope
	•	현재 Item
	•	Item Index
	•	Item Key
	•	선택 여부
	•	첫 번째 Item 여부
	•	마지막 Item 여부
	•	홀수/짝수 Index
	•	Parent Collection 정보

14.3 선택
	•	선택 없음
	•	단일 선택
	•	다중 선택
	•	범위 선택
	•	선택 해제
	•	기본 선택
	•	선택 변경 Command

14.4 정렬과 필터
	•	단일 정렬
	•	다중 정렬
	•	Ascending
	•	Descending
	•	조건 필터
	•	Text 검색
	•	Grouping
	•	Group Header
	•	Runtime Sort 변경
	•	Runtime Filter 변경

14.5 Virtualization
	•	Fixed Size
	•	Dynamic Size
	•	Recycling
	•	Pooling
	•	Overscan
	•	Pool Size
	•	가시 영역 계산
	•	Scroll Position 유지

14.6 Preview
	•	Preview Item Count
	•	Mock Item 생성
	•	Item Scope 값 설정
	•	Empty Preview
	•	Loading Preview
	•	Error Preview
	•	선택 Preview

⸻

15. Interaction

15.1 지원 Event
	•	Click
	•	Double Click
	•	Long Press
	•	Press
	•	Release
	•	Hover Enter
	•	Hover Exit
	•	Focus
	•	Blur
	•	Submit
	•	Cancel
	•	Drag Start
	•	Drag
	•	Drag End
	•	Drop
	•	Scroll
	•	Value Changed
	•	Selection Changed

15.2 기본 Action
	•	Open Screen
	•	Close Screen
	•	Push Screen
	•	Pop Screen
	•	Replace Screen
	•	Toggle Screen
	•	Dispatch Command
	•	Set Local State
	•	Toggle Visibility
	•	Show Element
	•	Hide Element
	•	Set Interactable
	•	Set Text
	•	Change Variant
	•	Play Motion Clip
	•	Trigger Motion Graph
	•	Show Tooltip
	•	Hide Tooltip
	•	Show Toast
	•	Set Focus
	•	Clear Focus
	•	Scroll Into View
	•	Play Sound

15.3 Interaction Row
	•	Event
	•	Condition
	•	Action
	•	Parameters
	•	Delay
	•	Once
	•	Consume Event
	•	Enabled
	•	순서 변경
	•	복제
	•	Graph로 변환

⸻

16. Drag & Drop
	•	Draggable 설정
	•	Drag Handle
	•	Drag Preview
	•	Drag Ghost
	•	Drag Offset
	•	Drag Threshold
	•	Allowed Drop Group
	•	Drop Target
	•	Drop Validation
	•	Drop Highlight
	•	Drop 성공 Action
	•	Drop 실패 Action
	•	원위치 복귀 Motion
	•	Drag 중 Auto Scroll
	•	Collection Item Drag
	•	Slot 간 이동
	•	Pointer 입력
	•	Touch 입력
	•	Gamepad 기반 이동

⸻

17. Tooltip

17.1 Trigger
	•	Hover
	•	Focus
	•	Long Press
	•	Manual

17.2 Placement
	•	Auto
	•	Top
	•	Bottom
	•	Left
	•	Right
	•	Cursor

17.3 옵션
	•	Show Delay
	•	Hide Delay
	•	Follow Cursor
	•	화면 내부 Clamp
	•	Safe Area Clamp
	•	Interactive
	•	최대 너비
	•	Dismiss Policy
	•	Pointer Offset
	•	Anchor Offset

17.4 Content
	•	Plain Text
	•	Rich Text
	•	Localization
	•	Image
	•	Item Preview
	•	Stat Block
	•	Key Hint
	•	비교 UI
	•	Custom Prefab
	•	Custom UXML

17.5 Motion
	•	등장 Motion
	•	퇴장 Motion
	•	Anchor 변경 Motion
	•	Tooltip Graph

⸻

18. Context Menu
	•	우클릭 Trigger
	•	Gamepad Menu Trigger
	•	메뉴 항목
	•	Icon
	•	Label
	•	Shortcut
	•	Command
	•	조건부 표시
	•	조건부 활성화
	•	Separator
	•	Submenu
	•	Tooltip
	•	선택 Item Scope
	•	외부 클릭 닫기
	•	Esc 닫기
	•	Focus Navigation

⸻

19. Motion Clip

19.1 Clip 관리
	•	Motion Clip 생성
	•	이름 변경
	•	복제
	•	삭제
	•	Duration
	•	FPS
	•	Loop
	•	Ping-Pong
	•	Playback Speed
	•	Time Scale 사용 여부
	•	Reduced Motion Clip
	•	Preview Target

19.2 트랙 유형

Transform
	•	Anchored Position
	•	Local Position
	•	Rotation
	•	Scale
	•	Size
	•	Pivot
	•	Anchor Min
	•	Anchor Max

Visual
	•	Opacity
	•	Background Color
	•	Text Color
	•	Border Color
	•	Border Width
	•	Border Radius
	•	Fill Amount
	•	Blur
	•	Brightness
	•	Saturation
	•	Material Float
	•	Material Color
	•	Material Vector

Layout
	•	Width
	•	Height
	•	Padding
	•	Margin
	•	Gap
	•	Flex Grow
	•	Flex Shrink
	•	Grid Gap
	•	Grid Cell Size

Text
	•	Text
	•	Font Size
	•	Letter Spacing
	•	Line Spacing
	•	Character Reveal
	•	Number Value

Event
	•	Command
	•	Graph Trigger
	•	Focus
	•	Tooltip
	•	Toast
	•	Sound
	•	Custom Event

19.3 Timeline
	•	Playhead
	•	시간 눈금
	•	Seconds 보기
	•	Frames 보기
	•	Seconds+Frames 보기
	•	Zoom
	•	Pan
	•	Work Area
	•	Marker
	•	Loop Region
	•	현재 시간 입력
	•	재생 범위 설정

19.4 Keyframe
	•	Key 추가
	•	Key 삭제
	•	Key 이동
	•	Key 복제
	•	Key 복사
	•	Key 붙여넣기
	•	다중 선택
	•	Box Selection
	•	Alt+Drag 복제
	•	Snap
	•	Timing Scale
	•	Reverse
	•	Mirror
	•	Ripple Edit
	•	Insert Time
	•	Delete Time
	•	Value 편집
	•	Time 편집
	•	Easing 편집

19.5 Snap
	•	Frame
	•	시간 간격
	•	다른 Key
	•	Playhead
	•	Clip 시작
	•	Clip 끝
	•	Marker
	•	Work Area

19.6 Easing
	•	Linear
	•	Step
	•	Smooth Step
	•	Quadratic
	•	Cubic
	•	Quartic
	•	Quintic
	•	Sine
	•	Exponential
	•	Circular
	•	Back
	•	Elastic
	•	Bounce
	•	Custom Curve
	•	Easing Preview
	•	Easing 용도 Tooltip

19.7 Curve Editor
	•	다중 Curve
	•	Axis 분리
	•	Bezier Handle
	•	Auto Tangent
	•	Linear Tangent
	•	Constant Tangent
	•	Broken Tangent
	•	Weighted Tangent
	•	Fit Selection
	•	Fit All
	•	Normalize
	•	Curve 복사
	•	Curve 붙여넣기

⸻

20. Motion 실시간 Preview
	•	Playhead Scrubbing
	•	실시간 Canvas 적용
	•	Play
	•	Pause
	•	Stop
	•	Loop
	•	Reverse Play
	•	Playback Speed
	•	Work Area Loop
	•	Preview Snapshot
	•	Stop 시 원본 복원
	•	Editor 종료 시 원본 복원
	•	Preview Target 상태 표시
	•	Backend 상태 표시
	•	Preview 오류 표시
	•	특정 Track Mute
	•	특정 Track Solo

⸻

21. Record와 Auto Key
	•	Record Mode
	•	Auto Key Off
	•	Existing Tracks
	•	All Changes
	•	Canvas 이동 Key 기록
	•	Canvas 크기 Key 기록
	•	Canvas 회전 Key 기록
	•	Canvas Scale Key 기록
	•	Inspector Color Key 기록
	•	Inspector Opacity Key 기록
	•	Inspector Fill Key 기록
	•	Inspector Font Size Key 기록
	•	기존 Key 수정
	•	새 Track 자동 생성
	•	Record 상태 표시
	•	Layout 편집과 Motion 편집 구분

⸻

22. Motion Path 및 Onion Skin

22.1 Motion Path
	•	Position Path 표시
	•	Key Point 표시
	•	Key Time 표시
	•	Path 방향 표시
	•	Bezier Handle
	•	Canvas에서 Key 이동
	•	Canvas에서 Key 삭제
	•	Tangent 편집
	•	일정 속도 정규화
	•	Path 따라 Rotation
	•	Path Target 변경

22.2 Onion Skin
	•	이전 Frame
	•	이전 Key
	•	현재 Frame
	•	다음 Key
	•	다음 Frame
	•	표시 개수
	•	Frame 간격
	•	색상
	•	불투명도

22.3 Split Preview
	•	두 시간 비교
	•	시작/끝 비교
	•	서로 다른 해상도 비교
	•	서로 다른 State 비교
	•	서로 다른 Theme 비교

⸻

23. Motion Layer
	•	Layer 생성
	•	Layer 삭제
	•	Layer 순서
	•	Weight
	•	Priority
	•	Enabled
	•	Mute
	•	Solo
	•	Override Blend
	•	Additive Blend
	•	Multiply Blend
	•	Relative Blend
	•	Property Mask
	•	Element Mask
	•	State별 Layer
	•	Runtime Layer 활성화
	•	Layer 충돌 검사

⸻

24. Motion State Machine

24.1 기본 State
	•	Normal
	•	Hover
	•	Pressed
	•	Focused
	•	Selected
	•	Disabled
	•	Loading
	•	Empty
	•	Error
	•	Success
	•	사용자 정의 State

24.2 Transition
	•	From State
	•	To State
	•	Any State
	•	Motion Clip
	•	Duration Override
	•	Condition
	•	Exit Time
	•	Priority

24.3 Interrupt Policy
	•	Restart
	•	Reverse
	•	Continue From Current
	•	Blend
	•	Queue
	•	Ignore
	•	Complete Immediately

24.4 Preview
	•	State Preview
	•	Transition Preview
	•	현재 State 표시
	•	현재 Transition 표시
	•	Binding 기반 State Preview
	•	Reduced Motion Preview

⸻

25. Motion Graph

25.1 Graph 관리
	•	Graph 생성
	•	Graph 복제
	•	Graph 삭제
	•	Graph 이름 변경
	•	Graph Parameter
	•	Graph Output
	•	Graph Validation
	•	Graph Template
	•	Subgraph

25.2 Port 타입
	•	Flow
	•	Bool
	•	Integer
	•	Float
	•	String
	•	Vector2
	•	Vector3
	•	Color
	•	Element
	•	Element List
	•	Screen
	•	Motion Clip
	•	Motion Graph
	•	Command
	•	Sprite
	•	Audio
	•	Object

25.3 Event Node

Screen
	•	On Screen Create
	•	On Screen Open
	•	On Screen Opened
	•	On Screen Close Requested
	•	On Screen Closing
	•	On Screen Closed
	•	On Screen Covered
	•	On Screen Revealed
	•	On Screen Focused
	•	On Screen Unfocused

Element
	•	On Click
	•	On Double Click
	•	On Long Press
	•	On Press
	•	On Release
	•	On Hover Enter
	•	On Hover Exit
	•	On Drag Start
	•	On Drag
	•	On Drag End
	•	On Drop
	•	On Scroll
	•	On Focus
	•	On Blur
	•	On Submit
	•	On Cancel
	•	On Value Changed
	•	On Selection Changed

Binding
	•	On Binding Changed
	•	On Bool True
	•	On Bool False
	•	On Value Above
	•	On Value Below
	•	On Collection Changed
	•	On Item Added
	•	On Item Removed

Environment
	•	On Resolution Changed
	•	On Safe Area Changed
	•	On Orientation Changed
	•	On Input Device Changed
	•	On Language Changed
	•	On Theme Changed

Custom
	•	Custom Event
	•	Trigger Graph

25.4 Flow Node
	•	Sequence
	•	Parallel
	•	Branch
	•	Switch
	•	Delay
	•	Wait Until
	•	Wait While
	•	Repeat
	•	Loop
	•	For Each
	•	Stagger
	•	Race
	•	Timeout
	•	Gate
	•	Once
	•	Debounce
	•	Throttle
	•	Cooldown
	•	Queue
	•	Cancel
	•	Try
	•	Catch
	•	Finally
	•	Fallback

25.5 Motion Node
	•	Move
	•	Rotate
	•	Scale
	•	Resize
	•	Fade
	•	Color
	•	Text Color
	•	Fill Amount
	•	Blur
	•	Shader Property
	•	Shake
	•	Punch Scale
	•	Pulse
	•	Flash
	•	Blink
	•	Bounce
	•	Spring
	•	Typewriter
	•	Text Scramble
	•	Number Count
	•	Progress Animate
	•	Radial Fill Animate
	•	Slot Highlight
	•	Cooldown Sweep

25.6 Motion Clip Node
	•	Play Motion Clip
	•	Stop Motion Clip
	•	Pause Motion Clip
	•	Resume Motion Clip
	•	Reverse Motion Clip
	•	Set Clip Time
	•	Set Clip Speed

25.7 UI Node
	•	Show Element
	•	Hide Element
	•	Toggle Visibility
	•	Set Interactable
	•	Set Text
	•	Set Class
	•	Add Class
	•	Remove Class
	•	Change Variant
	•	Set Focus
	•	Clear Focus
	•	Scroll Into View
	•	Show Tooltip
	•	Hide Tooltip
	•	Show Toast
	•	Open Screen
	•	Close Screen
	•	Push Screen
	•	Pop Screen
	•	Replace Screen

25.8 Command Node
	•	Dispatch Command
	•	Payload Builder
	•	Wait Result
	•	Success
	•	Failed
	•	Completed
	•	Timeout

⸻

26. Graph 대상 선택
	•	Element Picker
	•	Current Event Target
	•	Current Selected Element
	•	Current Focused Element
	•	Current Screen
	•	Parent
	•	Children
	•	Descendants
	•	Siblings
	•	Element List
	•	Element Query
	•	Class Query
	•	Type Query
	•	Slot Query
	•	Component Query

⸻

27. Graph Blackboard

27.1 Parameter
	•	Bool
	•	Integer
	•	Float
	•	String
	•	Vector
	•	Color
	•	Element
	•	Element List
	•	Motion Clip
	•	Screen
	•	Object

27.2 Local Variable
	•	생성
	•	삭제
	•	이름 변경
	•	기본값
	•	Runtime 값 표시
	•	Read
	•	Write

27.3 Built-in Variable
	•	Current Event Target
	•	Current Screen
	•	Current Focus
	•	Current Input Device
	•	Delta Time
	•	Unscaled Delta Time
	•	Current Item
	•	Current Index

27.4 입력 Source
	•	Constant
	•	Binding
	•	Parameter
	•	Variable
	•	Node Output
	•	Expression

⸻

28. Graph 구조화
	•	Comment Box
	•	Group
	•	Reroute Node
	•	Portal
	•	Subgraph
	•	Extract To Subgraph
	•	Collapse To Node
	•	Node 검색
	•	Minimap
	•	Breadcrumb
	•	Bookmark
	•	Node Category
	•	Node 색상
	•	Node 설명
	•	Node Documentation
	•	자동 정렬
	•	선택 정렬
	•	선택 분배

⸻

29. Graph Preview와 Debug

29.1 Editor Preview
	•	Preview Node
	•	Preview Selection
	•	Preview From Here
	•	Preview To Here
	•	Preview Graph
	•	Reset Preview
	•	실행 중 Node 표시
	•	완료 Node 표시
	•	실패 Node 표시
	•	취소 Node 표시
	•	실행 연결선 표시

29.2 Runtime Debug
	•	Graph Instance 목록
	•	실행 Node 표시
	•	Execution Count
	•	Last Duration
	•	Input Value
	•	Output Value
	•	Error 표시
	•	Breakpoint
	•	Pause Graph
	•	Step Into
	•	Step Over
	•	Continue
	•	Stop

29.3 중첩 정책
	•	Ignore New
	•	Restart
	•	Continue From Current
	•	Reverse Current
	•	Complete Current
	•	Queue
	•	Run In Parallel
	•	Blend

⸻

30. 텍스트 Motion

30.1 Typewriter
	•	문자 단위
	•	단어 단위
	•	줄 단위
	•	문장 단위
	•	구두점 Delay
	•	Rich Text 처리
	•	Click To Complete
	•	Typing Sound
	•	Skip

30.2 Text Scramble
	•	Random Characters
	•	Number Only
	•	Glitch Characters
	•	Left To Right
	•	Right To Left
	•	Random Decode

30.3 Number Count
	•	Integer
	•	Float
	•	Count Up
	•	Count Down
	•	Prefix
	•	Suffix
	•	천 단위 구분
	•	Decimal Places

30.4 Character Motion
	•	Position
	•	Rotation
	•	Scale
	•	Color
	•	Opacity
	•	Wave
	•	Shake
	•	Delay
	•	Left To Right
	•	Right To Left
	•	Center Out
	•	Random

⸻

31. Audio, Haptic, VFX

31.1 Audio
	•	Play Sound
	•	Stop Sound
	•	Pause Sound
	•	Resume Sound
	•	Volume
	•	Pitch
	•	Delay
	•	Audio Adapter
	•	Unity Audio
	•	FMOD Adapter
	•	Custom Adapter

31.2 Haptic
	•	Light
	•	Medium
	•	Heavy
	•	Custom
	•	플랫폼 지원 검사

31.3 UI VFX
	•	Spawn Effect
	•	Stop Effect
	•	Particle
	•	Click Burst
	•	Glow
	•	Trail
	•	Afterimage
	•	Shader Property
	•	Material Property
	•	Dissolve
	•	Glitch
	•	Blur Transition

⸻

32. Focus Navigation

32.1 Focus 설정
	•	Default Focus
	•	Up
	•	Down
	•	Left
	•	Right
	•	Submit
	•	Cancel
	•	Focus Scope
	•	Focus Trap
	•	Focus Restore
	•	Wrap Around

32.2 자동 Navigation
	•	Nearest
	•	Grid
	•	Angle Weighted
	•	Strict Row
	•	Strict Column
	•	Wrap
	•	제외 요소 설정

32.3 Focus Preview
	•	Focus 이동 시뮬레이션
	•	현재 Focus 표시
	•	방향 연결 표시
	•	접근 불가능한 요소 표시
	•	Focus History 표시
	•	Focus Motion
	•	Focus Sound
	•	Focus Tooltip
	•	Scroll Into View

⸻

33. Screen Flow

33.1 Node
	•	Screen
	•	Overlay
	•	Modal
	•	Persistent HUD
	•	Subflow

33.2 Transition
	•	Push
	•	Pop
	•	Replace
	•	Overlay
	•	Modal
	•	Return

33.3 Transition 설정
	•	Motion
	•	Guard
	•	Input Blocking
	•	Pause Policy
	•	Focus Policy
	•	Time Policy
	•	Transition Duration
	•	Cancel Policy

33.4 Deep Link
	•	특정 화면 열기
	•	특정 Tab 열기
	•	특정 Element Focus
	•	Parameter 전달
	•	Return 경로

⸻

34. Preview

34.1 Component State
	•	Normal
	•	Hover
	•	Pressed
	•	Focused
	•	Disabled
	•	Selected
	•	Loading
	•	Empty
	•	Error
	•	Success
	•	Indeterminate

34.2 Mock State
	•	Binding 값 입력
	•	Bool Toggle
	•	Number Slider
	•	String 입력
	•	Enum 선택
	•	Color 선택
	•	Sprite 선택
	•	Collection 생성
	•	값 고정
	•	값 초기화

34.3 Device Preview
	•	Desktop 16:9
	•	QHD
	•	4K
	•	Ultrawide
	•	Steam Deck
	•	Mobile Portrait
	•	Mobile Landscape
	•	Console Safe Area
	•	Custom Resolution

34.4 환경
	•	DPI
	•	UI Scale
	•	Safe Area
	•	Input Device
	•	Language
	•	Font Scale
	•	Theme
	•	Reduced Motion
	•	Color Blind Mode

34.5 Input Simulation
	•	Mouse
	•	Keyboard
	•	Gamepad
	•	Touch
	•	Pointer Hover
	•	Click
	•	Submit
	•	Cancel
	•	Navigation
	•	Drag
	•	Scroll

⸻

35. Scenario

35.1 Scenario Asset
	•	Binding Values
	•	Input Device
	•	Resolution
	•	DPI
	•	Safe Area
	•	Language
	•	Theme
	•	Focus
	•	Screen Stack
	•	Component State
	•	Triggered Event

35.2 Scenario Timeline
	•	시간별 Binding 변경
	•	시간별 Event 실행
	•	시간별 Input 실행
	•	시간별 화면 전환
	•	Motion과 Graph 반응 확인
	•	Timeline 재생
	•	Timeline Scrubbing
	•	Loop

35.3 Interaction Recording
	•	사용자 입력 기록
	•	화면 열기 기록
	•	클릭 기록
	•	Focus 이동 기록
	•	Drag 기록
	•	Command 기록
	•	Scenario 저장
	•	Scenario 재생
	•	회귀 테스트

⸻

36. 반응형 UI

36.1 Breakpoint
	•	Width 조건
	•	Height 조건
	•	Aspect 조건
	•	Orientation 조건
	•	Device 조건
	•	Input Device 조건

36.2 Breakpoint Override
	•	Visibility
	•	Position
	•	Size
	•	Layout
	•	Layout Direction
	•	Padding
	•	Margin
	•	Gap
	•	Grid Columns
	•	Font Size
	•	Component Variant
	•	Motion
	•	Navigation

36.3 동시 Preview
	•	여러 Resolution 동시 표시
	•	Breakpoint 비교
	•	Theme 비교
	•	언어 비교
	•	Safe Area 비교
	•	Overflow 검사

⸻

37. Theme 및 Design Token

37.1 Token 타입
	•	Color
	•	Typography
	•	Spacing
	•	Radius
	•	Border
	•	Shadow
	•	Motion Duration
	•	Motion Easing
	•	Breakpoint
	•	Z-Index

37.2 Token 기능
	•	Token 생성
	•	Token 삭제
	•	Token 이름 변경
	•	Token 그룹
	•	Token 별칭
	•	Token Reference
	•	Token Override
	•	사용 위치 검색
	•	미사용 Token 검사
	•	Token 일괄 교체

37.3 Theme
	•	Theme 생성
	•	Theme 복제
	•	Theme 전환
	•	Theme 상속
	•	Theme Override
	•	Default
	•	Dark
	•	High Contrast
	•	사용자 정의 Theme
	•	Theme 동시 Preview

⸻

38. Accessibility
	•	Accessibility Label
	•	Accessibility Description
	•	Accessibility Role
	•	읽기 순서
	•	Screen Reader Preview
	•	대비 검사
	•	색각 Preview
	•	Font Scale Preview
	•	Touch Target 검사
	•	Keyboard 접근성 검사
	•	Gamepad 접근성 검사
	•	Focus Trap 검사
	•	Reduced Motion
	•	Hold 입력 대체
	•	Double Click 대체
	•	접근성 Validation

⸻

39. Backend 출력

39.1 uGUI 생성
	•	GameObject
	•	RectTransform
	•	CanvasGroup
	•	Image
	•	TMP_Text
	•	Button
	•	Toggle
	•	Slider
	•	ScrollRect
	•	HorizontalLayoutGroup
	•	VerticalLayoutGroup
	•	GridLayoutGroup
	•	ContentSizeFitter
	•	LayoutElement
	•	Mask
	•	RectMask2D
	•	Prefab 생성
	•	Prefab 갱신
	•	부모-자식 반영
	•	Sibling 반영
	•	Component 설정 반영

39.2 UI Toolkit 생성
	•	UXML 생성
	•	UXML 갱신
	•	USS 생성
	•	USS 갱신
	•	Element Type
	•	name
	•	class
	•	Hierarchy
	•	Position
	•	Size
	•	Flex
	•	Padding
	•	Margin
	•	Gap
	•	Border
	•	Radius
	•	Text Style
	•	Background
	•	Opacity
	•	Display
	•	Overflow

39.3 Publish
	•	현재 화면 Publish
	•	모든 화면 Publish
	•	변경된 화면만 Publish
	•	Dry Run
	•	Build Report
	•	생성된 Asset 목록
	•	경고 목록
	•	오류 목록
	•	변경 Diff
	•	자동 Backup

⸻

40. Runtime Live Edit
	•	Play Mode 연결
	•	Runtime Screen 목록
	•	Runtime Element 선택
	•	Game View에서 UI Pick
	•	Runtime 값을 Designer에 표시
	•	Runtime Instance 편집
	•	Preview Only
	•	Runtime Instance 적용
	•	Source Asset 적용
	•	Override 생성
	•	Revert
	•	Binding 값 고정
	•	Runtime Motion 재생
	•	Runtime Graph Trigger
	•	Runtime Focus 변경

⸻

41. Runtime Debugger

41.1 Screen Tree
	•	열린 Screen
	•	Stack 순서
	•	Layer
	•	Modal
	•	Persistent
	•	Input Owner
	•	Focus Owner
	•	화면 상태
	•	생성 시간

41.2 Binding Trace
	•	Binding Source
	•	State Store
	•	Binding Target
	•	현재 값
	•	타입
	•	갱신 시간
	•	갱신 Source
	•	오류

41.3 Command Log
	•	Command 이름
	•	Source Element
	•	Payload
	•	결과
	•	Duration
	•	오류
	•	Timestamp

41.4 Motion Profiler
	•	실행 중 Motion
	•	Active Layer
	•	Clip Time
	•	Graph Instance
	•	Evaluation Time
	•	Layout Rebuild
	•	Allocation
	•	변경 요소 수
	•	중첩 Motion 수

41.5 Graph Trace
	•	실행 경로
	•	Node 상태
	•	Input 값
	•	Output 값
	•	실행 횟수
	•	실행 시간
	•	Error

⸻

42. Validation

42.1 화면 구조
	•	Screen 누락
	•	Metadata 누락
	•	중복 Element ID
	•	빈 Element ID
	•	Parent 누락
	•	자기 자신 Parent
	•	순환 Parent
	•	잘못된 Slot
	•	자식 미지원
	•	Slot 자식 수 초과
	•	중복 Sibling Index

42.2 Layout
	•	잘못된 크기
	•	음수 크기
	•	Safe Area 이탈
	•	화면 밖 요소
	•	Text Overflow
	•	Layout 충돌
	•	Breakpoint 누락

42.3 Binding
	•	State Key 누락
	•	Command 누락
	•	타입 불일치
	•	미지원 Channel
	•	잘못된 Expression
	•	Item Scope 오류
	•	Collection Key 누락

42.4 Component
	•	Component 순환 참조
	•	누락된 Main Component
	•	잘못된 Override
	•	누락된 Variant
	•	잘못된 Slot Content

42.5 Motion
	•	Target 누락
	•	Track 누락
	•	미지원 Property
	•	잘못된 Key Time
	•	Empty Clip
	•	Layer 충돌
	•	Reduced Motion 누락

42.6 Graph
	•	필수 Port 연결 누락
	•	타입 불일치
	•	도달 불가능한 Node
	•	Entry Event 없음
	•	무한 Flow
	•	Subgraph 누락
	•	Command Payload 오류
	•	안전하지 않은 State Write

42.7 Navigation
	•	Default Focus 누락
	•	Focus Trap
	•	접근 불가능 요소
	•	끊어진 방향 연결
	•	Offscreen Focus

42.8 Accessibility
	•	Label 누락
	•	Role 오류
	•	낮은 대비
	•	작은 Touch Target
	•	Keyboard 접근 불가
	•	Gamepad 접근 불가

42.9 Validation UX
	•	Error
	•	Warning
	•	Info
	•	요소로 이동
	•	Graph Node로 이동
	•	관련 Workspace 열기
	•	자동 수정
	•	모두 자동 수정
	•	무시 규칙
	•	사용자 정의 Validation

⸻

43. History와 Undo
	•	Unity Undo
	•	Unity Redo
	•	Command History
	•	작업 이름
	•	작업 시간
	•	대상 요소
	•	History 검색
	•	History 필터
	•	Metadata Dirty
	•	Backend Dirty
	•	Preview Dirty
	•	저장 지점 표시

⸻

44. 검색과 일괄 편집

44.1 검색
	•	Element ID
	•	이름
	•	Component 타입
	•	Class
	•	Binding
	•	Command
	•	Theme Token
	•	Variant
	•	Font
	•	Sprite
	•	Motion Clip
	•	Motion Graph

44.2 Replace
	•	ID Replace
	•	Class Replace
	•	Binding Replace
	•	Command Replace
	•	Token Replace
	•	Component Replace
	•	Variant Replace
	•	Font Replace
	•	Sprite Replace

44.3 일괄 편집
	•	Style 일괄 변경
	•	Binding 일괄 변경
	•	Theme 일괄 변경
	•	Accessibility 일괄 변경
	•	Component 일괄 교체
	•	Variant 일괄 변경

⸻

45. Command Palette와 단축키

45.1 Command Palette
	•	기능 검색
	•	화면 검색
	•	요소 검색
	•	Component 추가
	•	Workspace 전환
	•	Motion Clip 생성
	•	Graph 생성
	•	Validation
	•	Publish
	•	Preview
	•	Runtime Debug

45.2 단축키
	•	전체 선택
	•	선택 해제
	•	복사
	•	붙여넣기
	•	복제
	•	삭제
	•	Group
	•	Ungroup
	•	Layer 이동
	•	정렬
	•	분배
	•	Focus Selection
	•	Quick Insert
	•	Command Palette
	•	Play
	•	Stop
	•	Add Key
	•	Record
	•	Save
	•	Validate

45.3 재설정
	•	단축키 변경
	•	충돌 검사
	•	기본값 복원
	•	사용자 Profile 저장

⸻

46. Localization
	•	Designer UI 한국어
	•	Designer UI 영어
	•	실시간 언어 전환
	•	Tooltip 번역
	•	Component 이름 번역
	•	Validation 메시지 번역
	•	Command Palette 별칭
	•	Localization Key Preview
	•	언어별 Text Overflow 검사
	•	언어별 Font 설정
	•	RTL Preview

⸻

47. Template

47.1 화면 Template
	•	Main Menu
	•	Pause Menu
	•	Settings
	•	Player HUD
	•	Inventory
	•	Hotbar
	•	Equipment
	•	Crafting
	•	Quest Log
	•	Dialogue
	•	Loading Screen
	•	Save Select
	•	Confirmation Modal
	•	Tooltip
	•	Toast Stack
	•	Boss HUD
	•	Result Screen

47.2 Template 포함 데이터
	•	Hierarchy
	•	Layout
	•	Component
	•	Binding 이름
	•	Command 이름
	•	Motion
	•	Focus Navigation
	•	Accessibility
	•	Scenario
	•	Theme

47.3 Template 관리
	•	Template 생성
	•	Template 복제
	•	Template 삭제
	•	Project Template
	•	Package Template
	•	Template Import
	•	Template Export
	•	UI Kit Import
	•	UI Kit Export

⸻

48. 확장 API
	•	Custom Component
	•	Component Descriptor
	•	Custom Inspector
	•	Custom Slot
	•	Custom Binding Channel
	•	Custom Motion Property
	•	Custom Motion Node
	•	Custom Graph Node
	•	Custom Validation Rule
	•	Custom Backend Adapter
	•	Custom Audio Adapter
	•	Custom Tooltip Content
	•	Custom Template
	•	Custom Theme Token
	•	Custom Preview Provider
	•	자동 Registry 등록
	•	Generic Fallback

⸻

49. Figma Bridge
	•	Personal Access Token
	•	File Key
	•	Figma API 연결
	•	File 정보 조회
	•	Node 조회
	•	Raw JSON 표시
	•	Frame 선택
	•	Image Reference 조회
	•	연결 상태 표시

⸻

50. Migration
	•	이전 Metadata 버전 감지
	•	Metadata 자동 Migration
	•	Motion Clip Migration
	•	Motion Graph Migration
	•	Component Definition Migration
	•	Migration Preview
	•	Backup 생성
	•	변경 내용 표시
	•	일괄 Migration
	•	실패 Rollback

⸻

51. 테스트 및 샘플

51.1 샘플
	•	Main Menu
	•	Inventory
	•	Hotbar
	•	HUD
	•	Modal
	•	Tooltip
	•	Button Motion
	•	Grid Stagger
	•	Focus Navigation
	•	Collection Binding
	•	Motion Graph
	•	Runtime Debug

51.2 EditMode 테스트
	•	Metadata 직렬화
	•	계층
	•	Slot
	•	Undo/Redo
	•	Motion 평가
	•	Key 보간
	•	Curve 평가
	•	Graph Port
	•	Graph 실행
	•	Scenario
	•	Backend 생성

51.3 Integration 테스트
	•	Designer에서 uGUI 생성
	•	Designer에서 UXML/USS 생성
	•	Backend 동기화
	•	Motion Preview 복원
	•	Graph Preview
	•	Binding
	•	Command
	•	Focus Navigation
	•	Runtime Live Edit
