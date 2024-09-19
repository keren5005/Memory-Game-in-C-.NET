using System;
using System.Collections.Generic;
using GameLogic;


namespace GameUI
{
    // $G$ DSN-999 (-5) You should use constructor...
    public class GameUI
    {
        private const char k_InputForExitingProgram = 'Q';
        private const uint k_MinBoardDimension = 4;
        private const uint k_MaxBoardDimension = 6;
        private GameManager m_GameManager;



        // $G$ NTT-999 (-0) Should use Environment.NewLine rather than \n.
        // $G$ NTT-999 (-0) You should have used verbatim String
        // $G$ DSN-003 (-5) The code should be divided to methods. 
        public void GetPlayerDataAndStartGame()
        {
            m_GameManager = new GameManager();
            bool playAgain;
            string userInput;
            const bool v_IsHuman = true;

            Console.WriteLine("~ ~ ~ ~ ~ MEMORY GAME ~ ~ ~ ~ ~");
            Console.WriteLine("Welcome, Player! Please enter your name!");
            string playerName = Console.ReadLine();
            m_GameManager.AddNewPlayer(playerName, v_IsHuman); 

            Console.WriteLine("Choose an option:\n" +
                "\t1) Play against a friend\n" +
                "\t2) Play against the PC");
            do
            {
                userInput = Console.ReadLine();
                if (userInput != "1" && userInput != "2")
                {
                    Console.WriteLine("Please choose a valid option.");
                }
            } while (userInput != "1" && userInput != "2");

            if (userInput == "1")
            {
                Console.WriteLine("Please enter the name for Player 2!");
                playerName = Console.ReadLine();
                m_GameManager.AddNewPlayer(playerName, v_IsHuman);
            }
            else
            {
                m_GameManager.AddNewPlayer("Computer", !v_IsHuman);
            }

            do
            {
                m_GameManager.ResetGame();
                bool sentExitSignal = StartNewGame();
                playAgain = false;
                if (!sentExitSignal)
                {
                    Console.WriteLine("Would you like to play again?\n" +
                        "Y - Yes, play again!\n" +
                        "N - No, exit game.");
                    do
                    {
                        userInput = Console.ReadLine();
                        if (userInput != "Y" && userInput != "N")
                        {
                            Console.WriteLine("Please select a valid option (Y/N).");
                        }
                    } while (userInput != "Y" && userInput != "N");

                    if (userInput == "Y")
                    {
                        playAgain = true;
                    }
                }
            } while (playAgain);
            Console.WriteLine("Thanks for playing, see you soon!");
        }


        // $G$ NTT-999 (-0) Should use Environment.NewLine rather than \n.
        // $G$ NTT-999 (-0) You should have used verbatim String
        // $G$ CSS-999 (-3) Private methods should start with an lowercase letter.
        private void GetBoardDimensions(out uint o_BoardLength, out uint o_BoardHeight)
        {
            string userInputLength, userInputHeight;
            bool isValidUint, areDimensionsValid;

            Console.WriteLine("Enter the desired board dimensions.");
            Console.WriteLine("The dimensions must be a number from " +
                $"{k_MinBoardDimension} to {k_MaxBoardDimension}, " +
                "and they must form an even number of cells."); 
            do
            {
                Console.Write("Board length: ");
                userInputLength = Console.ReadLine();
                Console.Write("Board height: ");
                userInputHeight = Console.ReadLine();
                isValidUint = uint.TryParse(userInputLength, out o_BoardLength);
                isValidUint = uint.TryParse(userInputHeight, out o_BoardHeight) && isValidUint;
                if(isValidUint)
                {
                    areDimensionsValid = areBoardDimensionsValid(o_BoardLength, o_BoardHeight);
                    if (!areDimensionsValid)
                    {
                        Console.WriteLine("Invalid board dimensions! Try again.");
                    }
                    else
                    {
                        m_GameManager.SetBoardDimensions(o_BoardLength, o_BoardHeight);
                    }
                }
                else
                {
                    Console.WriteLine("You must input a number! Try again.");
                    areDimensionsValid = false;
                }
            } while (!areDimensionsValid);
        }

        // $G$ CSS-999 (-3) Missing blank line before return statement
        // $G$ DSN-999 (-3) This method should be private (none of the other classes uses it).
        public bool StartNewGame()
        {
            bool sentExitSignal = false;
            bool gameCompleted = false;

            Ex02.ConsoleUtils.Screen.Clear();
            Console.WriteLine("~ ~ ~ STARTING NEW GAME ~ ~ ~");
            GetBoardDimensions(out uint boardLength, out uint boardHeight);
            m_GameManager.SetCurrentPlayer(0);
             
            //MAIN GAME LOOP
            do
            {
                PrintBoard();
                //A turn consists of 2 moves
                if (makeMove(out uint row1, out uint col1) || makeMove(out uint row2, out uint col2))
                {
                    sentExitSignal = true;
                    break;
                }
                else
                {
                    if (m_GameManager.CheckMatch(row1, col1, row2, col2))
                    {
                        m_GameManager.AddPointToPlayer();
                    }
                    else
                    {
                        Console.Write("\nNot a match");
                        System.Threading.Thread.Sleep(500);
                        for (uint i = 0; i < 3; ++i)
                        {
                            Console.Write(".");
                            System.Threading.Thread.Sleep(500);
                        }
                        Console.WriteLine();
                        m_GameManager.NextTurn();
                    }

                    gameCompleted = m_GameManager.IsGameCompleted();
                }
            } while (!gameCompleted);
            
            if (gameCompleted)
            {
                printFinalScore();
            }
            return sentExitSignal;
        }


