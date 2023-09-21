/// <summary>
/// 환경설정 알림 사운드 항목 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 09. 21
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 21) : 최초 생성, Joycollab & TechnoPark 등 작업을 하면서 작성한 것들을 수정 및 적용
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using DG.Tweening;

namespace Joycollab.v2
{
    public class AlarmSoundItem : MonoBehaviour, iRepositoryObserver
    {
        private const string TAG = "AlarmSoundItem"; 

        [Header("content info")]
        [SerializeField] private Text _txtName;

        [Header("toggle")]
        [SerializeField] private Toggle _toggleAlarm;
        [SerializeField] private Image _imgHandle;

        [Header("button")]
        [SerializeField] private Button _btnPreview;
        [SerializeField] private Button _btnStop;

        // local variables
        private TpsInfo info;


    #region Unity functions

        private void Awake() 
        {
            R.singleton.RegisterObserver(this, eStorageKey.Locale);
        }

        private void OnDestroy() 
        {
            if (R.singleton != null)
            {
                R.singleton.UnregisterObserver(this, eStorageKey.Locale);
            }
        }

    #endregion  // Unity functions


    #region Initializer

        public void Init(TpsInfo info) 
        {
            // init state
            this.info = info;

            _txtName.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                "Setting", info.id, R.singleton.CurrentLocale
            );
            _btnStop.gameObject.SetActive(false);


            // set button listener
            _btnPreview.onClick.AddListener(async () => {
                Debug.Log($"{TAG} | sound play.");

                string url = $"{URL.SYSTEM_SOUND_PATH}{info.refVal}";
                AudioClip res = await NetworkTask.GetAudioAsync(url);
                if (res != null)
                    SystemManager.singleton.PlayAudioClip(res);

                _btnStop.gameObject.SetActive(true);
                _btnPreview.gameObject.SetActive(false);
            });

            _btnStop.onClick.AddListener(() => {
                Debug.Log("${TAG} | sound stop.");

                _btnPreview.gameObject.SetActive(true);
                _btnStop.gameObject.SetActive(false);
            });


            // set toggle listener
            _toggleAlarm.onValueChanged.AddListener((isOn) => {
                if (isOn) 
                {
                    _imgHandle.transform.DOLocalMoveX(19.0f, 0.1f);
                    _imgHandle.DOColor(Color.white, 0.1f);
                }
                else 
                {
                    _imgHandle.transform.DOLocalMoveX(-19.0f, 0.1f);
                    _imgHandle.DOColor(C.WORLD_NORMAL, 0.1f);
                }
            });
        }

    #endregion  // Initializer


    #region Event Listener

        public void UpdateInfo(eStorageKey key) 
        {
            if (key == eStorageKey.Locale) 
            {
                _txtName.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                    "Setting", info.id, R.singleton.CurrentLocale
                );
            }
        }

        public bool Usage 
        {
            get {
                return _toggleAlarm.isOn;
            }
            set {
                _toggleAlarm.isOn = value;
            }
        }

    #endregion  // Event Listener
    }
}