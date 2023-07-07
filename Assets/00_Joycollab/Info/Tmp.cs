/// <summary>
/// 임시로 저장하는 저장 공간. 지속적으로 가지고 있지 않아도 되는 현재 게시글 목록 등을 담아둔다. 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 07
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 07) : 최초 생성
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

            listOkr = new List<OkrData>();
            listOkr.Clear();

            // for board, notice
            listBoard = new List<BoardData>();
        }

        public void Clear() 
        {
            //for to-do, OKR
            ClearToDoList();
            ClearOkrList();

            // for board, notice
            ClearBoardList();
        }

    #endregion  // Common functions


    #region To-Do, OKR Info

        private List<ToDoData> listToDo;

        public void AddToDoInfo(int seq, ToDoData todo)
        {
            int index = listToDo.FindIndex(item => item.info.seq == seq);
            if (index == -1) 
                listToDo.Add(todo);
            else             
                listToDo[index].info = todo.info;
        }
        public ToDoData GetToDoInfo(int seq) 
        {
            int index = listToDo.FindIndex(item => item.info.seq == seq);
            if (index == -1) return null;
            
            return listToDo[index];
        }
        public void ClearToDoList() => listToDo.Clear(); 


        private List<OkrData> listOkr;

        public int AddOkrInfo(int seq, OkrData data)
        { 
            int index = listOkr.FindIndex(item => item.seq == seq);
            if (index == -1)
                listOkr.Add(data);
            else
                listOkr[index].info = data.info;

            return index;
	    }
        public OkrData GetOkrInfo(int seq)
        { 
            int index = listOkr.FindIndex(item => item.seq == seq);
            if (index == -1) return null;

			return listOkr[index];
	    }
        public void ClearOkrList() => listOkr.Clear();
        
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