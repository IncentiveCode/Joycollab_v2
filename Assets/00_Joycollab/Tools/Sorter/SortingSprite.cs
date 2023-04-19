using UnityEngine;

public enum eSortingType 
{
    Static, Update
}

public class SortingSprite : MonoBehaviour
{
    [SerializeField] private eSortingType sortingType;
    private SpriteSorter sorter;
    private SpriteRenderer spriteRenderer;


#region Unity functions
    private void Start()
    {
        sorter = FindObjectOfType<SpriteSorter>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = sorter.GetSortingOrder(gameObject);
    }

    private void Update() 
    {
        if (sortingType == eSortingType.Update) 
        {
            spriteRenderer.sortingOrder = sorter.GetSortingOrder(gameObject);
        }
    }
#endregion
}
