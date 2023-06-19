/// <summary>
/// Network 통신 - 파일 관련 요청과 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 06. 19
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 19) : Joycollab 에서 사용하던 클래스 정리 및 통합, infinite scroll data 추가.
/// </summary>

using System;
using System.Collections.Generic;
using Gpm.Ui;

namespace Joycollab.v2 
{
    [Serializable]
    public class ResFileList
    {
        public List<ResFileInfo> list; 
    }

    [Serializable]
    public class ResFileInfo 
    {
        public string useYn;
        public int seq = -1;
        public WorkspaceStatus workspace;
        public WorkspaceStatus space;
        public string rootFolder;
        public string name;
        public int fileSize;
        public string isFile;
        public string dtm;
        public string mdtm;
        public MemberName createMember;
        public MemberName modifyMember;
    }

    public class FileData : InfiniteScrollData 
    {
        public ResFileInfo info;
        public int spaceSeq;

        public FileData() 
        {
            info = new ResFileInfo();
            spaceSeq = -1;
        }
    }
}