# Figma Bridge

**현재 상태: 실험적 — 인증과 원본 JSON 조회만 지원**

Figma Bridge는 `Tools > NexUI > 유틸리티`에서 **Figma Bridge**를 선택해 엽니다. Personal Access Token을 EditorPrefs에 저장·삭제하고 `GET /v1/me` 연결을 확인하며, File Key로 파일 JSON의 앞부분을 조회할 수 있습니다.

> [!WARNING]
> Frame을 Screen으로 Import하거나 Auto Layout, Text/Font, Component, Style, Token을 변환하는 엔진은 구현되어 있지 않습니다. Fetch 결과가 Metadata나 Backend Asset을 변경하지 않습니다.

Token은 프로젝트 파일이 아닌 로컬 EditorPrefs에 저장되지만 운영 계정의 장기 Token 사용은 피하고, 화면 공유·로그·버그 보고에 포함하지 마세요.

