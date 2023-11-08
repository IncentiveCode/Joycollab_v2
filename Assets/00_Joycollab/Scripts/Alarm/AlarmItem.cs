/// <summary>
/// 알림 리스트 항목 Script
/// @author         : HJ Lee
/// @last update    : 2023. 11. 08
/// @version        : 0.1
/// @update
///     v0.1 (2023. 11. 08) : 최초 생성, v1 & mobile 의 내용 수정 & 기능 확장 후 적용.
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2
{
    public class AlarmItem : InfiniteScrollItem
    {
        private const string TAG = "AlarmItem";

        [Header("image")]
        [SerializeField] private Image _imgMark;
        [SerializeField] private RawImage _imgPhoto;

        [Header("default texture")]
		[SerializeField] private Texture2D _texSeminar;
		[SerializeField] private Texture2D _texToDo;
        [SerializeField] private Texture2D _texMeeting;
        [SerializeField] private Texture2D _texCall;

        [Header("content")]
        [SerializeField] private TMP_Text _txtTitle;
        [SerializeField] private TMP_Text _txtName;
        [SerializeField] private TMP_Text _txtDesc;
        [SerializeField] private TMP_Text _txtDate;
        [SerializeField] private TMP_Text _txtTimeGap;

        [Header("button")]
        [SerializeField] private Button _btnItem;
        [SerializeField] private Button _btnDelete;
        [SerializeField] private Button _btnMenu;

        // tools
        private ImageLoader loader;
	    private AsyncOperationHandle<Texture2D> operationHandle;

        // local variables
		private AlarmData data;
		private int seq;	
		private string id;
		private string fileName;
		private XmppTaskInfo task;


    #region Unity functions

        private void Awake() 
        {
            loader = _imgPhoto.GetComponent<ImageLoader>();


            // set button listener
            _btnItem.onClick.AddListener(OnSelect);
            if (_btnDelete != null) 
            {
                _btnDelete.onClick.AddListener(OnDelete);
            }
            if (_btnMenu != null) 
            {
                _btnMenu.onClick.AddListener(OnMenu);
            }
        }

    #endregion  // Unity functions


	#region GPM functions

		public override void UpdateData(InfiniteScrollData itemData) 
		{
			base.UpdateData(itemData);

			data = (AlarmData) itemData;
			this.seq = data.info.seq;
			this.id = string.IsNullOrEmpty(data.info.tp.id) ? string.Empty : data.info.tp.id;	// 간혹 tp 값이 null 인 데이터가 있음.
			bool isArrange = false;


			// 기본 정보 정리
			_imgMark.gameObject.SetActive(! data.info.read);

			if (id.Equals(S.ALARM_TASK))		_txtTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "일감", R.singleton.CurrentLocale); 
			else if (id.Equals(S.ALARM_TO_DO))	_txtTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "미리알림.옵션", R.singleton.CurrentLocale); 
			else								_txtTitle.text = data.info.title;

            _txtName.text = data.info.sender;

			// 시간값 정리
	        DateTime now = DateTime.Now;
			DateTime createDate = Convert.ToDateTime(data.info.dtm);
            _txtDate.text = R.singleton.isTimeFormatEquals24 ?
                createDate.ToString("yyyy-MM-dd HH:mm") :
                createDate.ToString("yyyy-MM-dd hh:mm tt", R.singleton.CurrentCulture);

			TimeSpan diff = now - createDate;
			if (diff.Days == 0)
			{
				string t = string.Empty;
				if (diff.Hours == 0) 
				{
					t = LocalizationSettings.StringDatabase.GetLocalizedString("Word", 
						diff.Minutes == 1 ? "시간.1분 전" : "시간.수분 전", 
						R.singleton.CurrentLocale);
					_txtTimeGap.text = string.Format(t, diff.Minutes);
				}
				else
				{
					t = LocalizationSettings.StringDatabase.GetLocalizedString("Word", 
						diff.Hours == 1 ? "시간.1시간 전" : "시간.수시간 전", 
						R.singleton.CurrentLocale);
					_txtTimeGap.text = string.Format(t, diff.Hours);
				}
			}
			else 
			{
				_txtTimeGap.text = string.Empty;
			}

            // content 정리
            switch (id) 
			{
				case S.ALARM_RESERVE_MEETING :
				case S.ALARM_UPDATE_MEETING :
				case S.ALARM_DELETE_MEETING :
				case S.ALARM_INVITE_MEETING :
				case S.ALARM_INVITE_MEETING_CANCEL :
				case S.ALARM_START_MEETING :
				case S.ALARM_DONE_MEETING :

				case S.ALARM_RESERVE_SEMINAR :
				case S.ALARM_UPDATE_SEMINAR :
				case S.ALARM_DELETE_SEMINAR :

				case S.ALARM_VOICE_CALL :
				case S.ALARM_REJECT_CALL :
					_txtDesc.text = data.info.content;
					break;

				case S.ALARM_TO_DO :
					_txtDesc.text = data.info.contentJson;
					break;

				case S.ALARM_UPDATE_MEMBER :
                    _txtDesc.text = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", "Member.정보 변경 안내", R.singleton.CurrentLocale);
					break;

				case S.ALARM_UPDATE_SPACE :
					isArrange = true;
                    _txtDesc.text = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", "Space.정보 변경 안내", R.singleton.CurrentLocale);
					break;

				case S.ALARM_UPDATE_SEAT :
                    _txtDesc.text = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", "Seat.정보 변경 안내", R.singleton.CurrentLocale);
					break;
				
				case S.ALARM_TASK :
					task = JsonUtility.FromJson<XmppTaskInfo>(data.info.contentJson);
	                string content = data.info.content.Replace(XmppManager.CONTENT_SPLITTER, "|");
					var arrContent = content.Split('|');
					string c0 = arrContent[0].Replace(XmppManager.TASK_SPLITTER, "|");
                	var arr = c0.Split('|'); 

					string res, key, value;
					if (arr.Length > 1) 
					{
						key = "Kanban."+ arr[0];
						value = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", key, R.singleton.CurrentLocale);
						res = string.Format(value, arr[1]); 
					}
					else
					{
						key = "Kanban."+ content;
						res = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", key, R.singleton.CurrentLocale);
					}

					if (arrContent.Length > 1) 
					{
						res += " ";

						value = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", "Kanban.여러 항목 수정", R.singleton.CurrentLocale);
						res += string.Format(value, (arrContent.Length - 1)); 
					}
					_txtDesc.text = res;
					break;
				
				default :
					Debug.LogWarning($"{TAG} | 알 수 없는 알림 id : {data.info.tp.id}");
					_txtDesc.text = string.Empty;
					break;
			}


            // texture 설정
			if (string.IsNullOrEmpty(data.info.img)) 
			{
                loader.SetDefault();
			}
			else if (isArrange) 
			{
                string[] split  = data.info.img.Split('/');
                fileName = split[split.Length - 1];
                Addressables.LoadAssetAsync<Texture2D>(fileName).Completed += OnLoadDone;
			}
			else 
			{
				string photoUrl = string.Format("{0}{1}", URL.SERVER_PATH, data.info.img);
                Debug.Log($"{TAG} | item get photo - url : {photoUrl}");

				if (photoUrl.Contains(S.ALARM_ICON_TO_DO))
				{
                    _imgPhoto.texture = _texToDo;
                    loader.Resize();
				}
                else if (photoUrl.Contains(S.ALARM_ICON_MEETING))
                {
                    _imgPhoto.texture = _texMeeting;
                    loader.Resize();
                }
                else if (photoUrl.Contains(S.ALARM_ICON_SEMINAR))
                {
                    _imgPhoto.texture = _texSeminar;
                    loader.Resize();
                }
                else if (photoUrl.Contains(S.ALARM_ICON_CALL))
                {
                    _imgPhoto.texture = _texCall;
                    loader.Resize();
                }
                else
                {
                    loader.LoadImage(photoUrl).Forget();
                }
			}
        }

	#endregion  // GPM functions


    #region Event handling

        private void OnLoadDone(AsyncOperationHandle<Texture2D> obj) 
		{
			switch (obj.Status) 
			{
				case AsyncOperationStatus.Succeeded :
					obj.Result.hideFlags = HideFlags.HideAndDontSave;
					obj.Result.filterMode = FilterMode.Point;
					obj.Result.Apply();

					_imgPhoto.texture = obj.Result;
                    loader.Resize();

					operationHandle = obj; 
					break;

				case AsyncOperationStatus.Failed :
					Debug.Log($"{TAG} | Addressable texture load fail. image : {fileName}");
					break;

				default :
					break;
			}
		} 

        private void OnMenu() 
		{
			string title = LocalizationSettings.StringDatabase.GetLocalizedString("Menu", "알림.관리", R.singleton.CurrentLocale);

			int targetSeq = 0;
			switch (id) 
			{
				case S.ALARM_RESERVE_MEETING :
				case S.ALARM_UPDATE_MEETING :
				case S.ALARM_DELETE_MEETING :
				case S.ALARM_INVITE_MEETING :
				case S.ALARM_INVITE_MEETING_CANCEL :
				case S.ALARM_START_MEETING :
				case S.ALARM_DONE_MEETING :

				case S.ALARM_RESERVE_SEMINAR :
				case S.ALARM_UPDATE_SEMINAR :
				case S.ALARM_DELETE_SEMINAR :

				case S.ALARM_VOICE_CALL :
				case S.ALARM_REJECT_CALL :
				case S.ALARM_TO_DO :
					int.TryParse(data.info.contentJson, out targetSeq);
					break;

				case S.ALARM_UPDATE_MEMBER :
				case S.ALARM_UPDATE_SPACE :
				case S.ALARM_UPDATE_SEAT :

				case S.ALARM_TASK : 
				default :
					targetSeq = 0;
					break;
			}

			PopupBuilder.singleton.OpenSlide(title, 
				new string[] {
					LocalizationSettings.StringDatabase.GetLocalizedString("Menu", "알림.상세 확인", R.singleton.CurrentLocale),
					LocalizationSettings.StringDatabase.GetLocalizedString("Menu", "알림.읽음으로 표시", R.singleton.CurrentLocale),
					LocalizationSettings.StringDatabase.GetLocalizedString("Menu", "알림.삭제", R.singleton.CurrentLocale)
				}, 
				new string[] {	// alarm seq, tp id, target seq
					seq.ToString(),
					data.info.tp.id,
					targetSeq.ToString()
			 	}, ID.MobileScene_Alarm, true
			);
		} 

        private void OnDelete() 
        {
            Debug.Log($"{TAG} | OnDelete() call.");
        }

    #endregion  // Event handling
    }
}