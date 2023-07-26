/// <summary>
/// 임시로 저장하는 저장 공간. 지속적으로 가지고 있지 않아도 되는 현재 게시글 목록 등을 담아둔다. 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 26
/// @version        : 0.3
/// @update
///     v0.1 (2023. 07. 07) : 최초 생성
///     v0.2 (2023. 07. 25) : To-Do 검색 목록도 추가.
///     v0.3 (2023. 07. 26) : Okr 검색 목록도 추가.
/// </summary>

using System.Collections.Generic;

namespace Joycollab.v2 
{
    public class Tmp : Singleton<Tmp>
    {


    #region Common functions

        private void Awake() 
        {
            // for to-do, OKR
            listToDo = new List<ToDoData>();
            listToDo.Clear();
            listToDoSearch = new List<ToDoData>();
            listToDoSearch.Clear();

            listOkr = new List<OkrData>();
            listOkr.Clear();
            listOkrSearch = new List<OkrData>();
            listOkrSearch.Clear();

            // for board, notice
            listBoard = new List<BoardData>();
        }

        public void Clear() 
        {
            //for to-do, OKR
            ClearToDoList();
            ClearToDoSearchList();
            ClearOkrList();
            ClearOkrSearchList();

            // for board, notice
            ClearBoardList();
        }

    #endregion  // Common functions


    #region To-Do, OKR Info

        private List<ToDoData> listToDo;

        public void AddToDoInfo(int seq, ToDoData todo)
        {
            int index = listToDo.FindIndex(item => item.info.seq == seq);
            if (index == -1)    listToDo.Add(todo);
            else                listToDo[index].info = todo.info;
        }
        public ToDoData GetToDoInfo(int seq) 
        {
            int index = listToDo.FindIndex(item => item.info.seq == seq);
            if (index == -1) return null;
            
            return listToDo[index];
        }
        public void ClearToDoList() => listToDo.Clear(); 


        private List<ToDoData> listToDoSearch;

        public void AddSearchToDo(int seq, ToDoData todo) 
        {
            int index = listToDoSearch.FindIndex(item => item.info.seq == seq);
            if (index == -1)    listToDoSearch.Add(todo);
            else                listToDoSearch[index].info = todo.info;
        }
        public ToDoData GetSearchToDo(int seq) 
        {
            int index = listToDoSearch.FindIndex(item => item.info.seq == seq);
            if (index == -1) return null;

            return listToDoSearch[index];
        }
        public void ClearToDoSearchList() => listToDoSearch.Clear();


        private List<OkrData> listOkr;

        public void AddOkrInfo(int seq, OkrData data)
        { 
            int index = listOkr.FindIndex(item => item.seq == seq);
            if (index == -1)    listOkr.Add(data);
            else                listOkr[index].info = data.info;
	    }
        public OkrData GetOkrInfo(int seq)
        { 
            int index = listOkr.FindIndex(item => item.seq == seq);
            if (index == -1) return null;

			return listOkr[index];
	    }
        public void ClearOkrList() => listOkr.Clear();


        private List<OkrData> listOkrSearch;

        public void AddSearchOkr(int seq, OkrData data) 
        {
            int index = listOkrSearch.FindIndex(item => item.info.seq == seq);
            if (index == -1)    listOkrSearch.Add(data);
            else                listOkrSearch[index].info = data.info;
        }
        public OkrData GetSearchOkr(int seq) 
        {
            int index = listOkrSearch.FindIndex(item => item.info.seq == seq);
            if (index == -1) return null;

            return listOkrSearch[index];
        }
        public void ClearOkrSearchList() => listOkrSearch.Clear();
        
    #endregion  // To-Do, OKR Info

        
    #region Board, Notice Info

        private List<BoardData> listBoard;

        public void AddBoardInfo(int seq, BoardData data)
        {
            int index = listBoard.FindIndex(item => item.info.seq == seq);
            if (index == -1) 
                listBoard.Add(data);
            else             
                listBoard[index].info = data.info;
        }
        public BoardData GetBoardInfo(int seq) 
        {
            int index = listBoard.FindIndex(item => item.info.seq == seq);
            if (index == -1) return null;
            
            return listBoard[index];
        }
        public void ClearBoardList() => listBoard.Clear(); 

    #endregion  // Board, Notice Info
    }
}