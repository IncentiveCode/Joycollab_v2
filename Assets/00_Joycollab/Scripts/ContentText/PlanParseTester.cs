/// <summary>
/// Plan parse class 를 test 하기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 11. 09
/// @version        : 0.2
/// @update
///     v0.1 (2023. 09. 12) : 최초 생성. PlanParser 에서 분리.
///     v0.2 (2023. 11. 09) : 각 언어별 가격 정보 추가.
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
{
    public class PlanParseTester : MonoBehaviour
    {
        private const string TAG = "PlanParseTester";
        
        [Header("plan data")]
        [SerializeField] private PlanData _planFree;
        [SerializeField] private PlanData _planBasic;
        [SerializeField] private PlanData _planStandard;
        [SerializeField] private PlanData _planPremium;

        [Header("test part")]
        [SerializeField] private Button _btnFree;
        [SerializeField] private Button _btnBasic;
        [SerializeField] private Button _btnStandard;
        [SerializeField] private Button _btnPremium;
        [SerializeField] private Text _txtFee;
        [SerializeField] private Toggle _toggleMonthly;
        [SerializeField] private Toggle _toggleYearly;

        [Header("display part")]
        [SerializeField] private Transform _transform;
        [SerializeField] private GameObject _goContent;

        // local variable
        private Color color;
        private PlanData currentData;


    #region Unity functions

        private void Awake() 
        {
            if (_transform == null) 
            {
                Debug.Log($"{TAG} | 내용물이 출력될 transform 을 먼저 설정해야 테스트를 진행할 수 있습니다.");
                return;
            }

            if (_goContent == null) 
            {
                Debug.Log($"{TAG} | 내용물이 출력될 GameObject 를 먼저 설정해야 테스트를 진행할 수 있습니다.");
                return;
            }

            _btnFree.onClick.AddListener(() => Parse(_planFree));
            _btnBasic.onClick.AddListener(() => Parse(_planBasic));
            _btnStandard.onClick.AddListener(() => Parse(_planStandard));
            _btnPremium.onClick.AddListener(() => Parse(_planPremium));

            _toggleMonthly.onValueChanged.AddListener((isOn) => {
                if (! isOn) return;
                _txtFee.text = currentData.GetFee(false); 
            });
            _toggleYearly.onValueChanged.AddListener((isOn) => {
                if (! isOn) return;
                _txtFee.text = currentData.GetFee(true);
            });
        }

        private void OnEnable() 
        {
            Parse(_planFree);
        }

    #endregion  // Unity functions


    #region public function

        public void Parse(PlanData data) 
        {
            currentData = data;
            color = data.PlanColor;

            ClearContent();
            ChangeContent(data);

            _txtFee.text = data.GetFee(_toggleYearly.isOn);
        }

    #endregion  // public function


    #region private functions

        private void CreateContentBlock(string key) 
        {
            GameObject c = Instantiate(_goContent, Vector3.zero, Quaternion.identity);
            c.GetComponent<PlanContent>().Init(color, key);
            c.transform.SetParent(_transform, false);
        }

        private void ClearContent() 
        {
            Transform children = _transform.GetComponentInChildren<Transform>();
            foreach (Transform child in children) 
            {
                if (child.name.Equals(_transform.name)) continue;
                Destroy(child.gameObject);
            }
        }

        private void ChangeContent(PlanData data) 
        {
            for (int i = 0; i < data.Count; i++) 
            {
                CreateContentBlock(data.GetFeature(i));
            }
        }

    #endregion  // private functions
    }
}