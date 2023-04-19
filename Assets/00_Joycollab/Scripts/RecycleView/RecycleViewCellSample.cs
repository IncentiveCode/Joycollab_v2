using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
{
    public class UICellSampleData 
    {
        public int index;
        public string name;
    }

    public class RecycleViewCellSample : RecycleViewCell<UICellSampleData> 
    {
        [SerializeField] private Text txtIndex;
        [SerializeField] private Text txtName;        

        public override void UpdateContent(UICellSampleData item) 
        {
            txtIndex.text = item.index.ToString();
            txtName.text = item.name;
        }

        public void onClickButton() 
        {
            string temp = string.Format("index : {0}, name : {1}", txtIndex.text, txtName.text);
            Debug.Log(temp);
        }
    }
}
