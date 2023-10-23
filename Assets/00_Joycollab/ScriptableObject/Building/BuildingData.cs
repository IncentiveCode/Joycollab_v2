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

        public string BuildingName 
        {
            get { return _name; }
        }

        public Texture2D Logo
        {
            get { return _logo; }
        }

        public bool UsingJoycollab
        {
            get { return string.IsNullOrEmpty(_joycollab); }
        }
        public string JoycollabLink
        {
            get { return _joycollab; }
        }

        public bool UsingHomepage
        {
            get { return string.IsNullOrEmpty(_homepage); }
        }
        public string HomepageLink
        {
            get { return _homepage; }
        }

        public string Tel
        {
            get { return _tel; }
        }

        public string Address
        {
            get { return _address; }
        }

        #endregion  // public functions
    }
}