/// <summary>
/// Button 을 꾸미는 'Decoratable' 의 inspector 관리 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 03. 23
/// @version        : 0.2
/// @update         :
///     v0.1 : 최초 생성.
/// 	v0.2 (2023. 03. 23) : TMP 대신 Legacy Text 를 사용하도록 수정.
/// </summary>

using UnityEditor;

namespace Joycollab.v2
{
	[CustomEditor(typeof(Decoratable))]
	public class DecoratableTypeEditor : Editor 
	{
		private SerializedProperty _type;

		protected virtual void OnEnable() 
		{
			_type = this.serializedObject.FindProperty("_type");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(_type);
			int t = _type.enumValueIndex;
			switch (t) 
			{
				case (int) eDecoratableType.SwapImage :
					DisplaySwapImage();
					break;

				case (int) eDecoratableType.SwapImageWithTooltip :
					DisplaySwapImageWithTooltip();
					break;

				case (int) eDecoratableType.ChangeTextStyle :
					DisplayChangeTextStyle();
					break;

				case (int) eDecoratableType.Both :
					DisplayBoth();
					break;

				case (int) eDecoratableType.TooltipOnly :
					DisplayTooltip();
					break;

				default :
					DisplayNone();
					break;
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void DisplaySwapImage() 
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_properties"));
		}

		private void DisplaySwapImageWithTooltip() 
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_properties"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_goTooltip"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_anchor"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_strKey"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_isOverride"));
		}

		private void DisplayChangeTextStyle() 
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_textProperties"));
		}

		private void DisplayBoth() 
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_properties"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_textProperties"));
		}

		private void DisplayTooltip() 
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_goTooltip"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_anchor"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_strKey"));
		}

		private void DisplayNone() 
		{
            EditorGUILayout.LabelField("원하는 타입을 먼저 설정하시기 바랍니다.");
		}
	}
}