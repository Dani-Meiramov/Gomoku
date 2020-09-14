using System;

namespace Gomoku
{
    class Program
    {
        static void Main(string[] args)
        {
            var board = new Board();
            var computer1 = new Computer();
            var computer2 = new Computer();
            var computerOneMove = true;
            Cell nextMove;

            while (true)
            {
                board.Print();
                if (computerOneMove)
                {
                    nextMove = computer1.NextMove(board.GetBoard());
                    board.SetValue(nextMove.X, nextMove.Y, computer1.MySymbol);
                }
                else
                {
                    nextMove = computer2.NextMove(board.GetBoard());
                    board.SetValue(nextMove.X, nextMove.Y, computer2.MySymbol);
                }

                if (board.CheckWin(nextMove.X, nextMove.Y))
                {
                    board.Print();
                    if (computerOneMove)
                    {
                        Console.WriteLine("Computer ONE wins!");
                    }
                    else
                    {
                        Console.WriteLine("Computer TWO wins!");
                    }
                    break;
                }
                else if (board.CheckDraw())
                {
                    board.Print();
                    Console.WriteLine("Draw game!");
                    break;
                }
                else
                {
                    computerOneMove = !computerOneMove;
                }
            }
        }
    }
}