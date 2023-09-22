/// <summary>
/// 설정 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 09. 20
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 20) : 최초 생성
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class SettingModule : MonoBehaviour
    {
        private const string TAG = "SettingModule";

        
    #region 개인 설정

        public async UniTask<PsResponse<ResMemberInfo>> GetMyInfo() 
        {
            string url = string.Format(URL.MEMBER_INFO, R.singleton.memberSeq);
            PsResponse<ResMemberInfo> res = await NetworkTask.RequestAsync<ResMemberInfo>(url, eMethodType.GET, string.Empty, R.singleton.token);

            return res;
        }

        public async UniTask<string> UpdateMyInfo(string body) 
        {
            string url = string.Format(URL.MEMBER_INFO, R.singleton.memberSeq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PUT, body, R.singleton.token);

            return res.message;
        }

        public async UniTask<string> UpdateMyCompanyInfo(string body) 
        {
            string url = string.Format(URL.COMPANY_INFO, R.singleton.memberSeq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PUT, body, R.singleton.token);

            return res.message;
        }

    #endregion  // 개인 설정


    #region 환경 설정

        public async UniTask<string> UpdateEnvironment(ReqMemberEnvironmentInfo info) 
        {
            string url = string.Format(URL.SET_ENVIRONMENT, R.singleton.memberSeq,
                info.dateFormatStr, info.fontSize, info.hourFormatStr, info.lanId, info.timeZone, info.weekStart
            );
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PATCH, string.Empty, R.singleton.token);

            return res.message;
        }

        public async UniTask<PsResponse<TpsList>> GetAlarmContents() 
        {
            string url = string.Format(URL.GET_CODE, "알림음 구분");
            PsResponse<TpsList> res = await NetworkTask.RequestAsync<TpsList>(url, eMethodType.GET, string.Empty, R.singleton.token);

            return res; 
        }

        public async UniTask<string> UpdateAlarmOptions(string body) 
        {
            string url = string.Format(URL.SET_ALARM_OPTION, R.singleton.memberSeq, R.singleton.myAlarmOpt.seq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PATCH, body, R.singleton.token);

            return res.message;
        }

    #endregion  // 환경 설정
    }
}