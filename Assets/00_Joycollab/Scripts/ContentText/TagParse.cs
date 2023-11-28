/// <summary>
/// 내용을 받아서 태그를 분리하는 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 11. 28
/// @version        : 0.2
/// @update
///     v0.1 (2023. 11. 20) : 최초 생성
///     v0.2 (2023. 11. 28) : 출력 방식 수정 >> # 없어도 클릭 가능한 태그로 출력.
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Joycollab.v2
{
    public class TagParse : MonoBehaviour
    {
        private const string Tag = "TagParser";

        [Header("input part")]
        [SerializeField] private TMP_InputField inputTag;
        [SerializeField] private Button btnCopy;
        [SerializeField] private Button btnUpdate;

        [Header("display part")]
        [SerializeField] private Transform tagTransform;
        [SerializeField] private GameObject goTag;

        private string _originTag;


    #region Unity functions

        private void Awake() 
        {
            if (tagTransform == null) 
            {
                Debug.Log($"{Tag} | 태그가 출력될 transform 을 먼저 설정해야 테스트를 진행할 수 있습니다.");
                return;
            }

            if (goTag == null) 
            {
                Debug.Log($"{Tag} | 태그가 출력될 GameObject 를 먼저 설정해야 테스트를 진행할 수 있습니다.");
                return;
            }

            if (inputTag != null)
            {
                inputTag.text =
                    "#롤드컵 #챔피언 #T1 #Zeus #Oner #Faker #Gumayusi #Keria \n#4번째우승 #우여곡절_끝에_피는_꽃\n대한민국 파이팅\nLCK 파이팅";
            }

            if (btnCopy != null)
            {
                btnCopy.onClick.AddListener(() => {
                    Debug.Log($"{Tag} | copy contents.");
                    JsLib.CopyToClipboard(_originTag);
                });
            }

            if (btnUpdate != null)
            {
                btnUpdate.onClick.AddListener(() => Init(inputTag.text));
            }
        }

        private void Start() 
        {
            if (inputTag != null)
            {
                Init(inputTag.text);
            }
        }

    #endregion  // Unity functions


    #region tag parsing functions
    
        public void Init(string str) 
        {
            _originTag = str;

            ClearTag();
            ChangeTag();
        }

        private void CreateTagBlock(string str) 
        {
            var c = Instantiate(goTag, Vector3.zero, Quaternion.identity);
            c.GetComponent<TagLink>().InitTag(str);
            c.transform.SetParent(tagTransform, false);
        }

        private void CreatePlainBlock(string str) 
        {
            var c = Instantiate(goTag, Vector3.zero, Quaternion.identity);
            c.GetComponent<TagLink>().InitPlain(str);
            c.transform.SetParent(tagTransform, false);
        }

        private void ClearTag() 
        {
            var children = tagTransform.GetComponentInChildren<Transform>();
            foreach (Transform child in children) 
            {
                if (child.name.Equals(tagTransform.name)) continue;
                Destroy(child.gameObject);
            }
        }

        private void ChangeTag() 
        {
            var t = (inputTag != null) ? inputTag.text : _originTag; 
            while (true) 
            {
                // 0. 더 이상 문장이 없다면 종료.
                if (t.Length < 1)
                {
                    // Debug.Log("current string length < 1. break the loop");
                    break;
                }

                // Debug.Log($" {t}");
                var spaceIdx = t.IndexOf(" ", StringComparison.Ordinal);
                var tabIdx = t.IndexOf("\t", StringComparison.Ordinal);
                var newLineIdx = t.IndexOf("\n", StringComparison.Ordinal);

                var endIdx = -1;
                // Debug.Log($" > {endIdx}");
                endIdx = spaceIdx != -1 ? spaceIdx : endIdx;
                // Debug.Log($" >> {endIdx}");
                endIdx = (tabIdx != -1 && tabIdx < endIdx) ? tabIdx : endIdx;
                // Debug.Log($" >>> {endIdx}");
                endIdx = (newLineIdx != -1 && newLineIdx < endIdx) ? newLineIdx : endIdx;
                // Debug.Log($" >>> {endIdx}");
                var sub = endIdx == -1 ? t : t[..endIdx];

                // Debug.Log($"tag block : {sub}");
                if (! string.IsNullOrWhiteSpace(sub)) CreateTagBlock(sub);

                if (endIdx == -1) 
                    break;
                else
                    t = t[(endIdx + 1)..];
            }
        }

    #endregion  // tag parsing functions
    }
}