/// <summary>
/// [world]
/// 환경설정 Script
/// @author         : HJ Lee
/// @last update    : 2023. 09. 15
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 15) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class SettingsW : WindowView, iRepositoryObserver
    {
        private const string TAG = "SettingsW";

        [Header("page")]
        [SerializeField] private IndividualSettings _pageIndividual;
        [SerializeField] private Configuration _pageConfiguration;

        [Header("toggle")]
        [SerializeField] private Toggle _toggleIndividual;
        [SerializeField] private Toggle _toggleConfiguration;

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
                }
            });

            _toggleConfiguration.onValueChanged.AddListener((isOn) => {
                if (isOn)
                {
                    _pageConfiguration.Show().Forget();
                    _pageIndividual.Hide();
                }
            });


            // set button listener
            _btnClose.onClick.AddListener(() => Hide());
            _btnSave.onClick.AddListener(async () => {
                if (_toggleIndividual.isOn) 
                {
                    _pageIndividual.Block(true); 
                    await UniTask.Delay(50);

                    Debug.Log($"{TAG} | 개인 설정 저장.");

                    await UniTask.Delay(50);
                    _pageIndividual.Block(false); 
                }
                else
                {
                    Debug.Log($"{TAG} | 환경 설정 저장.");
                }
            });
        }

        public async override UniTaskVoid Show() 
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