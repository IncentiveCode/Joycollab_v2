/// <summary>
/// Button 을 꾸미는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 08. 01
/// @version        : 0.3
/// @update         :
///     v0.1 : 최초 생성.
/// 	v0.2 (2023. 03. 23) : TMP 대신 Legacy Text 를 사용하도록 수정.
/// 	v0.3 (2023. 08. 01) : 기존에 사용하던 Tooltip, WorldTooltip 관련 정보 적용.
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Joycollab.v2
{
	[Serializable]
	public class SwapImageProperty 
	{
		public Image target;
		public Sprite sprite;
		public Sprite spriteOn;
		public Color color;
		public Color colorOn;
	}

	[Serializable] 
	public class SwapTextProperty 
	{
		public Text target;
		public Color color;
		public Color colorOn;
		public FontStyle style;
		public FontStyle styleOn;
	}


	[RequireComponent(typeof(Button))]
	public class Decoratable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		private const string TAG = "Decoratable";
		private Button btn;

        [SerializeField] private eDecoratableType _type;

	 	[Header("type : Swap Image")]
		[SerializeField] private SwapImageProperty[] _properties;

	 	[Header("tooltip")]
		[SerializeField] private GameObject _goTooltip;
		[SerializeField] private GameObject _goWorldTooltip;
		[SerializeField] private eTooltipAnchor _anchor;
		[SerializeField] private string _strKey;
		[SerializeField] private bool _isOverride;
		private GameObject instTooltip;
		// private Tooltip csTooltip;
		private GameObject instWorldTooltip;
		// private Tooltip csWorldTooltip;

	 	[Header("type : Swap Text style")]
		[SerializeField] private SwapTextProperty[] _textProperties;


	#region Unity functions

		private void Awake() 
		{
			btn = GetComponent<Button>();
		}

		private void OnEnable() 
		{
			switch (_type) 
			{
				case eDecoratableType.SwapImage :
					SetImage(false);
					break;

				case eDecoratableType.SwapImageWithTooltip :
					SetImage(false);
					MakeTooltip(_strKey, _anchor);
					SetTooltip(false);
					break;

				case eDecoratableType.ChangeTextStyle :
					SetText(false);
					break;

				case eDecoratableType.Both :
					SetImage(false);
					SetText(false);
					break;

				case eDecoratableType.TooltipOnly :
					MakeTooltip(_strKey, _anchor);
					SetTooltip(false);
					break;

				case eDecoratableType.WorldTooltip :
					MakeWorldTooltip(_anchor);
					SetWorldTooltip(false);
					break;

				default :
                    Debug.LogWarning($"{TAG} | Start() - 타입이 지정되지 않은 Object. name : {gameObject.name}");
					break;
			}
		}

	#endregion


	#region Private functions

		private void SetImage(bool isEnter) 
		{
			if (_properties.Length == 0) return;

			Image img;
			Sprite sprite, spriteOn;
			Color color, colorOn;

			for (int i = 0; i < _properties.Length; i++) 
			{
				img = _properties[i].target;
				sprite = _properties[i].sprite;
				spriteOn = _properties[i].spriteOn;
				color = _properties[i].color;
				colorOn = _properties[i].colorOn;

				img.sprite = isEnter ? spriteOn : sprite;
				img.color = isEnter ? colorOn : color;
			}
		}

		private void MakeTooltip(string key, eTooltipAnchor anchor) 
		{
			if (instTooltip == null /*|| csTooltip == null*/)
			{
				instTooltip = Instantiate(_goTooltip, Vector3.zero, Quaternion.identity);
				instTooltip.transform.SetParent(this.transform, false);
				// csTooltip = instTooltip.GetComponent<Tooltip>();
			}

			// csTooltip.Init(key, anchor, _isOverride);
			// csTooltip.Active(false);
		}

		private void SetTooltip(bool isEnter) 
		{
			// csTooltip.Active(isEnter);
		}

		private void SetText(bool isEnter)
		{
			if (_textProperties.Length == 0) return;

			Text txt;
			Color color, colorOn;
			FontStyle style, styleOn;

			for (int i = 0; i < _textProperties.Length; i++) 
			{
				txt = _textProperties[i].target;
				color = _textProperties[i].color;
				colorOn = _textProperties[i].colorOn;
				style = _textProperties[i].style;
				styleOn = _textProperties[i].styleOn;

				txt.color = isEnter ? colorOn : color;
				txt.fontStyle = isEnter ? styleOn : style;
			}
		}

		private void MakeWorldTooltip(eTooltipAnchor anchor) 
		{
			/**
			if (instWorldTooltip == null || csTooltip == null) 
			{
				instWorldTooltip = Instantiate(_goWorldTooltip, Vector3.zero, Quaternion.identity);
				instWorldTooltip.transform.SetParent(this.transform, false);
				csWorldTooltip = instWorldTooltip.GetComponent<Tooltip>();
			}

			csWorldTooltip.Init(anchor, _isOverride);
			csWorldTooltip.Active(false);
			 */
		}

		private void SetWorldTooltip(bool isEnter) 
		{
			/**
			csWorldTooltip.Active(isEnter);
			 */
		}

		public void SetKey(string key) 
		{
			if (string.IsNullOrEmpty(key)) return;

			/**
			_strKey = $"Mobile.{key}";
			csWorldTooltip.SetKey(_strKey);
			 */
		}

	#endregion
		

	#region Interface functions implementation

		public void OnPointerEnter(PointerEventData data) 
		{
			switch (_type) 
			{
				case eDecoratableType.SwapImage :
					if (btn.interactable) 
					{
						SetImage(true);
					}
					break;

				case eDecoratableType.SwapImageWithTooltip :
					if (btn.interactable) 
					{
						SetImage(true);
						SetTooltip(true);
					}
					break;

				case eDecoratableType.ChangeTextStyle :
					if (btn.interactable) 
					{
						SetText(true);
					}
					break;

				case eDecoratableType.Both :
					if (btn.interactable)
					{
						SetImage(true);
						SetText(true);
					}
					break;

				case eDecoratableType.TooltipOnly :
					if (btn.interactable) SetTooltip(true);
					break;

				case eDecoratableType.WorldTooltip :
					if (btn.interactable) SetWorldTooltip(true);
					break;

				default :
                    Debug.LogWarning($"{TAG} | OnPointerEnter(), 타입이 지정되지 않은 Object. name : {gameObject.name}");
					break;
			}
		}

		public void OnPointerExit(PointerEventData data) 
		{
			switch (_type) 
			{
				case eDecoratableType.SwapImage :
					SetImage(false);
					break;

				case eDecoratableType.SwapImageWithTooltip :
					SetImage(false);
					SetTooltip(false);
					break;

				case eDecoratableType.ChangeTextStyle :
					SetText(false);
					break;

				case eDecoratableType.Both :
					SetImage(false);
					SetText(false);
					break;

				case eDecoratableType.TooltipOnly :
					SetTooltip(false);
					break;

				case eDecoratableType.WorldTooltip :
					SetWorldTooltip(false);
					break;

				default :
                    Debug.LogWarning($"{TAG} | OnPointerExit(), 타입이 지정되지 않은 Object. name : {gameObject.name}");
					break;
			}
		}

	#endregion
	}
}