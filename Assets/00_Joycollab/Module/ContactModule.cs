/// <summary>
/// 연락처 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 06. 27
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 27) : 최초 생성
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2 
{
    public class ContactModule : MonoBehaviour
    {
        private const string TAG = "ContactModule"; 


    #region public functions

        public async UniTask<PsResponse<WorkspaceMemberList>> GetContact() 
        {
            string token = R.singleton.token;
            int workspaceSeq = R.singleton.workspaceSeq;

            string url = string.Format(URL.WORKSPACE_MEMBER_LIST, workspaceSeq);
            PsResponse<WorkspaceMemberList> res = await NetworkTask.RequestAsync<WorkspaceMemberList>(url, eMethodType.GET, string.Empty, token);

            return res;
        }

    #endregion  // public functions
    }
}