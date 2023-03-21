using UnityEngine;

namespace Joycollab.v2
{
    public class SortingMesh : MonoBehaviour
    {
        [SerializeField] private eSortingType sortingType;
        [SerializeField] private string sortingLayer;
        private SpriteSorter sorter;
        private MeshRenderer meshRenderer;

    #region Unity functions
        private void Start()
        {
            sorter = FindObjectOfType<SpriteSorter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.sortingLayerName = sortingLayer;
            meshRenderer.sortingOrder = sorter.GetSortingOrder(gameObject);
        }

        // Update is called once per frame
        private void Update()
        {
            if (sortingType == eSortingType.Update) 
            {
                meshRenderer.sortingOrder = sorter.GetSortingOrder(gameObject);
            } 
        }
    #endregion
    }
}