/// <summary>
/// 시스템 상 저장 공간 (Storage) 
/// @author         : HJ Lee
/// @last update    : 2023. 03. 17
/// @version        : 0.1
/// @update
///     v0.1 (2023. 03. 17) : 파일 생성, Joycollab 에서 사용하는 것들 정리. (정리 중)
/// </summary>

namespace Joycollab.v2
{
    public class Storage
    {
    #region Token
        private static string _id;
        private static string _accessToken;
        private static string _tokenType; 
        private static string _refreshToken;
        private static int _tokenExpire;
        private static string _tokenScope;

        public static string ID {
            get { return _id; }
            set { _id = value; }
        }
        public static string accessToken {
            get { return _accessToken; }
            set { _accessToken = value; }
        }
        public static string tokenType {
            get { return _tokenType; } 
            set { _tokenType = value; }
        }
        public static string refreshToken {
            get { return _refreshToken; }
            set { _refreshToken = value; }
        }
        public static int tokenExpire {
            get { return _tokenExpire; }
            set { _tokenExpire = value; }
        } 
        public static string tokenScope {
            get { return _tokenScope; }
            set { _tokenScope = value; }
        }
        public static string token {
            get { return string.Format("{0} {1}", _tokenType, _accessToken); }
        }
    #endregion   


    #region Current Workspace Info
        private static string _domainName;
        private static int _workspaceSeq;
        private static int _memberSeq;
        private static string _nickName;
        private static string _photoUrl;

        public static string domainName {
            get { return _domainName; }
            set { _domainName = value; }
        }
        public static int workspaceSeq {
            get { return _workspaceSeq; }
            set { _workspaceSeq = value; }
        }
        public static int memberSeq {
            get { return _memberSeq; }
            set { _memberSeq = value; }
        }
        public static string nickName {
            get { return _nickName; }
            set { _nickName = value; }
        }
        public static string photoUrl {
            get { return _photoUrl; }
            set { _photoUrl = value; }
        }
    #endregion // Current Workspace Info
    }
}