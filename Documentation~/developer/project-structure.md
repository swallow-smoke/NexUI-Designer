# 프로젝트 구조

| 경로 | 책임 | 대표 코드/주의사항 |
|---|---|---|
| `Editor/Core` | Window, Context, Session, Undo, Hierarchy | UI 전용 기능을 Runtime으로 내리지 않습니다. |
| `Editor/UI/Shell` | Global Toolbar, Sidebar, Inspector, Drawer | 복잡한 데이터 처리를 넣지 않습니다. |
| `Editor/UI/Panels`, `Editor/Panels` | Layers 및 Legacy Panel | Context 이벤트는 `ContextBoundSubscriptions` 사용 |
| `Editor/Viewport` | Canvas 입력, Overlay, Preview 표현 | Asset 저장 책임을 갖지 않습니다. |
| `Editor/Components` | Component descriptor와 support matrix | Component Type의 단일 등록 지점 |
| `Editor/Inspectors` | Metadata Field 편집 | 변경은 Context API와 Undo를 거칩니다. |
| `Editor/Backend` | uGUI/UI Toolkit Preview Adapter | Runtime Backend와 Editor Preview를 연결 |
| `Editor/Serialization` | Backend Save, JSON, UXML/USS 생성 | 생성과 파일 쓰기를 분리합니다. |
| `Editor/Validation` | 사용자용 구조 검증 | 안정적인 Issue Code 유지 |
| `Editor/Advanced`, `Editor/QA` | 독립 Tool과 분석 도구 | Main Window 구현에 직접 의존하지 않습니다. |
| `Editor/Figma` | Figma API 인증/조회 | 현재 변환 책임 없음 |
| `Editor/Styles`, `Localization` | USS와 ko/en UI 문자열 | API명은 번역하지 않습니다. |
| `Runtime/Metadata` | 직렬화 가능한 Designer 데이터 | `UnityEditor` 참조 금지 |
| `Samples~` | Import 가능한 Vertical Slice | 제품 Runtime 시스템으로 확장하지 않습니다. |
| `Documentation~` | UPM 문서 | 사용자·Reference·Developer 분리 |

