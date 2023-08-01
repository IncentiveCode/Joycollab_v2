/// <summary>
/// Plan data 를 받아서 출력하는 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 08. 01
/// @version        : 0.2
/// @update
///     v0.1 (2023. 07. 31) : 최초 생성
///     v0.2 (2023. 08. 01) : Init() 할 때, localize string key 를 전달하는 형태로 변경.
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
{
    public class PlanParser : MonoBehaviour
    {
        private const string TAG = "PlanParser";

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

        [Header("display part")]
        [SerializeField] private Transform _transform;
        [SerializeField] private GameObject _goContent;

        // local variable
        private Color color;


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

            _btnFree.onClick.AddListener(() => {
                Debug.Log($"{TAG} | free plan contents.");
                Parse(_planFree);
            });

            _btnBasic.onClick.AddListener(() => {
                Debug.Log($"{TAG} | basic plan contents.");
                Parse(_planBasic);
            });

            _btnStandard.onClick.AddListener(() => {
                Debug.Log($"{TAG} | standard plan contents.");
                Parse(_planStandard);
            });

            _btnPremium.onClick.AddListener(() => {
                Debug.Log($"{TAG} | premium plan contents.");
                Parse(_planPremium);
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
            color = data.PlanColor;

            ClearContent();
            ChangeContent(data);
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