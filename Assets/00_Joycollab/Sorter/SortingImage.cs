using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
{
    public class SortingImage : MonoBehaviour
    {
        [SerializeField] private eSortingType sortingType;
        private SpriteSorter sorter;
        private Canvas canvas;


    #region Unity functions
        private void Start()
        {
            sorter = FindObjectOfType<SpriteSorter>();
            canvas = GetComponent<Canvas>();
            canvas.sortingOrder = sorter.GetSortingOrder(gameObject);
        }

        private void Update() 
        {
            if (sortingType == eSortingType.Update) 
            {
                canvas.sortingOrder = sorter.GetSortingOrder(gameObject);
            }
        }
    #endregion
    }
}