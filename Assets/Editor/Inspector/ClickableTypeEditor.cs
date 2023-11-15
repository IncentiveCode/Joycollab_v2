/// <summary>
/// clickable class 의 inspector 를 custom 하는 editor class
/// @author         : HJ Lee
/// @last update    : 2023. 11. 14
/// @version        : 0.5
/// @update         :
///     v0.1 (2023. 04. 19) : 최초 생성. v1 에서 사용했던 것들 가지고 와서 수정 후 적용.
/// 	v0.2 (2023. 09. 25) : world 에 적용하는 작업 시작.
/// 	v0.3 (2023. 10. 21) : Building, World Avatar 에 사용할 기능 적용.
///     v0.4 (2023. 11. 01) : summary 추가 및 기능 일부 정리.
///     v0.5 (2023. 11. 14) : alpha value 대신 enter, exit color 추가. 
/// </summary>

using UnityEditor;

namespace Joycollab.v2
{
    [CustomEditor(typeof(Clickable))]
    public class ClickableTypeEditor : Editor
    {
        private SerializedProperty _objectType;
        private SerializedProperty _rendererType;
        private SerializedProperty _menuItems;
        private SerializedProperty _colorOnEnter;
        // private SerializedProperty _alphaValueOnEnter;
        private SerializedProperty _colorOnExit;
        // private SerializedProperty _alphaValueOnExit;

        protected virtual void OnEnable() 
        {
            _objectType = this.serializedObject.FindProperty("_objectType");     
            _rendererType = this.serializedObject.FindProperty("_rendererType");     
            _menuItems = this.serializedObject.FindProperty("_menuItems");     
            _colorOnEnter = this.serializedObject.FindProperty("_colorOnEnter");     
            // _alphaValueOnEnter = this.serializedObject.FindProperty("_alphaValueOnEnter");     
            _colorOnExit = this.serializedObject.FindProperty("_colorOnExit");     
            // _alphaValueOnExit = this.serializedObject.FindProperty("_alphaValueOnExit");     
        }

        public override void OnInspectorGUI() 
        {
            this.serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_objectType);
            EditorGUILayout.PropertyField(_rendererType);
            EditorGUILayout.PropertyField(_colorOnEnter);
            // EditorGUILayout.PropertyField(_alphaValueOnEnter);
            EditorGUILayout.PropertyField(_colorOnExit);
            // EditorGUILayout.PropertyField(_alphaValueOnExit);
            EditorGUILayout.PropertyField(_menuItems);

            int t = _objectType.enumValueIndex;
            switch (t) 
            {
                case (int) eClickableObjectType.Building :
                    DisplayBuildingInfo();
                    break;

                case (int) eClickableObjectType.Elevator :
                    DisplayElevator();
                    break;

                case (int) eClickableObjectType.Information :
                    DisplayInformation();
                    break;

                case (int) eClickableObjectType.WorldAvatar :
                    DisplayAvatarInfo();
                    break;

                case (int) eClickableObjectType.Board :
                    DisplayBoard();
                    break;
                
                case (int) eClickableObjectType.Notice :
                    DisplayNotice();
                    break;

                case (int) eClickableObjectType.Object :
                    DisplayObjectInfo();
                    break;

                case (int) eClickableObjectType.Dummy :
                    DisplayDummyInfo();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        } 

        private void DisplayBuildingInfo() 
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_soBuildingData"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_imgTag"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_alwaysOpenTag"));
        }

        private void DisplayAvatarInfo() 
        {

        }

        private void DisplayElevator() 
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_goElevatorMenu"));
        }

        private void DisplayInformation() 
        {
            EditorGUILayout.LabelField("Display information _ Not yet.");
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("_goInformation"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("_floorNo"));
        }
        
        private void DisplayBoard() 
        {
            EditorGUILayout.LabelField("Display board _ Not yet.");
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("_goBoard"));
        }

        private void DisplayNotice()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_goNotice"));
        }

        private void DisplayObjectInfo() 
        {
            EditorGUILayout.LabelField("Display Object Info _ Not yet.");
        }

        private void DisplayDummyInfo() 
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_goDummyMenu"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("_soDummyData"));
        }
    }
}