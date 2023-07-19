/// <summary>
/// 알림 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 07. 19
/// @version        : 0.2
/// @update
///     v0.1 (2023. 06. 28) : 최초 생성
///     v0.2 (2023. 07. 19) : GetAlarmList() 수정 - infinite scroll 을 변수로 추가.
/// </summary>

using UnityEngine;
using Gpm.Ui;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class AlarmModule : MonoBehaviour
    {
        private const string TAG = "AlarmModule";


    #region public functions
        
        public async UniTask<string> GetAlarmList(InfiniteScroll view) 
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.ALARM_LIST, memberSeq);
            PsResponse<ResAlarmList> res = await NetworkTask.RequestAsync<ResAlarmList>(url, eMethodType.GET, string.Empty, token);

            int unreadCnt = 0;
            if (string.IsNullOrEmpty(res.message)) 
            {
                view.Clear();
                R.singleton.ClearAlarmInfo();

                AlarmData t;
                foreach (var item in res.data.list) 
                {
                    t = new AlarmData(item);
                    view.InsertData(t);

                    R.singleton.AddAlarmInfo(item);
                    if (! item.read) unreadCnt ++;
                }

                R.singleton.UnreadAlarmCount = unreadCnt;
            }
            else 
            {
                view.Clear();
            }

            return res.message;
        }

        public async UniTask<string> ReadAlarm(int alarmSeq) 
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.READ_ALARM, memberSeq, alarmSeq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PATCH, string.Empty, token);

            if (string.IsNullOrEmpty(res.message))
                return string.Empty;
            else 
                return res.message;
        }

        public async UniTask<string> TruncateAlarm() 
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.TRUNCATE_ALARM, memberSeq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.DELETE, string.Empty, token);

            if (string.IsNullOrEmpty(res.message))
                return string.Empty;
            else 
                return res.message;
        }

    #endregion  // public functions
    }
}