/// <summary>
/// OKR 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 07. 04
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 04) : 최초 생성 및 작업 중.
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class OkrModule : MonoBehaviour
    {
        private const string TAG = "OkrModule";


    #region public functions

        public async UniTask<PsResponse<ResOkrList>> GetList(string url)
        {
            string token = R.singleton.token;

            PsResponse<ResOkrList> res = await NetworkTask.RequestAsync<ResOkrList>(url, eMethodType.GET, string.Empty, token);
            return res;
        }

        public async UniTask<PsResponse<ResOkrList>> GetObjectives(string url)
        { 
            string token = R.singleton.token;

            PsResponse<ResOkrList> res = await NetworkTask.RequestAsync<ResOkrList>(url, eMethodType.GET, string.Empty, token);
            return res;
	    }

        public async UniTask<PsResponse<string>> SaveObjective(string body, int shareOpt)
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.REGIST_OBJECTIVE, memberSeq, shareOpt);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.POST, body, token);

            return res;
        }

        public async UniTask<PsResponse<string>> SaveKeyResult(string body, int objectiveSeq)
        { 
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.REGIST_KEY_RESULT, memberSeq, objectiveSeq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.POST, body, token);

            return res;
	    }

        public async UniTask<PsResponse<string>> UpdateOkr(int seq, string body)
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.CONTROL_OKR, memberSeq, seq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PUT, body, token);

            return res;
        }

        public async UniTask<PsResponse<string>> DeleteOkr(int seq) 
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.CONTROL_OKR, memberSeq, seq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.DELETE, string.Empty, token);

            return res;
        }

    #endregion  // public functions
    }
}