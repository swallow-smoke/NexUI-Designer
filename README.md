# NexUI Designer

NexUI Designer는 NexUI 화면을 Unity Editor에서 구성하는 시각적 제작 도구입니다. 화면 요소 배치, Hierarchy, Binding, Preview, Motion, Validation과 Backend 저장 흐름을 한곳에서 연결합니다.

NexUI Core가 화면 실행, 상태, Command와 Backend 계약을 담당한다면 Designer는 그 데이터를 제작하고 검증하는 Editor 패키지입니다. Designer만 설치해서 독립적인 런타임 UI 시스템으로 사용할 수는 없습니다.

## 핵심 제작 흐름

```text
Screen 준비 → Component 배치 → Hierarchy 구성 → Binding/Motion 연결
→ Validation → Save/생성 → Play Mode 확인
```

## 주요 특징

- uGUI와 UI Toolkit 화면을 같은 Metadata 모델로 편집합니다.
- Canvas 단일·다중 선택, Hierarchy, Auto Layout과 Constraints를 제공합니다.
- Binding 및 Command Key를 화면 Element에 연결합니다.
- Motion Clip, Motion Graph와 Motion State Machine 에셋을 편집하고 Preview합니다.
- 저장 전 참조, Backend 지원 범위와 Motion 데이터를 검증합니다.
- 한국어와 영어 Editor UI를 제공합니다.

> [!IMPORTANT]
> 프로젝트는 현재 개발 단계입니다. Backend별 지원 범위와 실험적 기능은 [현재 기능 상태](Documentation~/reference/feature-status.md)를 먼저 확인해 주세요.

## 요구 환경

- Unity `6000.4` 이상
- NexUI Core `0.1.0`
- UniTask `2.5.10`

## 문서

- [문서 홈](Documentation~/index.md)
- [설치](Documentation~/getting-started/installation.md)
- [10분 빠른 시작](Documentation~/getting-started/quick-start.md)
- [첫 화면 만들기](Documentation~/getting-started/first-screen.md)
- [현재 기능 상태](Documentation~/reference/feature-status.md)
- [목표 기능 명세](Documentation~/reference/feature-specification.md)
- [개발자 아키텍처](Documentation~/developer/architecture.md)

## 관련 프로젝트와 라이선스

- [NexUI Designer 저장소](https://github.com/swallow-smoke/NexUI-Designer)
- [NexUI Core 패키지](../com.emiteat.nexui/README.md)
- [MIT License](LICENSE.md)

