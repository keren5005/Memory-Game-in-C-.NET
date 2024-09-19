using System;
using System.Collections.Generic;

namespace GameLogic
{
    public class Player
    {
        public class Memory
        {
            public class Info
            {
                private uint m_Row, m_Col;
                private uint m_Value;

                public uint Row
                {
                    get { return m_Row; }
                    set { m_Row = value; }
                }
                public uint Col
                {
                    get { return m_Col; }
                    set { m_Col = value; }
                }
                public uint Value
                {
                    get { return m_Value; }
                    set { m_Value = value; }
                }

                public Info(uint i_Row, uint i_Col, uint i_Value)
                {
                    m_Row = i_Row;
                    m_Col = i_Col;
                    m_Value = i_Value;
                }
            }

            public const uint k_MaxSize = 5;
            private List<Info> m_ListOfInfo;
            private bool m_HoldsSecondCellForMatch;
            private uint m_RememberedRow, m_RememberedCol;

            public List<Info> ListOfInfo
            {
                get { return m_ListOfInfo; }
                set { m_ListOfInfo = value; }
            }
            public bool HoldsSecondCellForMatch
            {
                get { return m_HoldsSecondCellForMatch; }
                set { m_HoldsSecondCellForMatch = value; }
            }
            public uint RememberedRow
            {
                get { return m_RememberedRow; }
                set { m_RememberedRow = value; }
            }
            public uint RememberedCol
            {
                get { return m_RememberedCol; }
                set { m_RememberedCol = value; }
            }

            public Memory()
            {
                m_ListOfInfo = new List<Info>();
                m_HoldsSecondCellForMatch = false;
            }
        }

        private readonly string r_PlayerName;
        private int m_Score;
        private bool m_isFirstMove;
        private readonly bool r_IsHuman;
        private Memory m_AIMemory;

        public string Name
        {
            get { return r_PlayerName; }
        }
        public bool IsHuman
        {
            get { return r_IsHuman; }
        }
        public int Score
        {
            get { return m_Score; }
            set { m_Score = value; }
        }

        public Player(string i_Name, bool i_IsHuman)
        {
            r_PlayerName = i_Name;
            r_IsHuman = i_IsHuman;
            m_Score = 0;

            if (!r_IsHuman)
            {
                m_AIMemory = new Memory();
                m_AIMemory.HoldsSecondCellForMatch = false;
                m_isFirstMove = true;
            }
        }

        public void RememberCell(uint i_Row, uint i_Col, uint i_Value)
        {
            //To make it feel more "human", computer player might forget random info.
            checkMemoryLoss();
            //If info already exists in memory, remove older copy before inserting info.
            foreach (var info in m_AIMemory.ListOfInfo)
            {
                if (info.Row == i_Row && info.Col == i_Col)
                {
                    forgetCell(m_AIMemory.ListOfInfo.IndexOf(info));
                    break;
                }
            }

            m_AIMemory.ListOfInfo.Insert(0, new Memory.Info(i_Row, i_Col, i_Value));
            //forget info that has gotten too old
            if (m_AIMemory.ListOfInfo.Count > Memory.k_MaxSize)
            {
                forgetCell((int)Memory.k_MaxSize - 1);
            }
        }

        private void forgetCell(int i_IndexInList)
        {
            m_AIMemory.ListOfInfo.RemoveAt(i_IndexInList);
        }


        // $G$ NTT-007 (-10) There's no need to re-instantiate the Random instance each time it is used.
        private void checkMemoryLoss()
        {
            const int ceilingForForgetting = 10; //The higher, the harder to forget.
            int numberOfElementsInMemory = m_AIMemory.ListOfInfo.Count;
            Random randomizer = new Random();

            if (randomizer.Next() % ceilingForForgetting == 0 && numberOfElementsInMemory != 0)
            {
                int indexOfForgottenInfo = randomizer.Next() % numberOfElementsInMemory;
                forgetCell(indexOfForgottenInfo);
            }
        }

        // $G$ DSN-999 (-3) This method should be private (None of the other classes used this method...)
        public bool TryToRememberMatch(out uint o_Row, out uint o_Col)
        {
            const bool v_FoundMatch = true;
            bool isMatchFound = !v_FoundMatch;

            o_Row = o_Col = 0;

            foreach (var newerInfo in m_AIMemory.ListOfInfo)
            {
                foreach (var olderInfo in m_AIMemory.ListOfInfo)
                {
                    if (newerInfo == olderInfo)
                        continue;

                    if (newerInfo.Value == olderInfo.Value)
                    {
                        o_Row = olderInfo.Row;
                        o_Col = olderInfo.Col;
                        m_AIMemory.HoldsSecondCellForMatch = true;
                        m_AIMemory.RememberedRow = newerInfo.Row;
                        m_AIMemory.RememberedCol = newerInfo.Col;
                        forgetCell(m_AIMemory.ListOfInfo.IndexOf(newerInfo));
                        forgetCell(m_AIMemory.ListOfInfo.IndexOf(olderInfo));
                        isMatchFound = v_FoundMatch;
                        break;
                    }
                }

                if (isMatchFound)
                {
                    break;
                }
            }

            return isMatchFound;
        }

        public void MakeComputerMove(out uint o_Row, out uint o_Col, Board i_Board)
        {
            bool isMatchFound;

            if (m_isFirstMove)
            {
                isMatchFound = TryToRememberMatch(out o_Row, out o_Col);
            }
            else
            {
                if (m_AIMemory.HoldsSecondCellForMatch)
                {
                    m_AIMemory.HoldsSecondCellForMatch = false;
                    o_Row = m_AIMemory.RememberedRow;
                    o_Col = m_AIMemory.RememberedCol;
                    isMatchFound = true;
                }
                else
                {
                    i_Board.GetRevealedCell(out Board.BoardCell revealedCell);
                    isMatchFound = findMatchForCell(revealedCell.Value, revealedCell.Row, revealedCell.Col, out o_Row, out o_Col);
                }
            }

            if (!isMatchFound)
            {
                Random randomValue = new Random();
                o_Row = (uint)randomValue.Next((int)i_Board.Rows);
                o_Col = (uint)randomValue.Next((int)i_Board.Columns);
            }
        }

        public void MarkSuccessfulMove()
        {
            m_isFirstMove = !m_isFirstMove;
        }

        private bool findMatchForCell(uint i_Value, uint i_RowOfRevealedCell, uint i_ColOfRevealedCell, out uint o_Row, out uint o_Col)
        {
            bool isMatchFound = false;

            o_Col = o_Row = 0;
            foreach (var info in m_AIMemory.ListOfInfo)
            {
                if (info.Value == i_Value &&
                    !(i_RowOfRevealedCell == info.Row && i_ColOfRevealedCell == info.Col))
                {
                    isMatchFound = true;
                    o_Col = info.Col;
                    o_Row = info.Row;
                }
            }

            return isMatchFound;
        }

        public void ResetPlayerData()
        {
            m_Score = 0;
            m_isFirstMove = true;
            if (!r_IsHuman)
            {
                m_AIMemory.ListOfInfo.Clear();
            }
        }
    }
}