        // $G$ CSS-999 (-3) Missing blank line before return statement
        // $G$ NTT-999 (-0) Should use Environment.NewLine rather than \n.
        // $G$ DSN-003 (-5) The code should be divided to methods. 
        private bool makeMove(out uint o_Row, out uint o_Col)
        {
            bool sentExitSignal = false;
            bool legalCellReveal;
            Player currentPlayer = m_GameManager.GetCurrentPlayer();

            Console.WriteLine($"\n--- {currentPlayer.Name}'s turn! ---");
            if (currentPlayer.IsHuman) //Human player
            {
                do
                {
                    sentExitSignal = !getUserInput(out o_Row, out o_Col);
                    if (sentExitSignal)
                    {
                        break;
                    }

                    legalCellReveal = m_GameManager.AttemptToMakeMove(ref o_Row, ref o_Col, out uint revealedValue);
                    if (!legalCellReveal)
                    {
                        Console.WriteLine("Cell is already revealed! Plase choose a different cell.");
                    }
                } while (!legalCellReveal);
            }
            else //Bot player
            {
                do
                {
                    o_Row = o_Col = 0;
                    legalCellReveal = m_GameManager.AttemptToMakeMove(ref o_Row, ref o_Col, out uint revealedValue);
                } while (!legalCellReveal);
            }

            if(!sentExitSignal)
            {
                PrintBoard();
            }
            else
            {
                Ex02.ConsoleUtils.Screen.Clear();
            }
            return sentExitSignal;
        }

        private bool getUserInput(out uint o_Row, out uint o_Col)
        {
            string userInput;
            bool isValidRow, isValidCol, isValidInput;
            bool movePlayed = true;

            o_Row = o_Col = 0;

            do
            {
                char rowAsChar, colAsChar;

                Console.Write("Enter the cell you'd like to choose: ");
                userInput = Console.ReadLine();
                if (userInput[0] == k_InputForExitingProgram && userInput.Length == 1)
                {
                    movePlayed = false;
                    break;
                }
                else
                {
                    if (userInput.Length > 1)
                    {
                        colAsChar = userInput[0];
                        rowAsChar = userInput[1];
                        isValidRow = char.IsDigit(rowAsChar);
                        isValidCol = char.IsUpper(colAsChar);
                        if (isValidRow && isValidCol)
                        {
                            o_Row = (uint)(rowAsChar - '1');
                            o_Col = (uint)(colAsChar - 'A');
                            isValidInput = m_GameManager.CheckUserInputValidity(o_Row, o_Col);
                            if (!isValidInput)
                            {
                                Console.WriteLine("Error- row or column selected is out of table bounds. Try again.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong input format! " +
                                "Format must be a Column (capital letter), then row (digit). Try again.");
                            isValidInput = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Input is too short! " +
                                "Format must be a column (capital letter), then row (digit). Try again.");
                        isValidInput = false;
                    }
                }
            } while (!isValidInput);

            return movePlayed;
        }

        private bool areBoardDimensionsValid(uint i_Length, uint i_Height)
        {
            return m_GameManager.CheckBoardDimensionsValidity(i_Length, i_Height)
                && i_Length <= k_MaxBoardDimension
                && i_Length >= k_MinBoardDimension
                && i_Height <= k_MaxBoardDimension
                && i_Height >= k_MinBoardDimension;
        }

        private void printFinalScore()
        {
            List<Player> playersList = m_GameManager.ListOfPlayers;
            
            Console.WriteLine("----- FINAL SCORES -----");
            foreach (Player player in playersList)
            {
                Console.WriteLine($"{player.Name}: {player.Score} points!");
            }
            Console.WriteLine("------------------------");
        }


        // $G$ DSN-999 (-3) This method should be private (none of the other classes uses it).
        public void PrintBoard()
        {
            Ex02.ConsoleUtils.Screen.Clear();

            //Print column letters
            Console.Write("   ");
            for (uint i = 0; i < m_GameManager.GameBoard.Columns; i++)
            {
                Console.Write($"  {(char)('A' + i)} ");
            }

            Console.WriteLine();

            //Print top border
            Console.Write("   ");
            for (uint i = 0; i < m_GameManager.GameBoard.Columns; i++)
            {
                Console.Write("====");
            }
            Console.WriteLine("=");

            //Print rows
            for (uint row = 0; row < m_GameManager.GameBoard.Rows; row++)
            {
                //Print row number
                Console.Write($" {row + 1} |");

                //Print each cell in the row
                for (uint col = 0; col < m_GameManager.GameBoard.Columns; col++)
                {
                    Board.BoardCell currentBoardCell = m_GameManager.GetBoardCell(row, col);
                    char cellValue = (currentBoardCell.IsRevealed || currentBoardCell.FoundMatch) ? 
                        (char)(currentBoardCell.Value + 'A' - 1) : ' ';
                    
                    Console.Write($" {cellValue} |");
                }
                Console.WriteLine();

                //Print row separator
                Console.Write("   ");
                for (uint col = 0; col < m_GameManager.GameBoard.Columns; col++)
                {
                    Console.Write("====");
                }
                Console.WriteLine("=");
            }
        }
    }
}
