/// <summary>
/// 알림 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 06. 28
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 28) : 최초 생성
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class AlarmModule : MonoBehaviour
    {
        private const string TAG = "AlarmModule";


    #region public functions
        
        public async UniTask<PsResponse<ResAlarmList>> GetAlarmList() 
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.ALARM_LIST, memberSeq);
            PsResponse<ResAlarmList> res = await NetworkTask.RequestAsync<ResAlarmList>(url, eMethodType.GET, string.Empty, token);

            return res;
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