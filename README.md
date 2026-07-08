# NexUI Designer

NexUI Designer는 NexUI 화면을 시각적으로 제작하고 검토하기 위한 Unity Editor 확장 패키지입니다.

이 패키지는 `com.emiteat.nexui`에 의존하지만, 에디터 전용 도구와 런타임 UI 코드를 분리합니다. 런타임 화면, 상태, 모션, 테마, 백엔드 계약은 런타임 패키지에 두고, 디자인용 패널, 메타데이터 편집, 검증, 프리뷰 도구는 이 패키지에서 다룹니다.

## 요구 사항

- Unity 2022.3 이상
- `com.emiteat.nexui`
- Unity Editor의 UI Toolkit 지원

## 열기

```text
Tools/NexUI/Designer
```

자주 쓰는 명령:

- `Tools/NexUI/Designer/Open Selected Screen`
- `Tools/NexUI/Designer/Rebuild Preview`
- `Tools/NexUI/Designer/Validate Current Screen`
- `Tools/NexUI/Designer/Save Current Screen`
- `Tools/NexUI/Designer/Language/Korean`
- `Tools/NexUI/Designer/Language/English`

## 기능

- `UIScreenDefinition` 에셋을 위한 시각적 작업 공간
- UI Toolkit과 uGUI를 위한 디자이너 백엔드 추상화
- 계층, 뷰포트, 인스펙터, 검증, 상태, 커맨드, 화면 그래프 패널
- element id, binding, localization link, variant, validation hint를 저장하는 디자이너 메타데이터
- snapshot, diff, cleanup, profiling, contract, responsive rule, refactoring을 위한 에디터 서비스
- 한국어/영어 에디터 UI 문자열

## 문서

- [개요](Documentation~/index.md)
- [설치](Documentation~/installation.md)
- [빠른 시작](Documentation~/how-to-use.md)
- [핵심 개념](Documentation~/concepts.md)
- [API 레퍼런스](Documentation~/api-reference.md)
- [개발 가이드](Documentation~/development.md)
- [레시피](Documentation~/recipes.md)
