using System;
using System.Collections.Generic;

namespace Gomoku
{
    class Computer
    {
        public char MySymbol { get; set; }
        public char OpponentSymbol { get; set; }
        public int Depth { get; set; } = 2;

        /// <summary>
        /// Находит наилучший ход для заданной позиции
        /// </summary>
        /// <param name="currentState">Текущее состояние игровой доски</param>
        /// <returns>Возвращает лучший ход для заданной позиции</returns>
        public Cell NextMove(char[,] currentState)
        {
            var board = new Board();
            for (var i = 0; i < Board.N; i++)
            {
                for (var j = 0; j < Board.N; j++)
                {
                    if (currentState[i, j] != '_')
                    {
                        board.SetValue(i, j, currentState[i, j]);
                    }
                }
            }
            MySymbol = board.OccupiedCells % 2 == 0 ? 'x' : 'o';
            OpponentSymbol = MySymbol == 'x' ? 'o' : 'x';

            // Первый ход в игре.
            if (board.OccupiedCells == 0)
            {
                return (new Cell(Board.N / 2, Board.N / 2));
            }

            var desiredCells = GetDesiredCells(board);
            var bestMove = new Cell(desiredCells[0].X, desiredCells[0].Y);
            var weightOfBestMove = Int32.MinValue;
            var alpha = Int32.MinValue;
            var beta = Int32.MaxValue;

            foreach (Cell cell in desiredCells)
            {
                var weightOfCurrentMove = MinimaxAlphaBeta(board, cell.X, cell.Y, Depth, true, alpha, beta);
                if (weightOfCurrentMove > weightOfBestMove)
                {
                    weightOfBestMove = weightOfCurrentMove;
                    alpha = weightOfBestMove;
                    bestMove.X = cell.X;
                    bestMove.Y = cell.Y;
                }
            }
            return bestMove;
        }

        /// <summary>
        /// находит перспективные клетки для следующего хода
        /// </summary>
        /// <param name="board">Текущее состояние игровой доски</param>
        /// <returns>Возвращает все перспективные клетки для заданной доски</returns>
        private List<Cell> GetDesiredCells(Board board)
        {
            var desiredCells = new List<Cell>();
            for (var i = 0; i < Board.N; i++)
            {
                for (var j = 0; j < Board.N; j++)
                {
                    if (board.GetValue(i, j) == '_')
                    {
                        if (board.СheckExistenceOfAdjacentSymbols(i, j))
                        {
                            desiredCells.Add(new Cell(i, j));
                        }
                    }
                }
            }
            return desiredCells;
        }

        /// <summary>
        /// Алгоритм minimax с альфа-бета отсечениями
        /// </summary>
        /// <param name="board">Текущее состояние игровой доски</param>
        /// <param name="i">Первая координата</param>
        /// <param name="j">Вторая координата</param>
        /// <param name="depth">Глубина рекурсии</param>
        /// <param name="isMax">Показывает кто ходил последним. Если равно
        /// true, значит я ходил последним, т.е. мой ход максимизируется</param>
        /// <param name="alpha">Нижняя граница состояний игры, найденная
        /// для игрока на текущий момент</param>
        /// <param name="beta">Верхняя граница состояний игры, найденная
        /// для игрока на текущий момент</param>
        /// <returns>Возвращает оценку хода</returns>
        private int MinimaxAlphaBeta(Board board, int i, int j, int depth, bool isMax, int alpha, int beta)
        {
            board.SetValue(i, j, isMax ? MySymbol : OpponentSymbol);
            if (board.CheckWin(i, j))
            {
                board.SetValue(i, j, '_');
                return isMax ? Int32.MaxValue : Int32.MinValue;
            }
            else if (board.CheckDraw())
            {
                board.SetValue(i, j, '_');
                return 0;
            }
            if (depth == 0)
            {
                var eval = EvaluationOfCurrentState(board, isMax);
                board.SetValue(i, j, '_');
                return eval;
            }

            var desiredCells = GetDesiredCells(board);

            // Ход противника.
            if (isMax)
            {
                var weightOfOpponentTreeNode = Int32.MaxValue;
                // Минимизация веса хода оппонента.
                foreach (Cell cell in desiredCells)
                {
                    var weightOfMyTreeNode = MinimaxAlphaBeta(board, cell.X, cell.Y, depth - 1, false, alpha, beta);
                    if (weightOfMyTreeNode < weightOfOpponentTreeNode)
                    {
                        weightOfOpponentTreeNode = weightOfMyTreeNode;
                    }
                    if (weightOfOpponentTreeNode < beta)
                    {
                        beta = weightOfOpponentTreeNode;
                    }
                    if (alpha >= beta)
                    {
                        break;
                    }
                }
                board.SetValue(i, j, '_');
                return weightOfOpponentTreeNode;
            }
            // Мой ход.
            else
            {
                var weightOfMyTreeNode = Int32.MinValue;
                // Максимизация веса моего хода.
                foreach (Cell cell in desiredCells)
                {
                    var weightOfOpponentTreeNode = MinimaxAlphaBeta(board, cell.X, cell.Y, depth - 1, true, alpha, beta);
                    if (weightOfOpponentTreeNode > weightOfMyTreeNode)
                    {
                        weightOfMyTreeNode = weightOfOpponentTreeNode;
                    }
                    if (weightOfMyTreeNode > alpha)
                    {
                        alpha = weightOfMyTreeNode;
                    }
                    if (alpha >= beta)
                    {
                        break;
                    }
                }
                board.SetValue(i, j, '_');
                return weightOfMyTreeNode;
            }
        }

