/// <summary>
/// [world]
/// 환경설정 Script
/// @author         : HJ Lee
/// @last update    : 2024. 01. 02
/// @version        : 0.2
/// @update
///     v0.1 (2023. 09. 15) : 최초 생성
///     v0.2 (2024. 01. 02) : 관리자 설정 페이지 추가
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
                if (_toggleIndividual.isOn) 
                {
                    _pageIndividual.Block(true); 

                    Debug.Log($"{TAG} | 개인 설정 저장.");
                    string res = await _pageIndividual.UpdateMyInfo();
                    if (string.IsNullOrEmpty(res)) 
                    {
                        PopupBuilder.singleton.OpenAlert(
                            LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "환경설정.변경 완료 안내", R.singleton.CurrentLocale)
                        );

                        _pageIndividual.Show().Forget();
                    }
                    else 
                    {
                        PopupBuilder.singleton.OpenAlert(res);
                    }

                    _pageIndividual.Block(false); 
                }
                else
                {
                    _pageConfiguration.Block(true);

                    Debug.Log($"{TAG} | 환경 설정 저장.");
                    string res = await _pageConfiguration.UpdateConfiguration();
                    if (string.IsNullOrEmpty(res))
                    {
                        PopupBuilder.singleton.OpenAlert(
                            LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "환경설정.변경 완료 안내", R.singleton.CurrentLocale)
                        );

                        _pageConfiguration.Show().Forget();
                    }
                    else 
                    {
                        PopupBuilder.singleton.OpenAlert(res);
                    }

                    _pageConfiguration.Block(false);
                }
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
            _toggleIndividual.isOn = true;
            _pageIndividual.Show().Forget();
            _pageConfiguration.Hide();
            _pageAdmin.Hide();

            _toggleAdmin.gameObject.SetActive(R.singleton.myMemberType.Equals(S.ADMIN));

            await UniTask.Yield();
            return 0;
        }

        public void UpdateInfo(eStorageKey key) 
        {
            if (key == keyRefresh) 
            {
                // TODO. refresh event 를 어떻게 처리할지 연구.
                if (_toggleIndividual.isOn) 
                    Debug.Log($"{TAG} | UpdateInfo() call. - 개인 설정 업데이트.");
                else
                    Debug.Log($"{TAG} | UpdateInfo() call. - 환경 설정 업데이트.");
            }
        }

    #endregion  // EVent handling
    }
}