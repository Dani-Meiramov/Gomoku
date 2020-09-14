using System;

namespace Gomoku
{
    class Board
    {
        // Размер игровой доски.
        public static readonly int N = 15;
        // Количество камней в ряд достаточных для победы.
        public static readonly int M = 5;
        // Количество занятых клеток на доске.
        public int OccupiedCells { get; set; } = 0;
        // Игровая доска.
        private char[,] board = new char[N, N];
        public char[,] GetBoard() => board;
        public char GetValue(int i, int j) => board[i, j];
        public void SetValue(int i, int j, char symbol)
        {
            board[i, j] = symbol;
            if (symbol != '_')
            {
                OccupiedCells++;
            }
            else
            {
                OccupiedCells--;
            }
        }

        public Board()
        {
            for (var i = 0; i < N; i++)
            {
                for (var j = 0; j < N; j++)
                {
                    board[i, j] = '_';
                }
            }
        }

        /// <summary>
        /// Проверяет не вышла ли данная ячейка за пределы доски, и если нет, то не занята ли она
        /// </summary>
        /// <param name="i">Первая координата</param>
        /// <param name="j">Вторая координата</param>
        /// <returns>Возвращает true если ячейка на доске пустая</returns>
        public bool CheckCellIsValidAndEmpty(int i, int j)
        {
            if ((i < 0) || (i >= N) || (j < 0) || (j >= N))
            {
                //Console.WriteLine("\nThe cell is outside of the board. Select another cell");
                return false;
            }
            else if (board[i, j] != '_')
            {
                //Console.WriteLine("\nThe cell is already in use. Select another cell");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Проверяет есть ли на расстоянии одной клетки какой-нибудь символ (x или o)
        /// </summary>
        /// <param name="i">Первая координата</param>
        /// <param name="j">Вторая координата</param>
        /// <returns>Возвращает true, если есть хотя бы один такой символ</returns>
        public bool СheckExistenceOfAdjacentSymbols(int i, int j)
        {
            int[,] adjacentCells = { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 },
                { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };

            var len = adjacentCells.GetUpperBound(0) + 1;
            for (var k = 0; k < len; k++)
            {
                if ((i + adjacentCells[k, 0] >= 0) && (i + adjacentCells[k, 0] < N)
                    && (j + adjacentCells[k, 1] >= 0) && (j + adjacentCells[k, 1] < N))
                {
                    if (board[i + adjacentCells[k, 0], j + adjacentCells[k, 1]] != '_')
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяет есть ли на расстоянии одной клетки заданный символ (т.е. примыкает к нему)
        /// </summary>
        /// <param name="i">Первая координата</param>
        /// <param name="j">Вторая координата</param>
        /// <param name="symbol">Символ, который нужен</param>
        /// <returns>Возвращает true, если на расстоянии одной клетки есть заданный символ</returns>
        public bool CheckExistenceOfAdjacentSymbol(int i, int j, char symbol)
        {
            int[,] adjacentCells = { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 },
                { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };

            var len = adjacentCells.GetUpperBound(0) + 1;
            for (var k = 0; k < len; k++)
            {
                if ((i + adjacentCells[k, 0] >= 0) && (i + adjacentCells[k, 0] < N)
                    && (j + adjacentCells[k, 1] >= 0) && (j + adjacentCells[k, 1] < N))
                {
                    if (board[i + adjacentCells[k, 0], j + adjacentCells[k, 1]] == symbol)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяет выиграл ли игрок игру, т.е. собрал ровно 5 камней в ряд
        /// по вертикали/горизонтали или по одной из диагоналей
        /// </summary>
        /// <param name="i">Первая координата</param>
        /// <param name="j">Вторая координата</param>
        /// <returns>Возвращает true, если игрок выиграл игру</returns>
        public bool CheckWin(int i, int j)
        {   // Да, тут не используются дальше параметры out, но это просто частный случай
            // Думаю это лучше чем писать точно такой же метод, но без out параметров
            return NumberOfConsecutiveStonesInRow(i, j, out int left, out int right) == M
                || NumberOfConsecutiveStonesInColumn(i, j, out int up, out int down) == M
                || NumberOfConsecutiveStonesOnMainDiagonal(i, j, out int leftUp, out int rightDown) == M
                || NumberOfConsecutiveStonesOnAntiDiagonal(i, j, out int leftDown, out int rightUp) == M;
        }

        public bool CheckDraw()
        {
            return OccupiedCells == N * N;
        }

        /// <summary>
        /// Проверяет есть ли у игрока открытая четверка по
        /// вертикали/горизонтали или по одной из диагоналей.
        /// </summary>
        /// <param name="i">Первая координата</param>
        /// <param name="j">Вторая координата</param>
        /// <returns></returns>
        public bool CheckOpenFour(int i, int j)
        {
            return RowOfOpenFour(i, j) || ColumnOfOpenFour(i, j)
                || MainDiagonalOfOpenFour(i, j) || AntiDiagonalOfOpenFour(i, j);
        }
        // Проверяет наличие открытой четверки по горизонтали.
        private bool RowOfOpenFour(int i, int j)
        {
            var numberOfConsecutiveStones =
                NumberOfConsecutiveStonesInRow(i, j, out int left, out int right);

            if (numberOfConsecutiveStones == 4
                && CheckCellIsValidAndEmpty(i, j - left)
                && CheckCellIsValidAndEmpty(i, j + right))
            {
                if (((j - left - 1 < 0) || (board[i, j - left - 1] != board[i, j]))
                    && ((j + right + 1 >= N) || (board[i, j + right + 1] != board[i, j])))
                {
                    return true;
                }
            }
            return false;
        }
        // Проверяет наличие открытой четверки по вертикали.
        private bool ColumnOfOpenFour(int i, int j)
        {
            var numberOfConsecutiveStones =
                NumberOfConsecutiveStonesInColumn(i, j, out int up, out int down);

            if (numberOfConsecutiveStones == 4
                && CheckCellIsValidAndEmpty(i - up, j)
                && CheckCellIsValidAndEmpty(i + down, j))
            {
                if (((i - up - 1 < 0) || (board[i - up - 1, j] != board[i, j]))
                    && ((i + down + 1 >= N) || (board[i + down + 1, j] != board[i, j])))
                {
                    return true;
                }
            }
            return false;
        }
        // Проверяет наличие открытой четверки на главной диагонали.
        private bool MainDiagonalOfOpenFour(int i, int j)
        {
            var numberOfConsecutiveStones =
                NumberOfConsecutiveStonesOnMainDiagonal(i, j, out int leftUp, out int rightDown);

            if (numberOfConsecutiveStones == 4
                && CheckCellIsValidAndEmpty(i - leftUp, j - leftUp)
                && CheckCellIsValidAndEmpty(i + rightDown, j + rightDown))
            {
                if ((((i - leftUp - 1 < 0) || (j - leftUp - 1 < 0))
                        || board[i - leftUp - 1, j - leftUp - 1] != board[i, j])
                    && (((i + rightDown + 1 >= N) || (j + rightDown + 1 >= N))
                        || board[i + rightDown + 1, j + rightDown + 1] != board[i, j]))
                {
                    return true;
                }
            }
            return false;
        }
        // Проверяет наличие открытой четверки на побочной диагонали.
        private bool AntiDiagonalOfOpenFour(int i, int j)
        {
            var numberOfConsecutiveStones =
                NumberOfConsecutiveStonesOnAntiDiagonal(i, j, out int leftDown, out int rightUp);

            if (numberOfConsecutiveStones == 4
                && CheckCellIsValidAndEmpty(i + leftDown, j - leftDown)
                && CheckCellIsValidAndEmpty(i - rightUp, j + rightUp))
            {
                if ((((i + leftDown + 1 >= N) || (j - leftDown - 1 < 0))
                        || board[i + leftDown + 1, j - leftDown - 1] != board[i, j])
                    && (((i - rightUp - 1 < 0) || (j + rightUp + 1 >= N))
                        || board[i - rightUp - 1, j + rightUp + 1] != board[i, j]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяет наличие атаки в строке, если атака будет
        /// найдена, то метод изменит значение переменной weightOfAttack.
        /// </summary>
        /// <param name="i">Первая координата</param>
        /// <param name="j">Вторая координата</param>
        /// <param name="weightOfAttack">Вес атаки</param>
        /// <returns>Возвращает true, если атака была найдена</returns>
        public bool CheckRowAttack(int i, int j, ref int weightOfAttack)
        {
            var numberOfConsecutiveStones =
                NumberOfConsecutiveStonesInRow(i, j, out int left, out int right);

            if (numberOfConsecutiveStones == 3
                && (CheckCellIsValidAndEmpty(i, j - left)
                    || CheckCellIsValidAndEmpty(i, j + right)))
            {
                weightOfAttack = 3;
                return true;
            }
            else if (numberOfConsecutiveStones == 2
                && (CheckCellIsValidAndEmpty(i, j - left)
                && CheckCellIsValidAndEmpty(i, j + right)))
            {
                weightOfAttack = 2;
                return true;
            }
            else if (numberOfConsecutiveStones == 1
                && CheckCellIsValidAndEmpty(i, j - left)
                && CheckCellIsValidAndEmpty(i, j + right))
            {
                weightOfAttack = 1;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Проверяет наличие атаки в столбце, если атака будет
        /// найдена, то метод изменит значение переменной weightOfAttack.
        /// </summary>
        /// <param name="i">Первая координата</param>
        /// <param name="j">Вторая координата</param>
        /// <param name="weightOfAttack">Вес атаки</param>
        /// <returns>Возвращает true, если атака была найдена</returns>
        public bool CheckColumnAttack(int i, int j, ref int weightOfAttack)
        {
            var numberOfConsecutiveStones =
                NumberOfConsecutiveStonesInColumn(i, j, out int up, out int down);

            if (numberOfConsecutiveStones == 3
                && (CheckCellIsValidAndEmpty(i - up, j)
                    || CheckCellIsValidAndEmpty(i + down, j)))
            {
                weightOfAttack = 3;
                return true;
            }
            else if (numberOfConsecutiveStones == 2
                && CheckCellIsValidAndEmpty(i - up, j)
                && CheckCellIsValidAndEmpty(i + down, j))
            {
                weightOfAttack = 2;
                return true;
            }
            else if (numberOfConsecutiveStones == 1
                && CheckCellIsValidAndEmpty(i - up, j)
                && CheckCellIsValidAndEmpty(i + down, j))
            {
                weightOfAttack = 1;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Проверяет наличие атаки на главной диагонали, если атака будет
        /// найдена, то метод изменит значение переменной weightOfAttack.
        /// </summary>
        /// <param name="i">Первая координата</param>
        /// <param name="j">Вторая координата</param>
        /// <param name="weightOfAttack">Вес атаки</param>
        /// <returns>Возвращает true, если атака была найдена</returns>
        public bool CheckMainDiagonalAttack(int i, int j, ref int weightOfAttack)
        {
            var numberOfConsecutiveStones =
                NumberOfConsecutiveStonesOnMainDiagonal(i, j, out int leftUp, out int rightDown);

            if (numberOfConsecutiveStones == 3
                && (CheckCellIsValidAndEmpty(i - leftUp, j - leftUp)
                    || CheckCellIsValidAndEmpty(i + rightDown, j + rightDown)))
            {
                weightOfAttack = 3;
                return true;
            }
            else if (numberOfConsecutiveStones == 2
                && CheckCellIsValidAndEmpty(i - leftUp, j - leftUp)
                && CheckCellIsValidAndEmpty(i + rightDown, j + rightDown))
            {
                weightOfAttack = 2;
                return true;
            }
            else if (numberOfConsecutiveStones == 1
                && CheckCellIsValidAndEmpty(i - leftUp, j - leftUp)
                && CheckCellIsValidAndEmpty(i + rightDown, j + rightDown))
            {
                weightOfAttack = 1;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Проверяет наличие атаки на побочной диагонали, если атака будет
        /// найдена, то метод изменит значение переменной weightOfAttack.
        /// </summary>
        /// <param name="i">Первая координата</param>
        /// <param name="j">Вторая координата</param>
        /// <param name="weightOfAttack"></param>
        /// <returns>Возвращает true, если атака была найдена</returns>
        public bool CheckAntiDiagonalAttack(int i, int j, ref int weightOfAttack)
        {
            var numberOfConsecutiveStones =
                NumberOfConsecutiveStonesOnAntiDiagonal(i, j, out int leftDown, out int rightUp);

            if (numberOfConsecutiveStones == 3
                && (CheckCellIsValidAndEmpty(i + leftDown, j - leftDown)
                    || CheckCellIsValidAndEmpty(i - rightUp, j + rightUp)))
            {
                weightOfAttack = 3;
                return true;
            }
            else if (numberOfConsecutiveStones == 2
                && CheckCellIsValidAndEmpty(i + leftDown, j - leftDown)
                && CheckCellIsValidAndEmpty(i - rightUp, j + rightUp))
            {
                weightOfAttack = 2;
                return true;
            }
            else if (numberOfConsecutiveStones == 1
                && CheckCellIsValidAndEmpty(i + leftDown, j - leftDown)
                && CheckCellIsValidAndEmpty(i - rightUp, j + rightUp))
            {
                weightOfAttack = 1;
                return true;
            }
            return false;
        }

        // Высчитывает количество камней идущих подряд в строке.
        private int NumberOfConsecutiveStonesInRow(int i, int j, out int left, out int right)
        {
            var numberOfConsecutiveStones = 1;
            left = 1;
            while ((j - left >= 0) && (board[i, j - left] == board[i, j]))
            {
                numberOfConsecutiveStones++;
                left++;
            }
            right = 1;
            while ((j + right < N) && (board[i, j + right] == board[i, j]))
            {
                numberOfConsecutiveStones++;
                right++;
            }
            return numberOfConsecutiveStones;
        }
        // Высчитывает количество камней идущих подряд в столбце.
        private int NumberOfConsecutiveStonesInColumn(int i, int j, out int up, out int down)
        {
            var numberOfConsecutiveStones = 1;
            up = 1;
            while ((i - up >= 0) && (board[i - up, j] == board[i, j]))
            {
                numberOfConsecutiveStones++;
                up++;
            }
            down = 1;
            while ((i + down < N) && (board[i + down, j] == board[i, j]))
            {
                numberOfConsecutiveStones++;
                down++;
            }
            return numberOfConsecutiveStones;
        }
        // Высчитывает количество камней идущих подряд на главной диагонали.
        private int NumberOfConsecutiveStonesOnMainDiagonal(int i, int j, out int leftUp, out int rightDown)
        {
            var numberOfConsecutiveStones = 1;
            leftUp = 1;
            while ((i - leftUp >= 0) && (j - leftUp >= 0)
                && board[i - leftUp, j - leftUp] == board[i, j])
            {
                numberOfConsecutiveStones++;
                leftUp++;
            }
            rightDown = 1;
            while ((i + rightDown < N) && (j + rightDown < N)
                && board[i + rightDown, j + rightDown] == board[i, j])
            {
                numberOfConsecutiveStones++;
                rightDown++;
            }
            return numberOfConsecutiveStones;
        }
        // Высчитывает количество камней идущих подряд на побочной диагонали.
        private int NumberOfConsecutiveStonesOnAntiDiagonal(int i, int j, out int leftDown, out int rightUp)
        {
            var numberOfConsecutiveStones = 1;
            leftDown = 1;
            while ((i + leftDown < N) && (j - leftDown >= 0)
                && board[i + leftDown, j - leftDown] == board[i, j])
            {
                numberOfConsecutiveStones++;
                leftDown++;
            }
            rightUp = 1;
            while ((i - rightUp >= 0) && (j + rightUp < N)
                && board[i - rightUp, j + rightUp] == board[i, j])
            {
                numberOfConsecutiveStones++;
                rightUp++;
            }
            return numberOfConsecutiveStones;
        }

        /// <summary>
        /// Выводит на консоль текущее состояние игровой доски
        /// </summary>
        public void Print()
        {
            Console.Write("   ");
            for (var i = 0; i < N; i++)
            {
                Console.Write($"{i}{(i < 10 ? "  " : " ")}");
            }
            Console.WriteLine();
            for (var i = 0; i < N; i++)
            {
                Console.Write($"{i}{(i < 10 ? "  " : " ")}");
                for (var j = 0; j < N; j++)
                {
                    Console.Write($"{board[i, j]}  ");
                }
                Console.WriteLine($"{i}\n");
            }
            Console.Write("   ");
            for (var i = 0; i < N; i++)
            {
                Console.Write($"{i}{(i < 10 ? "  " : " ")}");
            }
            Console.WriteLine("\n");
        }
    }
}