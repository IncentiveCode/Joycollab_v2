using UnityEngine;

namespace Joycollab.v2
{
    public class SpriteSorter : MonoBehaviour
    {
        [SerializeField] private Transform Back;
        [SerializeField] private Transform Front;

        public int GetSortingOrder(GameObject obj) 
        {
            float objDist = Mathf.Abs(Back.position.y - obj.transform.position.y);
            float totalDist = Mathf.Abs(Back.position.y - Front.position.y);

            return (int) (Mathf.Lerp(0, System.Int16.MaxValue, objDist / totalDist));
        }
    }
}