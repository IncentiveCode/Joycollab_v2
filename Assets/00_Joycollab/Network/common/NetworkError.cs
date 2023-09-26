/// <summary>
/// Network 통신 - 실패 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 02. 20
/// @version        : 0.1
/// @update
///     v0.1 (2023. 02. 20) : Joycollab 에서 사용하던 클래스 정리 및 통합
/// </summary>

using System;

namespace Joycollab.v2
{
    [Serializable]
    public class ErrorMsg
    {
        public string code;
        public string field;
        public string value;
        public string errorMsg;
    }

    [Serializable]
    public class ResBadRequest
    {
        public string path;
        public ErrorMsg[] errorsMsg;
        public string error;
        public string message;
        public long status;
        public string timestamp;

        public string Error {
            get { return this.error; }
        }

        public string Message {
            get { return GetErrorMessage(); }
        }

        private string GetErrorMessage() 
        {
            string errors = "";

            if (this.errorsMsg != null) 
            {
                int i = 1;
                foreach (ErrorMsg msg in errorsMsg)
                {
                    errors += msg.errorMsg;
                    if (i++ != errorsMsg.Length)
                        errors += ", ";
                }
            }
            
            return errors;
        }
    }

    [Serializable]
    public class SimpleError
    {
        public string error;
        public string error_description;
    }
}