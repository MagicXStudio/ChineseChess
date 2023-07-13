using System;

namespace ChineseChess
{
    public class GameController
    {
        public const int Rows = 10;  //行总数
        public const int Columns = 9;   //列总数
        private int[][] Pieces { get; set; }   //代表着全场象棋的数组
        private bool[][] avalChess { get; set; } //选择棋子时的可选表
        private int[][] Tips { get; set; } //移动棋子时的提示表
        private bool whosTurn { get; set; }


        private const int minX = 0, minY = 0, maxX = 8, maxY = 9;

        public GameController()
        {
            Pieces = new int[Rows][];
            avalChess = new bool[Rows][];
            Tips = new int[Rows][];
            for (int i = 0; i < Rows; ++i)
            {
                Pieces[i] = new int[Columns];
                avalChess[i] = new bool[Columns];
                Tips[i] = new int[Columns];
            }
        }

        /// <summary>
        /// 初始化棋盘
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < Pieces.Length; ++i)
                for (int j = 0; j < Pieces[i].Length; ++j)
                    switch (i * Pieces[i].Length + j)
                    {
                        case 0: Pieces[i][j] = -6; break;
                        case 89: Pieces[i][j] = 6; break;
                        case 1: Pieces[i][j] = -7; break;
                        case 88: Pieces[i][j] = 7; break;
                        case 2: Pieces[i][j] = -5; break;
                        case 87: Pieces[i][j] = 5; break;
                        case 3: Pieces[i][j] = -3; break;
                        case 86: Pieces[i][j] = 3; break;
                        case 4: Pieces[i][j] = -1; break;
                        case 85: Pieces[i][j] = 1; break;
                        case 5: Pieces[i][j] = -3; break;
                        case 84: Pieces[i][j] = 3; break;
                        case 6: Pieces[i][j] = -5; break;
                        case 83: Pieces[i][j] = 5; break;
                        case 7: Pieces[i][j] = -7; break;
                        case 82: Pieces[i][j] = 7; break;
                        case 8: Pieces[i][j] = -6; break;
                        case 81: Pieces[i][j] = 6; break;
                        case 25: Pieces[i][j] = -4; break;
                        case 70: Pieces[i][j] = 4; break;
                        case 19: Pieces[i][j] = -4; break;
                        case 64: Pieces[i][j] = 4; break;
                        case 27: Pieces[i][j] = -2; break;
                        case 62: Pieces[i][j] = 2; break;
                        case 29: Pieces[i][j] = -2; break;
                        case 60: Pieces[i][j] = 2; break;
                        case 31: Pieces[i][j] = -2; break;
                        case 58: Pieces[i][j] = 2; break;
                        case 33: Pieces[i][j] = -2; break;
                        case 56: Pieces[i][j] = 2; break;
                        case 35: Pieces[i][j] = -2; break;
                        case 54: Pieces[i][j] = 2; break;
                        default: Pieces[i][j] = 0; break;
                    }
            System.Diagnostics.Debug.WriteLine(Pieces);
            //初始化先手和可选棋表
            whosTurn = true;
            TransTurn();
        }

        //获取棋子
        public int GetChessman(int i, int j)
        {
            return Pieces[i][j];
        }

        //获取可移动状态
        public bool IsAvaliable(int i, int j)
        {
            return avalChess[i][j];
        }

        //获取移动目标状态
        public int GetMoveHelper(int i, int j)
        {
            return Tips[i][j];
        }

        //设置可移动状态，己方棋子及自身=3，可选敌方棋子=1，可选空位=2，不可选棋子0
        public void SetMoveHelper(int i, int j)
        {
            for (int a = 0; a < Rows; ++a)
            {
                for (int b = 0; b < Columns; ++b)
                {
                    if (i == -1 && j == -1)
                    {
                        Tips[a][b] = -1;
                        continue;
                    }
                    if (GetChessman(a, b) * GetChessman(i, j) > 0)
                        Tips[a][b] = -1;
                    else if (CanMove(i, j, a, b) == true)
                    {
                        if (GetChessman(a, b) == 0)
                            Tips[a][b] = 2;
                        else
                            Tips[a][b] = 1;
                    }
                    else
                        Tips[a][b] = 0;
                }
            }
        }

        //游戏结束判断
        public bool IsGameOver(int i, int j)
        {
            if (Math.Abs(Pieces[i][j]) == 1)
                return true;
            return false;
        }

        //游戏结束
        public bool GameOver()
        {
            //使所有棋子不能再移动，并返回赢家
            for (int i = 0; i < avalChess.Length; ++i)
                for (int j = 0; j < avalChess[i].Length; ++j)
                    avalChess[i][j] = false;
            return !whosTurn;
        }

        //获取路径中棋子个数
        public int GetChessmanCountInPath(int oldi, int oldj, int i, int j)
        {
            if (i != oldi && j != oldj)
                return -1;
            int count = 0;
            if (i == oldi)
            {
                int maxer = Math.Max(oldj, j);
                int miner = Math.Min(oldj, j);
                for (int loop = miner + 1; loop < maxer; ++loop)
                    if (Pieces[i][loop] != 0)
                        count++;
            }
            else if (j == oldj)
            {
                int maxer = Math.Max(oldi, i);
                int miner = Math.Min(oldi, i);
                for (int loop = miner + 1; loop < maxer; ++loop)
                    if (Pieces[loop][j] != 0)
                        count++;
            }
            return count;
        }

        //判断是否可以移动
        public bool CanMove(int oldi, int oldj, int i, int j)
        {
            //是否越界
            if (i < minY || i > maxY || j < minX || j > maxX)
                return false;
            //是否移动到本方棋子上
            if ((Pieces[i][j] > 0 && Pieces[oldi][oldj] > 0) || (Pieces[i][j] < 0 && Pieces[oldi][oldj] < 0))
                return false;
            //是否满足特定棋子的规律
            int type = Pieces[oldi][oldj];
            switch (Math.Abs(type))
            {
                case 0: return false;
                //帅/将 必须在九宫格内，每次只能移动一步，或者移动到对方的将
                case 1:
                    //移动到对方的将，且中间没有阻挡
                    if (Pieces[i][j] == -Pieces[oldi][oldj] && GetChessmanCountInPath(oldi, oldj, i, j) == 0)
                        return true;
                    //目标仅移动一步
                    if (Math.Abs(i - oldi) + Math.Abs(j - oldj) != 1)
                        return false;
                    //目标在九宫格内
                    if (type < 0 && (i >= 0 && i <= 2 && j >= 3 && j <= 5))
                        return true;
                    else if (type > 0 && (i >= 7 && i <= 9 && j >= 3 && j <= 5))
                        return true;
                    break;
                //兵/卒 只能平移或者向对方移动
                case 2:
                    //目标仅移动一步
                    if (Math.Abs(i - oldi) + Math.Abs(j - oldj) != 1)
                        return false;
                    //目标平移或向对方移动
                    if (type < 0 && (i > oldi || (i == oldi && i >= 5)))
                        return true;
                    if (type > 0 && (i < oldi || (i == oldi && i < 5)))
                        return true;
                    break;
                //仕/士 必须在九宫格内，每次只能斜线一步
                case 3:
                    //目标仅斜线移动一步
                    if (!(Math.Abs(i - oldi) == 1 && Math.Abs(j - oldj) == 1))
                        return false;
                    //目标在九宫格内
                    if (type < 0 && (i >= 0 && i <= 2 && j >= 3 && j <= 5))
                        return true;
                    else if (type > 0 && (i >= 7 && i <= 9 && j >= 3 && j <= 5))
                        return true;
                    break;
                //炮/砲 仅可直线移动，且如果路径中有棋子(有且仅有1个)，必须移动到敌方棋子上
                case 4:
                    //仅可直线移动
                    if (i != oldi && j != oldj)
                        return false;
                    //如果目标为空位，那么路径中不能有棋子，否则必须有1个
                    if (GetChessman(i, j) == 0 && GetChessmanCountInPath(oldi, oldj, i, j) == 0)
                        return true;
                    if (GetChessman(i, j) != 0 && GetChessmanCountInPath(oldi, oldj, i, j) == 1)
                        return true;
                    break;
                //相/象 仅可田字型移动，且不可卡位，且不可过河
                case 5:
                    //不可过河
                    if (type < 0 && i >= 5)
                        return false;
                    if (type > 0 && i < 5)
                        return false;
                    //仅可田字型移动
                    if (!(Math.Abs(i - oldi) == 2 && Math.Abs(j - oldj) == 2))
                        return false;
                    //不可被卡位
                    if (i > oldi && j > oldj && Pieces[oldi + 1][oldj + 1] == 0)
                        return true;
                    if (i > oldi && j < oldj && Pieces[oldi + 1][oldj - 1] == 0)
                        return true;
                    if (i < oldi && j > oldj && Pieces[oldi - 1][oldj + 1] == 0)
                        return true;
                    if (i < oldi && j < oldj && Pieces[oldi - 1][oldj - 1] == 0)
                        return true;
                    break;
                //车/车 仅可直线移动，且路径中不能有棋子
                case 6:
                    //仅可直线移动
                    if (i != oldi && j != oldj)
                        return false;
                    //路径中不能有棋子
                    if (GetChessmanCountInPath(oldi, oldj, i, j) == 0)
                        return true;
                    break;
                //马/马 仅可日字型移动，且不能被卡位
                case 7:
                    //仅可日字型移动
                    if (!((Math.Abs(i - oldi) == 2 && Math.Abs(j - oldj) == 1) || (Math.Abs(i - oldi) == 1 && Math.Abs(j - oldj) == 2)))
                        return false;
                    //不能被卡位
                    if (Math.Abs(i - oldi) == 2)
                    {
                        if (i > oldi && j > oldj && Pieces[oldi + 1][oldj] == 0)
                            return true;
                        if (i > oldi && j < oldj && Pieces[oldi + 1][oldj] == 0)
                            return true;
                        if (i < oldi && j > oldj && Pieces[oldi - 1][oldj] == 0)
                            return true;
                        if (i < oldi && j < oldj && Pieces[oldi - 1][oldj] == 0)
                            return true;
                    }
                    else if (Math.Abs(j - oldj) == 2)
                    {
                        if (i > oldi && j > oldj && Pieces[oldi][oldj + 1] == 0)
                            return true;
                        if (i > oldi && j < oldj && Pieces[oldi][oldj - 1] == 0)
                            return true;
                        if (i < oldi && j > oldj && Pieces[oldi][oldj + 1] == 0)
                            return true;
                        if (i < oldi && j < oldj && Pieces[oldi][oldj - 1] == 0)
                            return true;
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }

        //还原棋子
        public void ResetChessman(int oldi, int oldj, int i, int j, int oldValue, int value)
        {
            Pieces[i][j] = value;
            Pieces[oldi][oldj] = oldValue;
            TransTurn();
        }

        //移动棋子
        public void SetChessman(int oldi, int oldj, int i, int j)
        {
            Pieces[i][j] = Pieces[oldi][oldj];
            Pieces[oldi][oldj] = 0;
            TransTurn();
        }

        //跳过回合，防止TransTurn被滥用
        public void SkipTurn()
        {
            TransTurn();
        }

        //改变回合
        private void TransTurn()
        {
            whosTurn = !whosTurn;
            for (int i = 0; i < avalChess.Length; ++i)
                for (int j = 0; j < avalChess[i].Length; ++j)
                    if (Pieces[i][j] < 0 && whosTurn == false)
                        avalChess[i][j] = true;
                    else if (Pieces[i][j] > 0 && whosTurn == true)
                        avalChess[i][j] = true;
                    else avalChess[i][j] = false;
        }
    }
}
