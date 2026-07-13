# Upgrading

NexUI Designer와 Core는 `UIScreenDefinition`, Binding과 Backend 계약을 함께 사용하므로 같은 호환 버전으로 업데이트해야 합니다.

## 업데이트 전

1. 작업 브랜치를 만들고 Screen Definition, Metadata, Backend Asset과 `.meta` 파일을 커밋합니다.
2. 수동 변경한 Generated 파일이 있는지 Sync/Publish의 Dry Run과 Diff로 확인합니다.
3. [Compatibility](compatibility.md)와 `CHANGELOG.md`를 읽습니다.

## Package Manager 업데이트

Core를 먼저 업데이트하고 Designer를 업데이트합니다. Git dependency라면 두 package URL의 revision을 같은 검증된 commit/tag로 맞춥니다. Console compile error가 없는지 확인한 뒤 Unity를 다시 엽니다.

## 데이터와 생성물 확인

1. Migration 도구가 제공되는 버전이면 안내에 따라 실행합니다. 현재 `DesignerMetadataAsset`은 `schemaVersion`을 가지며, 임의로 숫자를 수정하면 안 됩니다.
2. 각 주요 Screen을 열고 Validate합니다.
3. UI Toolkit 생성물은 Dry Run과 Diff를 본 뒤 다시 Generate/Publish합니다.
4. Sync 상태와 Publish Manifest를 확인합니다.
5. uGUI와 UI Toolkit을 각각 Play Mode에서 확인합니다.

자동 Migration이 모든 사용자 Backend 변경을 병합해 준다고 가정하지 마세요. 문제가 있으면 업데이트 전 커밋으로 돌아갈 수 있도록 유지합니다.