        /// <summary>
        /// Оценивает текущее состояние доски
        /// </summary>
        /// <param name="board">Текущее состояние игровой доски</param>
        /// <param name="isMax">Показывает кто ходил последним. Если равно
        /// true, значит я ходил последним</param>
        /// <returns>Возвращает оценку текущего состояния доски</returns>
        private int EvaluationOfCurrentState(Board board, bool isMax)
        {
            int[] myAttacks = { 0, 0, 0, 0 };
            int[] opponentAttacks = { 0, 0, 0, 0 };
            var potentialOpenFour = false;

            for (var i = 0; i < Board.N; i++)
            {
                for (var j = 0; j < Board.N; j++)
                {
                    if (board.GetValue(i, j) == '_')
                    {
                        // В зависимости от того, кто ходил последним проверяем закончит ли игру оппонент(или я)
                        // в один ход, и есть ли у него(или у меня) потенциальная открытая четверка.
                        if (board.CheckExistenceOfAdjacentSymbol(i, j, isMax ? OpponentSymbol : MySymbol))
                        {
                            board.SetValue(i, j, isMax ? OpponentSymbol : MySymbol);
                            if (board.CheckWin(i, j))
                            {
                                board.SetValue(i, j, '_');
                                return isMax ? Int32.MinValue : Int32.MaxValue;
                            }
                            else if (board.CheckDraw())
                            {
                                board.SetValue(i, j, '_');
                                return 0;
                            }
                            else if (board.CheckOpenFour(i, j))
                            {
                                potentialOpenFour = true;
                            }
                            board.SetValue(i, j, '_');
                        }
                    }
                    else
                    {
                        var weightOfAttack = 0;

                        if (board.CheckRowAttack(i, j, ref weightOfAttack))
                        {
                            if (board.GetValue(i, j) == MySymbol)
                            {
                                myAttacks[weightOfAttack]++;
                            }
                            else
                            {
                                opponentAttacks[weightOfAttack]++;
                            }
                        }

                        if (board.CheckColumnAttack(i, j, ref weightOfAttack))
                        {
                            if (board.GetValue(i, j) == MySymbol)
                            {
                                myAttacks[weightOfAttack]++;
                            }
                            else
                            {
                                opponentAttacks[weightOfAttack]++;
                            }
                        }

                        // Не проверяем главные диагонали длиной меньше пяти.
                        if (((i <= 3) && (j >= 11) && (j - i >= 11))
                            || ((i >= 11) && (j <= 3) && (i - j >= 11)))
                        {
                            if (board.CheckMainDiagonalAttack(i, j, ref weightOfAttack))
                            {
                                if (board.GetValue(i, j) == MySymbol)
                                {
                                    myAttacks[weightOfAttack]++;
                                }
                                else
                                {
                                    opponentAttacks[weightOfAttack]++;
                                }
                            }
                        }

                        // Не проверяем побочные диагонали длиной меньше пяти.
                        if (((i <= 3) && (j <= 3) && (i + j <= 3))
                            || ((i >= 11) && (j >= 11) && (i + j >= 25)))
                        {
                            if (board.CheckAntiDiagonalAttack(i, j, ref weightOfAttack))
                            {
                                if (board.GetValue(i, j) == MySymbol)
                                {
                                    myAttacks[weightOfAttack]++;
                                }
                                else
                                {
                                    opponentAttacks[weightOfAttack]++;
                                }
                            }
                        }
                    }
                }
            }
            if (potentialOpenFour)
            {
                return isMax ? Int32.MinValue : Int32.MaxValue;
            }
            var multiplier = 1;
            var eval = 0;
            for (var i = 1; i < 4; i++)
            {
                eval += myAttacks[i] * multiplier - opponentAttacks[i] * multiplier * 10;
                multiplier *= 100;
            }
            return eval;
        }
    }
}