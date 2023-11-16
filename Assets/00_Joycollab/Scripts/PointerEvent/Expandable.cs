/// <summary>
/// 열렸다가 접혔다가 하는 UI 를 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 10. 30
/// @version        : 0.1
/// @update
///     v0.1 (2023. 10. 30) : 최초 생성, TP 에서 사용하던 항목 개선 후 적용.
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
{
    public class Expandable : MonoBehaviour
    {
        private const string TAG = "Expandable";

        [Header("Element")]
        [SerializeField] private LayoutElement _element;
        [SerializeField] private float _closedHeight; 
        [SerializeField] private float _openHeight;
        
        [Header("head")]
        [SerializeField] private GameObject _header;
        [SerializeField] private Image _imgHeader; 
        [SerializeField] private Button _btnExpand;
        [SerializeField] private Button _btnClose;

        [Header("body")]
        [SerializeField] private List<GameObject> _body;

        // local variables
        // private bool isExpand;

    
    #region Unity functions

        private void Awake() 
        {
            // set button listener
            if (_btnExpand != null)     _btnExpand.onClick.AddListener(ExpandPanel);
            if (_btnClose != null)      _btnClose.onClick.AddListener(ClosePanel);


            // init 
            // isExpand = false;
            ClosePanel();
        }

    #endregion  // Unity functions


    #region Expand functions

        public void RequestExpand() => ExpandPanel();

        public void RequestClose() => ClosePanel();

        public void RequestCloseForMap() => ClosePanelForMap();

        private void ExpandPanel() 
        {
            if (_element != null) 
            {
                if (_element.flexibleHeight == 1) 
                    _element.preferredHeight = -1;
                else 
                    _element.preferredHeight = _openHeight;
            }

            _btnExpand.gameObject.SetActive(false);
            _btnClose.gameObject.SetActive(true);
            foreach (var item in _body) item.SetActive(true);
        }

        private void ClosePanel() 
        {
            if (_element != null)
            {
                _element.preferredHeight = _closedHeight;
            }

            _btnExpand.gameObject.SetActive(true);
            _btnClose.gameObject.SetActive(false);
            foreach (var item in _body) item.SetActive(false);
        }

        private void ClosePanelForMap()
        {
            if (_element != null)
            {
                _element.preferredHeight = _closedHeight;
            }

            _btnExpand.gameObject.SetActive(false);
            _btnClose.gameObject.SetActive(false);
            foreach (var item in _body) item.SetActive(false);
        }

    #endregion  // Expand functions
    }
}