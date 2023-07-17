/// <summary>
/// ContentTextTester 를 테스트하기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 07. 14
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 14) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Joycollab.v2
{
    public class ContentTextTester : MonoBehaviour
    {
        private const string TAG = "ContentTextTester";
        private const string HTTP = "http://";
        private const string HTTPS = "https://";

        [Header("input part")]
        [SerializeField] private TMP_InputField _inputContent;
        [SerializeField] private Button _btnUpdate;

        [Header("display part")]
        [SerializeField] private Transform _transform;
        [SerializeField] private GameObject _goContent;
        [SerializeField] private GameObject _goLink;


    #region Unity functions

        private void Awake() 
        {
            if (_transform == null) 
            {
                Debug.Log($"{TAG} | 내용물이 출력될 transform 을 먼저 설정해야 테스트를 진행할 수 있습니다.");
                return;
            }

            if (_goContent == null || _goLink == null) 
            {
                Debug.Log($"{TAG} | 내용물이 출력될 GameObject 를 먼저 설정해야 테스트를 진행할 수 있습니다.");
                return;
            }

            _inputContent.text = "- 월드 코지 모임방 바닥 수정 / 이미지 저장 / 기타 전반적인 UI 구성 수정 / UX 는 사용자 측면에서 보다 쓰기 좋게 작업할 필요가 있다고 생각함. \n- 월드 UI 수정 작업\n피그마 URL https://www.figma.com/file/Sbsk76Qnv1F51VOnlzxIwK/%EC%9B%94%EB%93%9CUI?type=design&node-id=0-1&mode=design&t=d6CSa33YcPmMEBIi-0 링크 확인 요망.\nhttps://www.naver.com 와 https://www.google.com 을 이용해보자.";

            _btnUpdate.onClick.AddListener(() => {
                ClearContent();
                ChangeContent();
            });
        }

        private void Start() 
        {
            _btnUpdate.onClick.Invoke();
        }

    #endregion  // Unity functions


    #region contents

        private void CreateContentBlock(string str) 
        {
            GameObject c = Instantiate(_goContent, Vector3.zero, Quaternion.identity);
            c.GetComponent<ContentText>().InitText(str);
            c.transform.SetParent(_transform, false);
        }

        private void CreateLinkBlock(string str) 
        {
            GameObject l = Instantiate(_goLink, Vector3.zero, Quaternion.identity);
            l.GetComponent<ContentLink>().InitLink(str);
            l.transform.SetParent(_transform, false);
        }

        private void ClearContent() 
        {
            Transform children = _transform.GetComponentInChildren<Transform>();
            foreach (Transform child in children) 
            {
                if (child.name.Equals(_transform.name)) continue;
                Destroy(child.gameObject);
            }
        }

        private void ChangeContent() 
        {
            // variables
            string t, sub;
            int idx, endIdx; 
            int spaceIdx, tabIdx, newLineIdx;


            t = _inputContent.text; 
            while (true) 
            {
                // 0. 더 이상 문장이 없다면 종료.
                if (t.Length < 1)
                {
                    // Debug.Log("current string length < 1. break the loop");
                    break;
                }

                // 1. 문자열에 HTTP, HTTPS 가 있는지 확인.
                if (t.Contains(HTTP) || t.Contains(HTTPS)) 
                {
                    idx = t.IndexOf("http");
                    // 1-1. 시작점에 http 가 있다면, white space char 를 체크해서 그 영역까지 link block 생성.
                    if (idx == 0) 
                    {
                        // Debug.Log($" {t}");
                        spaceIdx = t.IndexOf(" ");
                        tabIdx = t.IndexOf("\t");
                        newLineIdx = t.IndexOf("\n");

                        endIdx = -1;
                        // Debug.Log($" > {endIdx}");
                        endIdx = spaceIdx != -1 ? spaceIdx : endIdx;
                        // Debug.Log($" >> {endIdx}");
                        endIdx = (tabIdx != -1 && tabIdx < endIdx) ? tabIdx : endIdx;
                        // Debug.Log($" >>> {endIdx}");
                        endIdx = (newLineIdx != -1 && newLineIdx < endIdx) ? newLineIdx : endIdx;
                        // Debug.Log($" >>> {endIdx}");
                        if (endIdx == -1)   sub = t;
                        else                sub = t.Substring(0, endIdx);
                        // Debug.Log($"link block : {sub}");
                        CreateLinkBlock(sub);

                        if (endIdx == -1) 
                            break;
                        else
                            t = t.Substring(endIdx + 1);
                    }
                    // 1-2. 시작점에 없다면, 시작점부터 http 까지 content block 생성.
                    else 
                    {
                        sub = t.Substring(0, idx-1);
                        // Debug.Log($"content block : {sub}");
                        CreateContentBlock(sub);

                        t = t.Substring(idx);
                    }
                }
                // 2. 링크 prefix 가 없다면 그냥 content block 을 출력하고 종료.
                else 
                {
                    // Debug.Log($"content block : {t}, and break the loop");
                    CreateContentBlock(t);
                    break;
                }
            }


            /**
            var arr = _inputContent.text.Split('\n');
            foreach (string s in arr) 
            {
                t = s;

                while (true) 
                {
                    if (t.Length < 1)
                    {
                        // Debug.Log("current string length < 1. break the loop");
                        break;
                    }

                    // 1. 받은 문자열에 HTTP, HTTPS 가 있는지 확인
                    if (t.Contains(HTTP) || t.Contains(HTTPS)) 
                    {
                        idx = t.IndexOf("http"); 
                        if (idx == 0) 
                        {
                            endIdx = t.IndexOf((char)32);
                            if (endIdx == -1)   sub = t.Substring(0);
                            else                sub = t.Substring(0, endIdx);
                            // Debug.Log($"link block : {sub}");
                            CreateLinkBlock(sub);

                            if (endIdx == -1) 
                                break;
                            else 
                                t = t.Substring(endIdx + 1);
                        }
                        else 
                        {
                            sub = t.Substring(0, idx-1);
                            // Debug.Log($"content block : {sub}");
                            CreateContentBlock(sub);

                            t = t.Substring(idx);
                        }
                    }
                    else 
                    {
                        // Debug.Log($"content block : {t}, and break the loop");
                        CreateContentBlock(t);
                        break;
                    }
                }
            }
             */
        }

    #endregion  // contents
    }
}