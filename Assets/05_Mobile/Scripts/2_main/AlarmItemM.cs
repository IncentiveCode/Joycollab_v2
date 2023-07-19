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
using UnityEngine.ResourceManagement.AsyncOperations;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2 
{
	public class AlarmItemM : InfiniteScrollItem 
	{
		[Header("image")]
		[SerializeField] private RawImage _imgPhoto;
		[SerializeField] private Image _imgMark;

		[Header("button")]
		[SerializeField] private Button _btnItem;

		[Header("text")]
		[SerializeField] private TMP_Text _txtTitle;
		[SerializeField] private TMP_Text _txtContent;
		[SerializeField] private TMP_Text _txtDate;

		// local variables
		private int seq;	
		private bool isRead;
		private XmppTaskInfo task;
	    private AsyncOperationHandle<Texture2D> operationHandle;


	#region Unity functions

		private void Awake() 
		{
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
			bool isMatched = false;

			// 기본 정보 설정
			_imgMark.gameObject.SetActive(! data.info.read);
			_txtTitle.text = data.info.title;
			_txtDate.text = data.info.dtm;

			// TODO. set text
			switch (data.info.tp.id) 
			{
				case S.ALARM_RESERVE_MEETING :
				case S.ALARM_UPDATE_MEETING :
				case S.ALARM_DELETE_MEETING :
				case S.ALARM_INVITE_MEETING :
				case S.ALARM_INVITE_MEETING_CANCEL :

				case S.ALARM_RESERVE_SEMINAR :
				case S.ALARM_UPDATE_SEMINAR :
				case S.ALARM_DELETE_SEMINAR :
					foreach (var item in R.singleton.myAlarmOpt.alarmOptItems) 
					{
						if (item.tp.id.Equals(S.ALARM_ID_RESERVE_MEETING) && item.alarm)
						{
							_txtContent.text = data.info.content;
							isMatched = true;
							break;
						}
					}
					break;

				case S.ALARM_START_MEETING :
					foreach (var item in R.singleton.myAlarmOpt.alarmOptItems) 
					{
						if (item.tp.id.Equals(S.ALARM_ID_START_MEETING) && item.alarm) 
						{
							_txtContent.text = data.info.content;
							isMatched = true;
							break;
						}
					}
					break;

				case S.ALARM_DONE_MEETING :
					foreach (var item in R.singleton.myAlarmOpt.alarmOptItems) 
					{
						if (item.tp.id.Equals(S.ALARM_ID_DONE_MEETING) && item.alarm) 
						{
							_txtContent.text = data.info.content;
							isMatched = true;
							break;
						}
					}
					break;

				case S.ALARM_VOICE_CALL :
					foreach (var item in R.singleton.myAlarmOpt.alarmOptItems) 
					{
						if (item.tp.id.Equals(S.ALARM_ID_REQUEST_VOICE) && item.alarm) 
						{
							_txtContent.text = data.info.content;
							isMatched = true;
							break;
						}
					}
					break;

				case S.ALARM_REJECT_CALL :
					foreach (var item in R.singleton.myAlarmOpt.alarmOptItems) 
					{
						if (item.tp.id.Equals(S.ALARM_ID_REJECT_VOICE) && item.alarm) 
						{
							_txtContent.text = data.info.content;
							isMatched = true;
							break;
						}
					}
					break;
			}
			

			// TODO. set texture


		}

	#endregion	// GPM functions
	}
}