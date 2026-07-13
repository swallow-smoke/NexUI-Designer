# uGUI Backend

uGUI Screen의 Backend Asset은 Prefab입니다. Save 시 `UGUIAssetSerializer`가 `PrefabUtility.LoadPrefabContents`로 Prefab을 열고 Metadata Element ID에 대응하는 GameObject와 `RectTransform`을 갱신한 뒤 저장합니다.

Panel/Image 계열은 `Image`, Button 계열은 `Button`, Label/Button Text는 `UnityEngine.UI.Text`를 사용합니다. Progress 계열은 Filled `Image`와 `fillAmount`로 일부 변환합니다. 이 Serializer는 TextMeshPro를 자동 생성하지 않습니다.

Designer가 주로 쓰는 범위는 Rect, Anchor, 순서, 이름, 활성 상태, Tint, 기본 Text/Button/Fill 구성입니다. 사용자 Script, 복잡한 Animator, EventSystem 설정과 커스텀 Component는 Designer 소유 범위가 아닙니다.

지원하지 않는 Component도 Placeholder GameObject가 만들어질 수 있으므로 Save Report의 Skipped/Warning을 확인해야 합니다. Preview와 Prefab Runtime 결과는 폰트, Layout Group, Canvas Scaler에 따라 달라질 수 있습니다.

