/// <summary>
/// [mobile]
/// Alarm Item Script
/// @author         : HJ Lee
/// @last update    : 2023. 07. 19
/// @version        : 0.2
/// @update
///     v0.1 (2023. 06. 14) : 최초 생성
///     v0.2 (2023. 07. 19) : Addressable asset 의 thumbnail 불러오는 기능 추가. delete button 삭제. content 내용 정리.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2 
{
	public class AlarmItemM : InfiniteScrollItem 
	{
		private const string TAG = "AlarmItemM";

		[Header("image")]
		[SerializeField] private RawImage _imgPhoto;
		[SerializeField] private Image _imgMark;

		[Header("button")]
		[SerializeField] private Button _btnItem;

		[Header("text")]
		[SerializeField] private TMP_Text _txtTitle;
		[SerializeField] private TMP_Text _txtContent;
		[SerializeField] private TMP_Text _txtDate;

		[Header("for texture")]
		[SerializeField] private Vector2 _v2PhotoSize;
		[SerializeField] private Texture2D _texDefault;
		[SerializeField] private Texture2D _texSeminar;

		// local variables
		private int seq;	
		private bool isRead;

		private RectTransform rectPhoto;
	    private AsyncOperationHandle<Texture2D> operationHandle;


	#region Unity functions

		private void Awake() 
		{
			rectPhoto = _imgPhoto.GetComponent<RectTransform>();

			_btnItem.onClick.AddListener(OnSelect);
		}

	#endregion	// Unity functions


	#region GPM functions

		public override void UpdateData(InfiniteScrollData itemData) 
		{
			base.UpdateData(itemData);

			AlarmData data = (AlarmData) itemData;
			this.seq = data.info.seq;
			bool isArrange = false;

			// 기본 정보 설정
			_imgMark.gameObject.SetActive(! data.info.read);
			_txtTitle.text = data.info.title;
			_txtDate.text = data.info.dtm;

            Locale currentLocale = LocalizationSettings.SelectedLocale;
			switch (data.info.tp.id) 
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
					_txtContent.text = data.info.content;
					break;

				case S.ALARM_UPDATE_MEMBER :
                    _txtContent.text = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", "Member.정보 변경 안내", currentLocale);
					break;

				case S.ALARM_UPDATE_SPACE :
					isArrange = true;
                    _txtContent.text = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", "Space.정보 변경 안내", currentLocale);
					break;

				case S.ALARM_UPDATE_SEAT :
                    _txtContent.text = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", "Seat.정보 변경 안내", currentLocale);
					break;
				
				case S.ALARM_TASK :
					XmppTaskInfo task = JsonUtility.FromJson<XmppTaskInfo>(data.info.contentJson);
	                string content = data.info.content.Replace(XmppManager.CONTENT_SPLITTER, "|");
					var arrContent = content.Split('|');
					string c0 = arrContent[0].Replace(XmppManager.TASK_SPLITTER, "|");
                	var arr = c0.Split('|'); 

					string res, key, value;
					if (arr.Length > 1) 
					{
						key = "Kanban."+ arr[0];
						value = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", key, currentLocale);
						res = string.Format(value, arr[1]); 
					}
					else
					{
						key = "Kanban."+ content;
						res = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", key, currentLocale);
					}

					if (arrContent.Length > 1) 
					{
						res += " ";

						value = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", "Kanban.여러 항목 수정", currentLocale);
						res += string.Format(value, (arrContent.Length - 1)); 
					}
					_txtContent.text = res;
					break;
				
				default :
					Debug.LogWarning($"{TAG} | 알 수 없는 알림 id : {data.info.tp.id}");
					_txtContent.text = string.Empty;
					break;
			}
			

			// TODO. set texture
			if (string.IsNullOrEmpty(data.info.img)) 
			{
				_imgPhoto.texture = _texDefault;
				Util.ResizeRawImage(rectPhoto, _imgPhoto, _v2PhotoSize.x, _v2PhotoSize.y);
			}


		}

	#endregion	// GPM functions
	}
}