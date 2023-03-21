using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Joycollab.v2
{
    public class AddressableLoader : MonoBehaviour
    {
        [SerializeField] private eRendererType _type;
        [SerializeField] private bool _isMap;
        [SerializeField] private string _key;

        private Image image;
        private SpriteRenderer spriteRenderer;
        private Color spriteColor;

        private AsyncOperationHandle<Sprite> operationHandle;


    #region Unity functions
        private void Awake() 
        {
            if (_type == eRendererType.UI_Image)
            {
                image = GetComponent<Image>();
                spriteRenderer = null;
            }
            else 
            {
                image = null;
                spriteColor = new Color(1f, 1f, 1f, 0f);
                spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.color = spriteColor;
            }
        }

        private void Start() 
        {
            if (string.IsNullOrEmpty(_key)) return;

            Addressables.LoadAssetAsync<Sprite>(_key).Completed += OnLoadDone;
        }

        private void OnDestroy() 
        {
            if (operationHandle.IsValid())
                Addressables.Release(operationHandle);
        }
    #endregion  // Unity functions


    #region complete function
        private void OnLoadDone(AsyncOperationHandle<Sprite> handle) 
        {
            switch (handle.Status) 
            {
                case AsyncOperationStatus.Succeeded :
                    if (_type == eRendererType.UI_Image)
                    {
                        image.sprite = handle.Result;
                        if (! _isMap) image.alphaHitTestMinimumThreshold = 0.1f;
                    }
                    else 
                    {
                        spriteRenderer.sprite = handle.Result;
                        spriteColor.a = 1f;
                        spriteRenderer.color = spriteColor;
                    }

                    operationHandle = handle;
                    break;

                case AsyncOperationStatus.Failed :
                    Debug.LogError($"Addressable | Sprite load fail. name : {_key}");
                    break;

                default : break;
            }
        }
    #endregion  // complete function
    }
}
