# Sync와 Publish

생성된 UXML/USS를 디스크 파일 및 마지막 Publish 기준과 **3-way**로 비교해, Designer 변경·백엔드 변경·충돌을 구분하고 변경분만 Publish합니다 (스펙 §32 · §39.3).

## 3-way 상태 모델

세 값을 비교합니다.

- **Designer**: 지금 메타데이터에서 생성한 UXML/USS
- **File**: 디스크의 `.g.uxml`/`.g.uss`
- **Base**: 마지막 Publish 때 기록한 기준 해시(`DesignerPublishManifest`)

| 상태 | 조건 | 의미 |
|---|---|---|
| **신규(New)** | 파일 없음 | 아직 Publish 안 됨 |
| **동기화됨(InSync)** | Designer == File | 할 일 없음 |
| **Designer 변경** | File == Base, Designer ≠ Base | Designer만 바뀜 → 안전하게 쓰기 가능 |
| **백엔드 변경** | Designer == Base, File ≠ Base | 생성 파일을 손으로 수정함 |
| **충돌(Conflict)** | Designer·File 모두 Base와 다름 | 양쪽 변경 → 해결 필요 |

기준(Base)이 없으면(매니페스트 미설정) 충돌을 구분할 수 없고 변경/미변경만 판단합니다.

## 충돌 해결

- **Designer 사용(쓰기)**: 생성 결과를 파일로 쓰고 기준값으로 기록합니다. 생성 배너가 없는 파일은 덮어쓰지 않습니다.
- **백엔드 사용(기준 채택)**: 디스크 파일을 새 기준값으로 채택합니다(재생성 없이 충돌 해소).
- **Diff 보기**: 파일 → 생성 결과의 라인 단위 차이(+ 추가, - 제거)를 봅니다. LCS 기반.

## 변경분만 Publish (§39.3)

**Dry Run**은 쓰지 않고 각 화면의 처리 결과를 보고합니다. **변경분 Publish**는 신규·Designer 변경 화면만 파일로 쓰고 기준값을 갱신하며, 동기화됨은 건너뛰고 백엔드 변경·충돌은 "해결 필요"로 보고합니다.

- `Tools > NexUI > 유틸리티`에서 **Sync / Publish**를 선택합니다.
- 프로젝트의 모든 `DesignerMetadataAsset`을 대상으로 합니다.

## 아키텍처

- 순수 `SyncStateResolver`(3-way 분류) + `TextLineDiff`(LCS 라인 diff)는 Unity 없이 단위 테스트됩니다(`SyncStateResolverTests`).
- `DesignerPublishService`가 생성(→`UIToolkitCodeGenerator`)·해시(SHA1)·상태 판정·쓰기/채택/변경분 Publish를 담당합니다.
- 기준값은 `DesignerPublishManifest`(Designer.Runtime, 화면별 UXML/USS 해시)에 저장됩니다.
- 쓰기는 항상 별도 생성 파일(`.g.uxml`/`.g.uss`)만 대상으로 하며, 생성 배너를 잃은 파일은 덮어쓰지 않습니다 — 손으로 수정한 파일은 조용히 사라지지 않고 백엔드 변경/충돌로 드러납니다.

## 아직 하지 않는 것

3-way 자동 병합(Merge)은 제공하지 않습니다(Designer/백엔드 중 선택). uGUI 프리팹 대상 동기화, 자동 백업은 이후 단계입니다.
