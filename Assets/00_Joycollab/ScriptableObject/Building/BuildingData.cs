/// <summary>
/// Building data 를 위한 scriptable object class 
/// @author         : HJ Lee
/// @last update    : 2023. 10. 23
/// @version        : 0.1
/// @update
///     v0.1 (2023. 10. 23) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
{
    [CreateAssetMenu(fileName="Building Data", menuName="ScriptableObject/Building Data", order=int.MaxValue)]
    public class BuildingData : ScriptableObject
    {
        private const string TAG = "BuildingData";

        [SerializeField] private string _name;
        [SerializeField] private Texture2D _logo;
        [SerializeField] private string _joycollab;
        [SerializeField] private string _homepage;
        [SerializeField] private string _tel;
        [SerializeField] private string _address;


    #region public functions
    
        public string BuildingName => _name;
        public Texture2D Logo => _logo;
        public bool UsingJoycollab => !string.IsNullOrEmpty(_joycollab);
        public string JoycollabLink => _joycollab;
        public bool UsingHomepage => !string.IsNullOrEmpty(_homepage);
        public string HomepageLink => _homepage;
        public string Tel => _tel;
        public string Address => _address;

    #endregion  // public functions
    }
}