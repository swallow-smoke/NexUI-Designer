# NexUI Designer

NexUI Designer는 런타임 패키지에 에디터 전용 코드를 넣지 않고 NexUI 화면을 제작하고 검토하기 위한 Unity Editor 패키지입니다.

`UIScreenDefinition`을 선택하고, 디자이너 메타데이터를 확인하고, 화면 프리뷰를 갱신하고, 자주 발생하는 제작 실수를 검증하고, 백엔드별 저장 로직으로 변경 사항을 저장할 수 있습니다.

## 문서

- [설치](installation.md)
- [빠른 시작](how-to-use.md)
- [핵심 개념](concepts.md)
- [API 레퍼런스](api-reference.md)
- [개발 가이드](development.md)
- [레시피](recipes.md)

## 패키지 구조

```text
com.emiteat.nexui.designer/
  Editor/             Unity Editor 창, 패널, 도구, 검증기, 서비스
  Runtime/Metadata/   직렬화 가능한 디자이너 메타데이터 에셋
  Samples~/           선택해서 가져올 수 있는 샘플 에셋과 샘플 스크립트
  Documentation~/     패키지 문서
```

## 요구 사항

- Unity 2022.3 이상
- `com.emiteat.nexui`
- Unity Editor의 UI Toolkit 패키지 지원

## 진입점

- `Tools/NexUI/Designer`
- `Tools/NexUI/Designer/Open Selected Screen`
- `Tools/NexUI/Designer/Rebuild Preview`
- `Tools/NexUI/Designer/Validate Current Screen`
- `Tools/NexUI/Designer/Save Current Screen`

## 현재 범위

이 패키지는 에디터 셸, 메타데이터 모델, 백엔드 추상화, 검증 패널, 프리뷰 패널, 로컬라이제이션 헬퍼, 프로파일링 헬퍼, 확장 지점을 제공합니다. 실제 런타임 렌더링은 대체하지 않으며, 런타임 동작은 `com.emiteat.nexui`에 남겨둡니다.
