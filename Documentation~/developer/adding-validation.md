# Validation 추가하기

`DesignerValidationService.Validate`는 `DesignerValidationIssue` 목록을 반환합니다. Issue에는 `Severity`, 안정적인 `Code`, `ScreenId`, `ElementId`, `Message`, `Fix`와 선택적인 `Asset`을 채웁니다.

좋지 않은 메시지:

```text
Invalid data.
```

좋은 메시지:

```text
Element 'submitButton'에 Command Key가 있지만 현재 Component는 Command Binding을 지원하지 않습니다.
```

Code는 테스트와 외부 보고가 참조하므로 문구를 고치는 이유로 변경하지 마세요. Element 문제는 클릭 선택이 가능하도록 `ElementId`를, Asset 문제는 Ping할 `Asset`을 지정합니다. 자동 Fix를 제공할 때는 Undo, Dirty와 다중 대상 안전성을 보장해야 합니다.

