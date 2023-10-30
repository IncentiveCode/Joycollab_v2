/// <summary>
/// 건물 정보 팝업
/// @author         : HJ Lee
/// @last update    : 2023. 10. 23.
/// @version        : 0.1
/// @update
///     v0.1 (2023. 10. 23) : v1 에서 만들었던 PopupBuildingInfo 수정 후 적용.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using TMPro;

namespace Joycollab.v2
{
    public class BuildingInfo : MonoBehaviour
    {
        private const string TAG = "BuildingInfo";

        [Header("common")] 
        [SerializeField] private Image _imgPanel;
        [SerializeField] private TMP_Text _txtName;

        [Header("button")] 
        [SerializeField] private Button _btnOffice;
        [SerializeField] private Button _btnHomepage;
        [SerializeField] private Button _btnBookmark;
        [SerializeField] private Button _btnClose;

        [Header("body")]
        [SerializeField] private RawImage _imgLogo;
        [SerializeField] private TMP_Text _txtHomepage;
        [SerializeField] private TMP_Text _txtTel;
        [SerializeField] private TMP_Text _txtAddress;
        
        // area check
        private Vector2 v2Cursor;
        private RectTransform rect;
        
        // canvas scaler, ratio
        private CanvasScaler scaler;
        private float ratio;

        // local info
        // private ImageLoader loader;
        private BuildingData data;
        
    
    #region Unity function

        private void Awake()
        {
            // set button listener
            _btnOffice.onClick.AddListener(() => {
                if (data.UsingJoycollab)
                {
                    JsLib.OpenWebview(data.JoycollabLink, "office");
                }
                else 
                {
                    Debug.Log($"{TAG} | {data.BuildingName} 은 joycollab 주소가 없음.");
                }
            });
            _btnHomepage.onClick.AddListener(() => {
                if (data.UsingHomepage) 
                {
                    JsLib.OpenWebview(data.HomepageLink, "homepage");
                }
                else 
                {
                    Debug.Log($"{TAG} | {data.BuildingName} 은 homepage 주소가 없음.");
                }
            });
            _btnBookmark.onClick.AddListener(() => {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "기능 준비 안내", R.singleton.CurrentLocale)
                );
            });
            _btnClose.onClick.AddListener(Close);


            // set local variables
            rect = _imgPanel.GetComponent<RectTransform>();
            // loader = _imgLogo.GetComponent<ImageLoader>();
            data = null;
        }

        private void Update()
        {
            if (!Input.GetMouseButtonUp(0) && !Input.GetMouseButtonUp(1) && !Input.GetMouseButtonUp(2) &&
                Input.GetAxis("Mouse ScrollWheel") == 0) return;
            
            v2Cursor = rect.InverseTransformPoint(Input.mousePosition);
            if (! rect.rect.Contains(v2Cursor)) Close();
        }
    
    #endregion  // Unity function
    
    
    #region public functions

        public void Open(BuildingData data, Vector2 pos)
        {
            scaler = transform.parent.GetComponent<CanvasScaler>();
            ratio = Util.CalculateScalerRatio(scaler);

            // calculate position
            Vector2 v2Temp = (pos / ratio);
            float maxX = (float) (Screen.width / ratio);
            float maxY = (float) (Screen.height / ratio);
            Vector2 sizeDelta = rect.sizeDelta;
            float panelX = sizeDelta.x;
            float panelY = sizeDelta.y;
            v2Temp.x = Mathf.Clamp(v2Temp.x, 0, maxX - panelX);
            v2Temp.y = Mathf.Clamp(v2Temp.y, 0, maxY - panelY);
            rect.anchoredPosition = v2Temp;

            // set info
            this.data = data;
            _btnOffice.gameObject.SetActive(data.UsingJoycollab);
            _btnHomepage.gameObject.SetActive(data.UsingHomepage);
            _txtName.text = data.BuildingName;
            _imgLogo.texture = data.Logo;
            _txtHomepage.text = data.HomepageLink;
            _txtTel.text = data.Tel;
            _txtAddress.text = data.Address; 

            gameObject.SetActive(true);
        }
    
    #endregion  // public functions


    #region private functions

        private void Close() => Destroy(gameObject);

    #endregion  // private functions
    }
}