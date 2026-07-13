# NexUI Designer

NexUI Designer는 Unity 안에서 NexUI 화면을 배치하고 Binding, Interaction, Motion을 연결한 뒤 uGUI 또는 UI Toolkit으로 저장·Publish하는 Editor 패키지입니다.

## 기본 작업 흐름

```text
화면 생성 → 요소 배치 → 부모/자식 구성 → Binding 설정
→ Motion Clip 연결 → 저장 → Reload → Backend Publish → Play Mode 확인
```

## 시작하기

1. `com.emiteat.nexui` Core를 설치합니다.
2. `com.emiteat.nexui.designer`를 설치합니다.
3. Package Manager에서 `Designer Sample`을 Import합니다.
4. `Tools > NexUI > Designer`를 엽니다.
5. Screen Definition과 Designer Metadata를 선택하거나 새로 만듭니다.

자세한 설치 순서는 [PackageInstallation](Documentation~/PackageInstallation.md)을 참고하십시오.

## 안정화된 구성

- 다중 Designer 창을 위한 Active Session Registry
- Panel Attach/Detach 기반 Context 이벤트 수명주기
- Motion Clip 에셋 참조를 포함하는 화면 Motion Metadata
- Element ID 변경 시 Parent/Focus/Variant/Motion 참조 동기화
- Undo/Redo 이후 Preview·Inspector·Validator 갱신
- Ctrl+S, Dirty 표시, 화면 전환/창 종료 저장 경고
- 원자적 UXML/USS 생성, Generated Marker 보호, Dry Run
- Motion target/clip/track/keyframe Validator
- 한국어/영어 Editor UI

## 문서

- [구현 상태](Documentation~/ImplementationStatus.md)
- [아키텍처](Documentation~/Architecture.md)
- [테스트와 CI](Documentation~/Testing.md)
- [설치](Documentation~/PackageInstallation.md)
- [사용법](Documentation~/how-to-use.md)
- [Motion Clip Editor](Documentation~/motion-clip-editor.md)
- [목표 기능 명세](Documentation~/FunctionList.md)

`FunctionList.md`는 목표 명세이며 실제 지원 여부는 `ImplementationStatus.md`를 기준으로 판단합니다.

## 요구 사항

- Unity 6000.4 이상
- `com.emiteat.nexui` 0.1.0
- uGUI 또는 UI Toolkit

## 검증

```powershell
dotnet build emiteat.NexUI.Designer.Tests.EditMode.csproj --no-restore
```

Unity Test Runner와 Batchmode 실행법은 [Testing](Documentation~/Testing.md)에 있습니다.
