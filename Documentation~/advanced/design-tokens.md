# Design Token

디자인 토큰은 색상·간격·반경·모션 등 디자인 시스템의 값을 이름으로 정의하고 재사용하는 시스템입니다 (스펙 §33 · §37).

## 에셋

`DesignerTokenSetAsset`은 Theme과 같은 **독립 에셋**입니다.

- `Assets > Create > NexUI > Designer > Token Set` 또는 도구의 **토큰 세트 생성**으로 만듭니다.
- 각 토큰: 이름 + 카테고리 + 값(또는 다른 토큰 **참조**).

### 카테고리

`Color`, `Spacing`, `Radius`, `Border`, `Shadow`, `MotionDuration`, `MotionEasing`, `Breakpoint`, `ZIndex`, `Typography`.

- `Color` → 색상 값
- 숫자 계열(`Spacing`/`Radius`/`Border`/`MotionDuration`/`Breakpoint`/`ZIndex`) → 숫자 값
- 그 외(`MotionEasing`/`Typography`/`Shadow`) → 문자열 값

## 참조(별칭)

토큰은 다른 토큰을 **참조**해 별칭을 만들 수 있습니다. 예: `color.accent.primary` → `color.brand.blue`. 참조는 체인으로 이어질 수 있으며, 최종 리터럴 값으로 해석됩니다.

에디터는 각 참조 토큰의 **해석된 값**을 실시간으로 보여줍니다(색상은 스와치, 숫자는 값).

## 사용법

1. `Tools > NexUI > 유틸리티`에서 **Design Token**을 엽니다.
2. **토큰 추가**로 토큰을 만들고 이름·카테고리·값을 지정합니다.
3. **참조** 드롭다운에서 `(Literal)` 대신 다른 토큰을 선택하면 별칭이 됩니다.
4. **검증** 섹션이 문제를 나열합니다.

## 검증 (`DesignerTokenResolver.Validate`)

- **오류**: 중복 이름, 존재하지 않는 토큰 참조, 참조 순환.
- **경고**: 빈 이름, 다른 카테고리 토큰 참조.

## 아키텍처

- 데이터 `Runtime/Metadata/DesignerTokenSetAsset.cs`(Designer.Runtime).
- 해석·검증 코어 `Editor/Advanced/Tokens/DesignerTokenResolver.cs`는 **순수 클래스**(참조 해석은 순환 방지 포함)로 단위 테스트됩니다(`DesignerTokenResolverTests`).
- 에디터 `Editor/Advanced/Tokens/DesignerTokenWindow.cs`.

## 아직 하지 않는 것

토큰 값을 요소 스타일/테마에 **적용**(요소가 토큰을 참조하고 토큰 변경 시 즉시 갱신)하는 통합, 미사용 토큰 검사, 토큰 일괄 교체는 이후 단계입니다.
