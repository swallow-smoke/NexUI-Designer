# 테스트와 검증

## 로컬 Batchmode

Windows 예시:

```powershell
& "<Unity>/Editor/Unity.exe" -batchmode -nographics -quit `
  -projectPath "<repo>" -runTests -testPlatform EditMode `
  -testResults "<repo>/TestResults/editmode.xml"
```

PlayMode는 `-testPlatform PlayMode`로 실행합니다. 같은 프로젝트를 Unity Editor에서 열어 둔 상태에서는 별도 Batchmode 실행이 실패할 수 있으므로 Editor를 닫거나 CI checkout에서 실행하십시오.

## 빠른 컴파일 확인

Unity가 `.csproj`를 갱신한 뒤 다음을 실행합니다.

```powershell
dotnet build emiteat.NexUI.Designer.Tests.EditMode.csproj --no-restore
```

이 명령은 Unity Test Runner를 실행하지 않고 C# 컴파일만 확인합니다.

## GitHub Actions

`.github/workflows/unity-tests.yml`은 PR, `master` push, 수동 실행에서 EditMode/PlayMode를 수행하고 결과를 artifact로 올립니다.

Personal License는 `UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD` secret이 필요합니다. Pro License는 `UNITY_EMAIL`, `UNITY_PASSWORD`, `UNITY_SERIAL`을 사용합니다. Secret이 없으면 GameCI 단계가 라이선스 오류로 실패합니다.

## 주요 테스트 범위

- Session Registry 등록/활성/해제/중복/파괴된 창
- VisualElement 구독 Activate/Detach/Reattach
- Motion Binding 저장/Reload/ID Rename/Validator
- Element 이동/부모 변경/Motion Binding Undo
- Generated UXML/USS 동일 콘텐츠/Marker/Dry Run/원자적 실패
- Settings/Inventory/ConfirmDialog/Loading/HUD Sample Smoke
- 기존 Metadata/Hierarchy/Preview/Generator/Scenario 테스트

## 수동 검증 체크리스트

- [ ] Unity Console 컴파일 오류 없음
- [ ] `Tools > NexUI > Designer` 열기와 한국어/영어 전환
- [ ] Screen/Metadata 연결, Preview Rebuild
- [ ] Component 추가, 선택·다중 선택·이동·크기 변경
- [ ] Reparent, Layer 순서, Group/Ungroup
- [ ] Undo/Redo 후 Preview와 Inspector 갱신
- [ ] Validation Issue 클릭 선택과 Asset Ping
- [ ] Save Report의 Changed/Skipped/Warning/Error 확인
- [ ] uGUI Prefab 저장 및 UI Toolkit Metadata/Generation 확인
- [ ] Motion Clip Scrub/Play/Undo와 Motion Graph Preview
- [ ] 창 재실행 및 Screen/선택/Scroll/Tab 복원
- [ ] Play Mode Binding, Command, Motion, Screen Open/Close
- [ ] Unity Console Runtime 오류 없음

Session, Metadata, Hierarchy, Validator, Generator와 Sample Load는 EditMode 자동화 대상입니다. 실제 Pointer 입력, Backend Player 결과, 폰트/Layout, Runtime Overlay와 Figma 네트워크는 수동 또는 별도 통합 환경이 필요합니다.
