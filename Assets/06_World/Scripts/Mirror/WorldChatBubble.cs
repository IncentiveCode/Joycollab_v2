/// <summary>
/// Square 에서 사용할 instant message bubble class
/// @author         : HJ Lee
/// @last update    : 2023. 09. 20 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 20) : 최초 생성
/// </summary>

using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;

namespace Joycollab.v2
{
    public class WorldChatBubble : MonoBehaviour
    {
        private const string TAG = "WorldChatBubble";
        private const float OPENING_TIME = 1f;
        private const int DISPLAY_TIME = 8;
        private const float CLOSING_TIME = 3f;

        [SerializeField] private SpriteRenderer _rendererBubble;
        [SerializeField] private TMP_Text _txtChat; 

        private Sequence sequence;
        private Guid uid;


    #region Bubble functions

        public static void Create(Transform parent, Vector3 localPosition, string chat) 
        {
            foreach (Transform child in parent.GetComponentInChildren<Transform>()) 
            {
                if (child.name.Equals(parent.name)) continue;
                if (child.TryGetComponent(out WorldChatBubble oldBubble))
                {
                    oldBubble.Stop();
                }
            }

            Transform transformBubble = Instantiate(SystemManager.singleton.pfChatBubble, parent);
            transformBubble.localPosition = localPosition;

            WorldChatBubble bubble = transformBubble.GetComponent<WorldChatBubble>();
            bubble.Setup(chat);
        }

        public void Stop() 
        {
            if (sequence != null) 
            {
                // Debug.Log($"Stop() call. uid : {uid}");
                DOTween.Kill(uid);
                sequence = null;
                Destroy(gameObject);
            }
        }

        private void Setup(string chat) 
        {
            _txtChat.text = chat;
            _txtChat.ForceMeshUpdate();

            Vector2 chatSize = _txtChat.GetRenderedValues(false);
            Vector2 padding = new Vector2(0.6f, 0.6f);
            _rendererBubble.size = chatSize + padding;

            sequence = DOTween.Sequence()
                .Append(_rendererBubble.DOFade(1, OPENING_TIME)) 
                .Join(_txtChat.DOFade(1, OPENING_TIME))
                .AppendInterval(DISPLAY_TIME)
                .Append(_rendererBubble.DOFade(0, CLOSING_TIME))
                .Append(_txtChat.DOFade(0, CLOSING_TIME))
                .AppendCallback(() => Destroy(gameObject));

            uid = Guid.NewGuid();
            // Debug.Log($"Setup() call. uid : {uid}");
            sequence.id = uid;
            sequence.Play();
        }

    #endregion  // Bubble functions
    }
}