/// <summary>
/// 게시판 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 07. 06
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 06) : 최초 생성 
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class BoardModule : MonoBehaviour
    {
        private const string TAG = "BoardModule";


    #region public functions

        /**
        public async UniTask<PsResponse<ResToDoList>> GetList(int spaceSeq)
        {
            string token = R.singleton.token;
            int workspaceSeq = R.singleton.workspaceSeq;

            string url = string.Format(URL.
            PsResponse<ResToDoList> res = await NetworkTask.RequestAsync<ResToDoList>(url, eMethodType.GET, string.Empty, token);

            return res;
        }

        public async UniTask<PsResponse<string>> CheckItem(int seq) 
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.CONTROL_TODO, memberSeq, seq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PUT, string.Empty, token);

            return res;
        }

        public async UniTask<PsResponse<string>> SaveToDo(string body, int shareOpt, int remindOpt)
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.REGIST_TODO, memberSeq, shareOpt, remindOpt);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.POST, body, token);

            return res;
        }

        public async UniTask<PsResponse<string>> UpdateToDo(int seq, string body, int shareOpt, int remindOpt)
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.MODIFY_TODO, memberSeq, seq, shareOpt, remindOpt);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PUT, body, token);

            return res;
        }

        public async UniTask<PsResponse<string>> DeleteToDo(int seq) 
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.CONTROL_TODO, memberSeq, seq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.DELETE, string.Empty, token);

            return res;
        }
         */

    #endregion  // public functions
    }
}