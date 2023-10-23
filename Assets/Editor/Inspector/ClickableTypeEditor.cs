using UnityEditor;

namespace Joycollab.v2
{
    [CustomEditor(typeof(Clickable))]
    public class ClickableTypeEditor : Editor
    {
        private SerializedProperty _objectType;
        private SerializedProperty _rendererType;
        private SerializedProperty _menuItems;
        private SerializedProperty _alphaValueOnEnter;
        private SerializedProperty _alphaValueOnExit;

        protected virtual void OnEnable() 
        {
            _objectType = this.serializedObject.FindProperty("_objectType");     
            _rendererType = this.serializedObject.FindProperty("_rendererType");     
            _menuItems = this.serializedObject.FindProperty("_menuItems");     
            _alphaValueOnEnter = this.serializedObject.FindProperty("_alphaValueOnEnter");     
            _alphaValueOnExit = this.serializedObject.FindProperty("_alphaValueOnExit");     
        }

        public override void OnInspectorGUI() 
        {
            this.serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_objectType);
            EditorGUILayout.PropertyField(_rendererType);
            EditorGUILayout.PropertyField(_alphaValueOnEnter);
            EditorGUILayout.PropertyField(_alphaValueOnExit);
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
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("_soAvatarData"));
        }

        private void DisplayElevator() 
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_goElevatorMenu"));
        }

        private void DisplayInformation() 
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_goInformation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_floorNo"));
        }
        
        private void DisplayBoard() 
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_goBoard"));
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