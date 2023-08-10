/// <summary>
/// Network 통신 - 공지사항 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 08. 10
/// @version        : 0.1
/// @update
///     v0.1 (2023. 08. 10) : v1 에서 사용하던 '업데이트 공지사항' 클래스 정리 및 통합
/// </summary>

using System;
using System.Collections.Generic;
using Gpm.Ui;

namespace Joycollab.v2
{
    [Serializable]
    public class UpdateNoticeList 
    {
        public List<UpdateNotice> list;
    }

    [Serializable]
    public class UpdateNotice 
    {
        public int seq;
        public string title;
        public string content;
        public string sdtm;
        public string edtm;
    }
}