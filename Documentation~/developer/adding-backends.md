# Backend 추가하기

새 Backend는 `INexUIDesignerBackend`를 구현하고 `DesignerBackendRegistry.Register`로 등록합니다. 계약에는 Preview Surface 생성/해제, Hierarchy, Selection Mapping, Element 생성·삭제·Reparent·Reorder, Position/Size/Anchor/Visibility/Name/Class/Binding과 Save가 포함됩니다.

1. Runtime `UIRenderBackend` 값과 해당 `IUISurface` 구현을 준비합니다.
2. Editor Preview Adapter에서 모든 Handle이 Stable Element ID를 반환하게 합니다.
3. Component Registry support와 Backend Validation을 추가합니다.
4. `IDesignerAssetSerializer`를 구현하고 `DesignerSerializerRegistry`에 연결합니다.
5. Save에서 기록하지 않은 값을 `DesignerSaveReport.Skipped` 또는 Warning으로 명시합니다.
6. Preview, Selection, 저장 실패와 Reload 테스트를 작성합니다.

uGUI는 `UGUIDesignerBackend`/`UGUIAssetSerializer`, UI Toolkit은 `UIToolkitDesignerBackend`/`UIToolkitAssetSerializer`를 참고하세요. Editor Backend가 Runtime의 게임 상태나 Command를 직접 실행해서는 안 됩니다.

