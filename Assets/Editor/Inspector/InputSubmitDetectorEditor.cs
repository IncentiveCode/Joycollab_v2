/// <summary>
/// InputField 의 Submit 을 탐지하는 'InputSubmitDetector' 의 inspector 관리 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 03. 21
/// @version        : 0.1
/// @update
///     v0.1 (2023. 03. 20) : 최초 생성
/// </summary>

using UnityEditor;
using UnityEditor.UI;

namespace Joycollab.v2
{
    [CustomEditor(typeof(InputSubmitDetector), true)]
    [CanEditMultipleObjects]
    public class InputSubmitDetectorEditor : InputFieldEditor
    {
        private SerializedProperty m_KeyboardDoneProperty;
        private SerializedProperty m_TextComponent;

        protected override void OnEnable() 
        {
            base.OnEnable();

            m_KeyboardDoneProperty = serializedObject.FindProperty("m_keyboardDone");
            m_TextComponent = serializedObject.FindProperty("m_TextComponent");
        }

        public override void OnInspectorGUI() 
        {
            base.OnInspectorGUI();
            EditorGUI.BeginDisabledGroup(m_TextComponent == null || m_TextComponent.objectReferenceValue == null);
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_KeyboardDoneProperty);
            serializedObject.ApplyModifiedProperties();

            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();
        }
    }
}