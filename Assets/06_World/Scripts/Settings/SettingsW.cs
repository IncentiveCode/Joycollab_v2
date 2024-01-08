/// <summary>
/// [world]
/// 환경설정 Script
/// @author         : HJ Lee
/// @last update    : 2024. 01. 08
/// @version        : 0.3
/// @update
///     v0.1 (2023. 09. 15) : 최초 생성
///     v0.2 (2024. 01. 02) : 관리자 설정 페이지 추가
///     v0.3 (2024. 01. 08) : 관리자 설정 페이지 저장 기능 추가 및 Refresh / save call 방식 변경.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class SettingsW : WindowView, iRepositoryObserver
    {
        private const string TAG = "SettingsW";

        [Header("toggle")]
        [SerializeField] private Toggle _toggleIndividual;
        [SerializeField] private Toggle _toggleConfiguration;
        [SerializeField] private Toggle _toggleAdmin;

        [Header("page")]
        [SerializeField] private IndividualSettings _pageIndividual;
        [SerializeField] private Configuration _pageConfiguration;
        [SerializeField] private AdminSettings _pageAdmin;

        [Header("button")]
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnSave;
        
        // local variables
        private eStorageKey keyRefresh;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();

            keyRefresh = eStorageKey.WindowRefresh;
        }

    #endregion  // Unity functions

    
    #region WindowView functions

        protected override void Init() 
        {
            base.Init();
            viewID = ID.SETTINGS_W;


            // set toggle listener
            _toggleIndividual.onValueChanged.AddListener((isOn) => {
                if (isOn) 
                {
                    _pageIndividual.Show().Forget();
                    _pageConfiguration.Hide();
                    _pageAdmin.Hide();
                }
            });
            _toggleConfiguration.onValueChanged.AddListener((isOn) => {
                if (isOn)
                {
                    _pageIndividual.Hide();
                    _pageConfiguration.Show().Forget();
                    _pageAdmin.Hide();
                }
            });
            _toggleAdmin.onValueChanged.AddListener((isOn) => {
                if (isOn) 
                {
                    _pageIndividual.Hide();
                    _pageConfiguration.Hide();
                    _pageAdmin.Show().Forget();
                }
            });


            // set button listener
            _btnClose.onClick.AddListener(() => Hide());
            _btnSave.onClick.AddListener(async () => {

                string memberType = R.singleton.myMemberType;
                bool isAdmin = memberType.Equals(S.ADMIN) || memberType.Equals(S.OWNER);

                _pageIndividual.Block(true);
                _pageConfiguration.Block(true);
                if (isAdmin) _pageAdmin.Block(true);

                // 각 페이지에 있는 저장 api call
                string individualRes = string.Empty, configRes = string.Empty, adminRes = string.Empty;
                if (isAdmin) 
                {
                    (individualRes, configRes, adminRes) = await UniTask.WhenAll(
                        _pageIndividual.UpdateMyInfo(),
                        _pageConfiguration.UpdateConfiguration(),
                        _pageAdmin.UpdateCenterInfo()
                    );
                }
                else 
                {
                    (individualRes, configRes) = await UniTask.WhenAll(
                        _pageIndividual.UpdateMyInfo(),
                        _pageConfiguration.UpdateConfiguration()
                    );
                }

                _pageIndividual.Block(false); 
                _pageConfiguration.Block(false);
                if (isAdmin) _pageAdmin.Block(false);

                // error check
                if (! string.IsNullOrEmpty(individualRes)) 
                {
                    Debug.Log($"{TAG} | 개인 설정 저장 실패.");
                    PopupBuilder.singleton.OpenAlert(individualRes);
                    return;
                }

                if (! string.IsNullOrEmpty(configRes)) 
                {
                    Debug.Log($"{TAG} | 환경 설정 저장 실패.");
                    PopupBuilder.singleton.OpenAlert(configRes);
                    return;
                }

                if (isAdmin && ! string.IsNullOrEmpty(adminRes))
                {
                    Debug.Log($"{TAG} | 관리자 설정 저장 실패.");
                    PopupBuilder.singleton.OpenAlert(adminRes);
                    return;
                }

                // 이상 없다면 완료 출력
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "환경설정.변경 완료 안내", R.singleton.CurrentLocale)
                );
            });
        }

        public override async UniTaskVoid Show() 
        {
            base.Show().Forget();

            if (R.singleton != null) 
            {
                R.singleton.RegisterObserver(this, keyRefresh);
            }

            await Refresh();
            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();

            if (_toggleIndividual.isOn) 
            {
                Debug.Log($"{TAG} | 개인 설정 취소.");
            }
            else 
            {
                Debug.Log($"{TAG} | 환경 설정 취소.");
            }

            if (R.singleton != null) 
            {
                R.singleton.UnregisterObserver(this, keyRefresh);
            }
        }

    #endregion  // WindowView functions


    #region EVent handling

        private async UniTask<int> Refresh() 
        {
            string memberType = R.singleton.myMemberType;
            bool isAdmin = memberType.Equals(S.ADMIN) || memberType.Equals(S.OWNER);
            _toggleAdmin.gameObject.SetActive(isAdmin);

            _toggleIndividual.isOn = true;
            _pageIndividual.Show().Forget();
            _pageConfiguration.Hide();
            _pageAdmin.Hide();

            // 각 페이지에 있는 refresh api call
            int individualRes = 0, configRes = 0, adminRes = 0;
            if (isAdmin) 
            {
                (individualRes, configRes, adminRes) = await UniTask.WhenAll(
                    _pageIndividual.Refresh(),
                    _pageConfiguration.Refresh(),
                    _pageAdmin.Refresh()
                );
            }
            else 
            {
                (individualRes, configRes) = await UniTask.WhenAll(
                    _pageIndividual.Refresh(),
                    _pageConfiguration.Refresh()
                );
            }

            if (individualRes != 0)
            {
                Debug.Log($"{TAG} | 개인 설정 로딩 실패.");
                PopupBuilder.singleton.OpenAlert("개인 정보 로딩 실패.");
            }

            if (configRes != 0) 
            {
                Debug.Log($"{TAG} | 환경 설정 로딩 실패.");
                PopupBuilder.singleton.OpenAlert("환경 설정 로딩 실패.");
            }

            if (isAdmin && adminRes != 0) 
            {
                Debug.Log($"{TAG} | 관리자 설정 로딩 실패.");
                PopupBuilder.singleton.OpenAlert("관리자 설정 로딩 실패.");
            }

            return 0;
        }

        public void UpdateInfo(eStorageKey key) 
        {
            if (key == keyRefresh) 
            {
                // TODO. refresh event 를 어떻게 처리할지 연구.
                if (_toggleIndividual.isOn) 
                    Debug.Log($"{TAG} | UpdateInfo() call. - 개인 설정 업데이트.");
                else if (_toggleConfiguration.isOn)
                    Debug.Log($"{TAG} | UpdateInfo() call. - 환경 설정 업데이트.");
                else 
                    Debug.Log($"{TAG} | UpdateInfo() call. - 관리자 설정 업데이트.");
            }
        }

    #endregion  // EVent handling
    }
}