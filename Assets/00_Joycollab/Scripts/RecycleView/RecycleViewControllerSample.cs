using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
{
    public class RecycleViewControllerSample : RecycleViewController<UICellSampleData>
    {
    #region Public function
        public void OnPressCell(RecycleViewCellSample cell) 
        {
            Debug.Log("Cell Click");
        }
    #endregion


    #region Protected function
        protected override void Start() 
        {
            base.Start();

            LoadData();
        }
    #endregion


    #region Private function
        // 리스트 항목의 데이터를 읽어 들이는 메서드
        private void LoadData()
        {
            // 일반적인 데이터는 데이터 소스로부터 가져오는데 여기서는 하드 코드를 사용해하여 정의한다
            tableData.Clear();
            tableData = new List<UICellSampleData>() {
                new UICellSampleData { index=1, name="One"},
                new UICellSampleData { index=2, name="Two" },
                new UICellSampleData { index=3, name="Three" },
                new UICellSampleData { index=4, name="Four" },
                new UICellSampleData { index=5, name="Five" },
                new UICellSampleData { index=6, name="Six" },
                new UICellSampleData { index=7, name="Seven" },
                new UICellSampleData { index=8, name="Eight" },
                new UICellSampleData { index=9, name="Nine" },
                new UICellSampleData { index=10,name="Ten" },
                new UICellSampleData { index=11, name="One"},
                new UICellSampleData { index=12, name="Two" },
                new UICellSampleData { index=13, name="Three" },
                new UICellSampleData { index=14, name="Four" },
                new UICellSampleData { index=15, name="Five" },
                new UICellSampleData { index=16, name="Six" },
                new UICellSampleData { index=17, name="Seven" },
                new UICellSampleData { index=18, name="Eight" },
                new UICellSampleData { index=19, name="Nine" },
                new UICellSampleData { index=20,name="Ten" },
                new UICellSampleData { index=31, name="One"},
                new UICellSampleData { index=32, name="Two" },
                new UICellSampleData { index=33, name="Three" },
                new UICellSampleData { index=34, name="Four" },
                new UICellSampleData { index=35, name="Five" },
                new UICellSampleData { index=36, name="Six" },
                new UICellSampleData { index=37, name="Seven" },
                new UICellSampleData { index=38, name="Eight" },
                new UICellSampleData { index=39, name="Nine" },
                new UICellSampleData { index=40, name="Ten" }
            };

            // 스크롤시킬 내용의 크기를 갱신한다
            InitializeScrollView();
        }
    #endregion
    }
}
