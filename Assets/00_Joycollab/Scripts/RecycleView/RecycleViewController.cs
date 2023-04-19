using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
{
    [RequireComponent(typeof(ScrollRect))]
    [RequireComponent(typeof(RectTransform))]
    public class RecycleViewController<T> : MonoBehaviour
    {
        // for list
        protected List<T> tableData; // 리스트 항목의 데이터를 저장
        private LinkedList<RecycleViewCell<T>> cells; // 셀 저장 리스트

        // for search
        protected List<T> originData;

        [SerializeField] protected Transform cellParent = null;
        [SerializeField] protected GameObject cellBase = null;   // 복사 원본 셀
        [SerializeField] private RectOffset padding; // 스크롤할 내용의 패딩
        [SerializeField] private float spacingHeight = 4.0f; // 각 셀의 간격
        [SerializeField] private RectOffset visibleRectPadding = null; // visibleRect의 패딩

        private Rect visibleRect; // 리스트 항목을 셀의 형태로 표시하는 범위를 나타내는 사각형
        private Vector2 prevScrollPos; // 바로 전의 스크롤 위치를 저장


    #region Unity functions
        private void Awake() 
        {
            tableData = new List<T>();
            cells = new LinkedList<RecycleViewCell<T>>();

            originData = new List<T>();
        }

        protected virtual void Start() 
        {
            cellBase.SetActive(false);

            CachedScrollRect.onValueChanged.AddListener(OnScrollPosChanged);
        }
    #endregion


    #region Public functions
        public ScrollRect CachedScrollRect => GetComponent<ScrollRect>();
        public RectTransform CachedRectTransform => GetComponent<RectTransform>();

        /// <summary>
        /// 스크롤뷰가 움직였을 때 호출되는 함수
        /// </summary>
        /// <param name="scrollPos">Scroll position.</param>
        public void OnScrollPosChanged(Vector2 scrollPos)
        {
            // visibleRect를 갱신한다
            UpdateVisibleRect();

            // 셀을 재사용한다
            UpdateCells((scrollPos.y < prevScrollPos.y) ? 1 : -1);

            prevScrollPos = scrollPos;
        }
    #endregion


    #region Protected functions
        /// <summary>
        /// scroll view 초기화
        /// </summary>
        protected void InitializeScrollView() 
        {
            UpdateScrollViewSize(); // 스크롤할 내용의 크기를 갱신한다
            UpdateVisibleRect();    // visibleRect를 갱신한다

            if (cells.Count < 1)
            {
                // 셀이 하나도 없을 때는 visibleRect의 범위에 들어가는 첫 번째 리스트 항목을 찾아서
                // 그에 대응하는 셀을 작성한다
                Vector2 cellTop = new Vector2(0.0f, -padding.top);
                for (int i = 0; i < tableData.Count; i++)
                {
                    float cellHeight = GetCellHeightAtIndex(i);
                    Vector2 cellBottom = cellTop + new Vector2(0.0f, -cellHeight);
                    if ((cellTop.y <= visibleRect.y && cellTop.y >= visibleRect.y - visibleRect.height) ||
                       (cellBottom.y <= visibleRect.y && cellBottom.y >= visibleRect.y - visibleRect.height))
                    {
                        RecycleViewCell<T> cell = CreateCellForIndex(i);
                        cell.Top = cellTop;
                        break;
                    }
                    cellTop = cellBottom + new Vector2(0.0f, spacingHeight);
                }

                // visibleRect의 범위에 빈 곳이 있으면 셀을 작성한다
                SetFillVisibleRectWithCells();
            }
            else
            {
                // 이미 셀이 있을 때는 첫 번째 셀부터 순서대로 대응하는 리스트 항목의
                // 인덱스를 다시 설정하고 위치와 내용을 갱신한다
                LinkedListNode<RecycleViewCell<T>> node = cells.First;
                UpdateCellForIndex(node.Value, node.Value.Index);
                node = node.Next;

                while (node != null)
                {
                    UpdateCellForIndex(node.Value, node.Previous.Value.Index + 1);
                    node.Value.Top = node.Previous.Value.Bottom + new Vector2(0.0f, -spacingHeight);
                    node = node.Next;
                }

                // visibleRect의 범위에 빈 곳이 있으면 셀을 작성한다
                SetFillVisibleRectWithCells();
            }    
        }

        /// <summary>
        /// 셀의 높이값을 리턴하는 함수
        /// </summary>
        /// <returns>The cell height at index.</returns>
        /// <param name="index">Index.</param>
        protected virtual float GetCellHeightAtIndex(int index)
        {
            // 실제 값을 반환하는 처리는 상속한 클래스에서 구현한다
            // 셀마다 크기가 다를 경우 상속받은 클래스에서 재 구현한다
            return cellBase.GetComponent<RectTransform>().sizeDelta.y;
        }

        /// <summary>
        /// 스크롤할 내용 전체의 높이를 갱신하는 함수
        /// </summary>
        protected void UpdateScrollViewSize()
        {
            // 스크롤할 내용 전체의 높이를 계산한다
            float contentHeight = 0.0f;
            for (int i = 0; i < tableData.Count; i++)
            {
                contentHeight += GetCellHeightAtIndex(i);

                if (i > 0)
                {
                    contentHeight += spacingHeight;
                }
            }

            // 스크롤할 내용의 높이를 설정한다
            Vector2 sizeDelta = CachedScrollRect.content.sizeDelta;
            sizeDelta.y = padding.top + contentHeight + padding.bottom;
            CachedScrollRect.content.sizeDelta = sizeDelta;
        }
    #endregion


    #region Private functions
        /// <summary>
        /// 셀을 생성하는 함수
        /// </summary>
        /// <returns>The cell for index.</returns>
        /// <param name="index">Index.</param>
        private RecycleViewCell<T> CreateCellForIndex(int index)
        {
            // 복사 원본 셀을 이용해 새로운 셀을 생성한다
            GameObject obj = Instantiate(cellBase) as GameObject;
            obj.SetActive(true);
            RecycleViewCell<T> cell = obj.GetComponent<RecycleViewCell<T>>();

            // 부모 요소를 바꾸면 스케일이나 크기를 잃어버리므로 변수에 저장해둔다
            Vector3 scale = cell.transform.localScale;
            Vector2 sizeDelta = cell.CachedRectTransform.sizeDelta;
            Vector2 offsetMin = cell.CachedRectTransform.offsetMin;
            Vector2 offsetMax = cell.CachedRectTransform.offsetMax;

            // cell.transform.SetParent(cellBase.transform.parent);
            cell.transform.SetParent(cellParent, false);

            // 셀의 스케일과 크기를 설정한다
            cell.transform.localScale = scale;
            cell.CachedRectTransform.sizeDelta = sizeDelta;
            cell.CachedRectTransform.offsetMin = offsetMin;
            cell.CachedRectTransform.offsetMax = offsetMax;

            // 지정된 인덱스가 붙은 리스트 항목에 대응하는 셀로 내용을 갱신한다
            UpdateCellForIndex(cell, index);

            cells.AddLast(cell);

            return cell;
        }

        /// <summary>
        /// 셀의 내용을 갱신하는 함수
        /// </summary>
        /// <param name="cell">Cell.</param>
        /// <param name="index">Index.</param>
        private void UpdateCellForIndex(RecycleViewCell<T> cell, int index)
        {
            // 셀에 대응하는 리스트 항목의 인덱스를 설정한다
            cell.Index = index;

            if (cell.Index >= 0 && cell.Index <= tableData.Count - 1)
            {
                // 셀에 대응하는 리스트 항목이 있다면 셀을 활성화해서 내용을 갱신하고 높이를 설정한다
                cell.gameObject.SetActive(true);
                cell.UpdateContent(tableData[cell.Index]);
                cell.Height = GetCellHeightAtIndex(cell.Index);
            }
            else
            {
                // 셀에 대응하는 리스트 항목이 없다면 셀을 비활성화시켜 표시되지 않게 한다
                cell.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// VisibleRect를 갱신하기 위한 함수
        /// </summary>
        private void UpdateVisibleRect()
        {
            // visibleRect의 위치는 스크롤할 내용의 기준으로부터 상대적인 위치다
            visibleRect.x = CachedScrollRect.content.anchoredPosition.x + visibleRectPadding.left;
            visibleRect.y = -CachedScrollRect.content.anchoredPosition.y + visibleRectPadding.top;

            // visibleRect의 크기는 스크롤 뷰의 크기 + 패딩
            visibleRect.width = CachedRectTransform.rect.width + visibleRectPadding.left + visibleRectPadding.right;
            visibleRect.height = CachedRectTransform.rect.height + visibleRectPadding.top + visibleRectPadding.bottom;
        }

        /// <summary>
        /// VisubleRect 범위에 표시될 만큼의 셀을 생성하여 배치하는 함수
        /// </summary>
        private void SetFillVisibleRectWithCells()
        {
            // 셀이 없다면 아무 일도 하지 않는다
            if (cells.Count < 1)
            {
                return;
            }

            // 표시된 마지막 셀에 대응하는 리스트 항목의 다음 리스트 항목이 있고
            // 또한 그 셀이 visibleRect의 범위에 들어온다면 대응하는 셀을 작성한다
            RecycleViewCell<T> lastCell = cells.Last.Value;
            int nextCellDataIndex = lastCell.Index + 1;
            Vector2 nextCellTop = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);

            while (nextCellDataIndex < tableData.Count && nextCellTop.y >= visibleRect.y - visibleRect.height)
            {
                RecycleViewCell<T> cell = CreateCellForIndex(nextCellDataIndex);
                cell.Top = nextCellTop;

                lastCell = cell;
                nextCellDataIndex = lastCell.Index + 1;
                nextCellTop = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);
            }
        }

        /// <summary>
        /// 셀을 재사용하여 표시를 갱신하는 함수
        /// </summary>
        /// <param name="scrollDirection">Scroll direction.</param>
        private void UpdateCells(int scrollDirection)
        {
            if (cells.Count < 1)
            {
                return;
            }

            if (scrollDirection > 0)
            {
                // 위로 스크롤하고 있을 때는 visibleRect에 지정된 범위보다 위에 있는 셀을
                // 아래를 향해 순서대로 이동시켜 내용을 갱신한다
                RecycleViewCell<T> firstCell = cells.First.Value;
                while (firstCell.Bottom.y > visibleRect.y)
                {
                    RecycleViewCell<T> lastCell = cells.Last.Value;
                    UpdateCellForIndex(firstCell, lastCell.Index + 1);
                    firstCell.Top = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);

                    cells.AddLast(firstCell);
                    cells.RemoveFirst();
                    firstCell = cells.First.Value;
                }

                // visibleRect에 지정된 범위 안에 빈 곳이 있으면 셀을 작성한다
                SetFillVisibleRectWithCells();
            }
            else if (scrollDirection < 0)
            {
                // 아래로 스크롤하고 있을 때는 visibleRect에 지정된 범위보다 아래에 있는 셀을
                // 위를 향해 순서대로 이동시켜 내용을 갱신한다
                RecycleViewCell<T> lastCell = cells.Last.Value;
                while (lastCell.Top.y < visibleRect.y - visibleRect.height)
                {
                    RecycleViewCell<T> firstCell = cells.First.Value;
                    UpdateCellForIndex(lastCell, firstCell.Index - 1);
                    lastCell.Bottom = firstCell.Top + new Vector2(0.0f, spacingHeight);

                    cells.AddFirst(lastCell);
                    cells.RemoveLast();
                    lastCell = cells.Last.Value;
                }
            }
        } 
    #endregion
    }
}