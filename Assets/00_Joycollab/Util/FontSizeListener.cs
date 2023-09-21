/// <summary>
/// Font 설정 변경 이벤트 처리하는 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 13
/// @version        : 0.1
/// @update
///     v0.1 (2022. 07. 13) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;

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

            R.singleton.RegisterObserver(this, eStorageKey.FontSize);
        }

        private void OnEnable() 
        {
            UpdateInfo(eStorageKey.FontSize);    
        }

        private void OnDestroy() 
        {
            if (R.singleton != null)
            {
                R.singleton.UnregisterObserver(this, eStorageKey.FontSize);
            }
        }
    
    #endregion  // Unity functions


    #region Event Listener

        public void UpdateInfo(eStorageKey key) 
        {
            if (key == eStorageKey.FontSize) 
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

                // TODO. layout element size change... 일단 임시로 조치
                if (_layoutElement != null) 
                {
                    _layoutElement.flexibleHeight = 0;
                    _layoutElement.flexibleHeight = 1;
                }
            }
        }

    #endregion  // Event Listener

    }
}