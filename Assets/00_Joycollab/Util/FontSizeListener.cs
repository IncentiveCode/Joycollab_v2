/// <summary>
/// Font 설정 변경 이벤트 처리하는 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 13
/// @version        : 0.1
/// @update
///     v0.1 (2022. 07. 13) : 최초 생성
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Joycollab.v2 
{
    [RequireComponent(typeof(Text))]
    public class FontSizeListener : MonoBehaviour, iRepositoryObserver
    {
        private const string TAG = "FontSizeListener";
        private const int _SMALL = 1;
        private const int _NORMAL = 2;
        private const int _LARGE = 3;

        [Header("text option")]
        [SerializeField] private Text _text;
        [SerializeField] private int sizeOpt;
        [SerializeField, Range(10, 20)] private int _sizeSmall;
        [SerializeField, Range(12, 24)] private int _sizeNormal;
        [SerializeField, Range(14, 28)] private int _sizeLarge;

        [Header("(optional) layout element")]
        [SerializeField] private LayoutElement _layoutElement;


    #region Unity functions

        private void Awake() 
        {
            _text = GetComponent<Text>();

            // set event variables and add listener
            keyFontSize = eStorageKey.FontSize;

            R.singleton.RegisterObserver(this, keyFontSize);
        }

        private void OnDestroy() 
        {
            if (R.singleton != null)
            {
                R.singleton.UnregisterObserver(this, keyFontSize);
            }
        }
    
    #endregion  // Unity functions


    #region Event Listener

        private eStorageKey keyFontSize;

        public void UpdateInfo(eStorageKey key) 
        {
            if (key == keyFontSize) 
            {
                sizeOpt = R.singleton.FontSizeOpt;

                switch (sizeOpt) 
                {
                    case _NORMAL :
                        _text.fontSize = _sizeNormal;
                        break;

                    case _LARGE :
                        _text.fontSize = _sizeLarge;
                        break;

                    case _SMALL :
                    default :
                        _text.fontSize = _sizeSmall;
                        break;
                }

                /**
                // TODO. layout element size change
                if (_layoutElement != null) 
                {
                    Canvas.ForceUpdateCanvases();
                    float tmp = _text.GetComponent<RectTransform>().rect.height;
                    Debug.Log("current text height : "+ tmp);
                    Debug.Log("next element height : "+ (tmp+4));

                    _layoutElement.preferredHeight = (tmp + 4);
                }
                 */
            }
        }

    #endregion  // Event Listener

    }
}