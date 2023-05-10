/// <summary>
/// Login 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 05. 09
/// @version        : 0.1
/// @update
///     v0.1 (2023. 05. 09) : 최초 생성
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class LoginModule : MonoBehaviour
    {

    #region public functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pw"></param>
        /// <param name="complete"></param>
        /// <returns></returns>
        public async UniTask<string> LoginAsync(string id, string pw, System.Action complete) 
        {
            WWWForm form = new WWWForm();
            form.AddField(NetworkTask.GRANT_TYPE, NetworkTask.GRANT_TYPE_PW);
            form.AddField(NetworkTask.PASSWORD, pw);
            form.AddField(NetworkTask.REFRESH_TOKEN, string.Empty);
            form.AddField(NetworkTask.SCOPE, NetworkTask.SCOPE_ADM);
            form.AddField(NetworkTask.USERNAME, id);

            PsResponse<ResToken> res = await NetworkTask.PostAsync<ResToken>(URL.REQUEST_TOKEN, form, string.Empty, NetworkTask.BASIC_TOKEN);
            if (string.IsNullOrEmpty(res.message)) 
            {
                complete.Invoke(); 
                return string.Empty;
            }
            else 
            {
                return res.message;
            }
        }

    #endregion  // public functions

    }
}