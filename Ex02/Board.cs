using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{
    public class Board
    {
        public class BoardCell
        {
            private readonly uint r_CellValue;
            private readonly uint r_Row, r_Col;
            private bool m_IsRevealed;
            private bool m_FoundMatch;

            public uint Value
            {
                get { return r_CellValue; }
            }
            public uint Row
            {
                get { return r_Row; }
            }
            public uint Col
            {
                get { return r_Col; }
            }
            public bool IsRevealed
            {
                get { return m_IsRevealed; }
                set { m_IsRevealed = value; }
            }
            public bool FoundMatch
            {
                get { return m_FoundMatch; }
                set { m_FoundMatch = value; }
            }

            public BoardCell(uint i_CellValue, uint i_Row, uint i_Col)
            {
                r_CellValue = i_CellValue;
                r_Row = i_Row;
                r_Col = i_Col;
                m_IsRevealed = false;
                m_FoundMatch = false;
            }
        }

        private readonly BoardCell[,] r_Board;
        private readonly uint r_BoardRows;
        private readonly uint r_BoardColumns;

        public uint Rows
        {
            get { return r_BoardRows; }
        }
        public uint Columns
        {
            get { return r_BoardColumns; }
        }
        public BoardCell[,] BoardMatrix
        {
            get { return r_Board; }
        }

        public Board(uint i_BoardHeight, uint i_BoardWidth)
        {
            r_BoardRows = i_BoardHeight;
            r_BoardColumns = i_BoardWidth;
            r_Board = new BoardCell[r_BoardRows, r_BoardColumns];

            initializeBoard();
        }

        private void initializeBoard()
        {
            List<uint> boardValuesUnshuffled = generateUnshuffledValues(Rows, Columns);
            Random randomForShufflingBoardValues = new Random();

            for (uint row = 0; row < r_BoardRows; ++row)
            {
                for (uint column = 0; column < r_BoardColumns; ++column)
                {
                    int randomIndexInList = randomForShufflingBoardValues.Next(boardValuesUnshuffled.Count);
                    r_Board[row, column] = new BoardCell(boardValuesUnshuffled[randomIndexInList], row, column);
                    boardValuesUnshuffled.RemoveAt(randomIndexInList);
                }
            }
        }

        private List<uint> generateUnshuffledValues(uint i_BoardRows, uint i_BoardColumns)
        {
            List<uint> boardValuesUnshuffled = new List<uint>();
            uint numOfPairs = (i_BoardRows * i_BoardColumns) / 2;

            for (uint currentCellID = 1; currentCellID <= numOfPairs; ++currentCellID)
            {
                boardValuesUnshuffled.Add(currentCellID);
                boardValuesUnshuffled.Add(currentCellID);
            }

            return boardValuesUnshuffled;
        }

        // $G$ CSS-999 (-3) Missing blank line before return statement
        public bool RevealCell(uint i_Row, uint i_Column, out uint o_RevealedValue)
        {
            bool shouldReveal = true;
            BoardCell cell = r_Board[i_Row, i_Column];
            if (cell.IsRevealed || cell.FoundMatch)
            {
                o_RevealedValue = 0;
                shouldReveal = false;
            }

            cell.IsRevealed = true;
            o_RevealedValue = cell.Value;
            return shouldReveal;
        }

        public bool GetRevealedCell(out BoardCell o_RevealedCell)
        {
            bool foundRevealedCell = false;
            o_RevealedCell = null;

            foreach (var cell in r_Board.Cast<BoardCell>())
            {
                if (cell.IsRevealed && !(cell.FoundMatch))
                {
                    o_RevealedCell = cell;
                    foundRevealedCell = true;
                    break;
                }
            }

            return foundRevealedCell;
        }

    }
}
