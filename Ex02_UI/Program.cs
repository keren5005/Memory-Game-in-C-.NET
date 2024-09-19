namespace GameUI
{
    //Relevant to the entire solution:
    // $G$ NTT-999 (-10) Should use Environment.NewLine rather than \n.
    // $G$ NTT-999 (-10) You should have used verbatim String
    // $G$ SFN-012 (-7) The program does not cope properly with invalid input
    // $G$ SFN-014 (+12) AI Implementation
    // $G$ RUL-006 (-40) Late submission
    public class Program
    {
        public static void Main()
        {
            // $G$ CSS-999 (-3) Missing blank line 
            GameUI myGame = new GameUI();
            myGame.GetPlayerDataAndStartGame();
        }
    }
}

