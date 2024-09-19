using System.Collections.Generic;

namespace GameLogic
{
    public class GameManager
    {
        private Board m_Board;
        private List<Player> m_ListOfPlayers;
        private uint m_CurrentPlayerIndex;
        

        public Board GameBoard
        {
            get { return m_Board; }
            set { m_Board = value; }
        }
        public List<Player> ListOfPlayers
        { 
            get { return m_ListOfPlayers;}
            set { m_ListOfPlayers = value; }
        }
        public uint CurrentPlayerIndex
        {
            get { return m_CurrentPlayerIndex; }
            set { m_CurrentPlayerIndex = value; }
        }

        public void AddNewPlayer(string i_PlayerName, bool i_IsPlayerHuman)
        {
            if (m_ListOfPlayers == null)
            {
                m_ListOfPlayers = new List<Player>();
            }
            m_ListOfPlayers.Add(new Player(i_PlayerName, i_IsPlayerHuman));
        }

        public Player GetCurrentPlayer()
        {
            return m_ListOfPlayers[(int)m_CurrentPlayerIndex];
        }

        public bool CheckBoardDimensionsValidity(uint i_BoardLength, uint i_BoardHeight)
        {
            return (i_BoardLength * i_BoardHeight) % 2 == 0;
        }
        
        public bool SetBoardDimensions(uint i_BoardLength, uint i_BoardHeight)
        {
            bool m_isCellsNumEven = CheckBoardDimensionsValidity(i_BoardLength, i_BoardHeight);

            if (m_isCellsNumEven)
            {
                GameBoard = new Board(i_BoardHeight, i_BoardLength);
            }

            return m_isCellsNumEven;
        }

        public void SetCurrentPlayer(uint i_DesiredPlayerIndex)
        {
            m_CurrentPlayerIndex = i_DesiredPlayerIndex;
        }

        public bool AttemptToMakeMove(ref uint io_Row, ref uint io_Col, out uint o_RevealedValue)
        {
            Player currentPlayer = GetCurrentPlayer();
            bool legalCellReveal, attemptToMakeLegalMoveSuccessful = false;

            if (!currentPlayer.IsHuman) //Bot move
            {
                currentPlayer.MakeComputerMove(out io_Row, out io_Col, GameBoard);
                legalCellReveal = m_Board.RevealCell(io_Row, io_Col, out o_RevealedValue);
                if (legalCellReveal)
                {
                    currentPlayer.MarkSuccessfulMove();
                    attemptToMakeLegalMoveSuccessful = true;

                    //Used it so we can at least see the computer's moves. Hope you don't mind!
                    System.Threading.Thread.Sleep(500);
                }
               
            }
            else //Human player
            {
                legalCellReveal = m_Board.RevealCell(io_Row, io_Col, out o_RevealedValue);
                if (legalCellReveal)
                {
                    attemptToMakeLegalMoveSuccessful = true;
                }
            }

            if (attemptToMakeLegalMoveSuccessful)
            {
                foreach (var player in m_ListOfPlayers)
                {
                    if (!player.IsHuman)
                    {
                        player.RememberCell(io_Row, io_Col, o_RevealedValue);
                    }
                }
            }

            return attemptToMakeLegalMoveSuccessful;
        }

        public bool CheckUserInputValidity(uint i_Row, uint i_Col)
        {
            return i_Row < GameBoard.Rows && i_Col < GameBoard.Columns;
        }

        public bool CheckMatch(uint i_Row1, uint i_Column1, uint i_Row2, uint i_Column2)
        {
            Board.BoardCell cell1 = m_Board.BoardMatrix[i_Row1, i_Column1];
            Board.BoardCell cell2 = m_Board.BoardMatrix[i_Row2, i_Column2];
            bool isMatch = cell1.Value == cell2.Value;

            if (isMatch)
            {
                cell1.FoundMatch = true;
                cell2.FoundMatch = true;
            }
            else
            {
                cell1.IsRevealed = false;
                cell2.IsRevealed = false;
            }

            return isMatch;
        }

        public void AddPointToPlayer()
        {
            m_ListOfPlayers[(int)m_CurrentPlayerIndex].Score++;
        }

        public void NextTurn()
        {
            m_CurrentPlayerIndex = (uint)((m_CurrentPlayerIndex + 1) % m_ListOfPlayers.Count);
        }

        // $G$ CSS-999 (-3) Missing blank line before return statement
        // $G$ CSS-027 (-3) Missing blank line, after local variables

        public bool IsGameCompleted()
        {
            bool isGameCompleted = true;
            for (uint row = 0; row < m_Board.Rows; row++)
            {
                for (uint col = 0; col < m_Board.Columns; col++)
                {
                    if (!m_Board.BoardMatrix[row, col].FoundMatch)
                    {
                        isGameCompleted = false;
                        break;
                    }
                }
                
                if (!isGameCompleted)
                {
                    break;
                }
            }
            return isGameCompleted;
        }

        public Board.BoardCell GetBoardCell(uint i_Row, uint i_Col)
        {
            return m_Board.BoardMatrix[i_Row, i_Col];
        }

        public void ResetGame()
        {
            m_Board = null;
            m_CurrentPlayerIndex = 0;

            foreach (var player in m_ListOfPlayers)
            {
                player.ResetPlayerData();
            }
        }
    }
}
