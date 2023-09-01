/// <summary>
/// WorldScene, 건물 위에 사용할 포인터 표시 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 09. 01 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 01) : v1 에서 사용하던 항목 수정 후 적용.
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
    public class BuildingPointer : MonoBehaviour
    {
        [Header("Default")]
        [SerializeField] private eRendererType _rendererType;
        [SerializeField] private bool _isMovable;

        [Header("Move Parameter")]
        [SerializeField] private float _moveLimit = 5f;
        [SerializeField] private float _speed = 2f;

        private int _direction = 1;
        private bool run;

        // position variables
        private RectTransform rect; 
        private Vector2 pos;
        private Vector3 rotate;
        private float currentY, minY, maxY;


    #region Unity functions

        private void Awake() 
        {
            if (_rendererType == eRendererType.UI_Image)
            {
                rect = GetComponent<RectTransform>();
                pos = rect.anchoredPosition;
                rotate = rect.eulerAngles;
            }
            else if (_rendererType == eRendererType.SpriteRenderer) 
            {
                pos = transform.position;
                rotate = transform.eulerAngles;
            }

            currentY = pos.y;
            minY = currentY - _moveLimit;
            maxY = currentY + _moveLimit;
        }

        private void Update() 
        {
            if (! run) return;

            pos.y += Time.deltaTime * _speed * _direction;
            rotate.y += Time.deltaTime * 90f;

            if (_rendererType == eRendererType.UI_Image)
            {
                rect.anchoredPosition = pos;
                rect.eulerAngles = rotate;
            }
            else if (_rendererType == eRendererType.SpriteRenderer) 
            {
                transform.position = pos;
                transform.eulerAngles = rotate;
            }

            if (pos.y >= maxY) _direction = -1;
            if (pos.y <= minY) _direction = 1;
        }

        private void OnEnable() 
        {
            run = _isMovable;
        }

        private void OnDisable() 
        {
            run = false;     
        }

    #endregion
    }
}