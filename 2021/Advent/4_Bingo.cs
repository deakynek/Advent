using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class Bingo
    {
        List<int> entries;
        List<List<List<int>>> Boards = new List<List<List<int>>>();

        public Bingo(List<string> lines)
        {
            entries = lines.First().Trim().Split(',').Select(x=>int.Parse(x)).ToList();

            //set up boards
            for(int i = 2; i<lines.Count; i++)
            {
                if(String.IsNullOrWhiteSpace(lines[i]))
                    continue;

                
                var brandNewBoard = new List<List<int>>();
                for(int j = i; j<i+5; j++)
                {
                    var jline = lines[j].Trim().Split(' ');
                    var thisLine = new List<int>();
                    foreach(var ent in jline)
                    {
                        if (String.IsNullOrWhiteSpace(ent))
                            continue;
                        thisLine.Add(int.Parse(ent));
                    }

                    brandNewBoard.Add(thisLine.ToList());
                }

                Boards.Add(brandNewBoard);
                i+=5;
            }

            FindWinningBoard();
            FindLastWinningBoard();
        }

        private void FindWinningBoard()
        {
            var boardFound = false;
            var boardNum = 0;
            for(int i = 0; i< entries.Count; i++)
            {
                var calledNumbers = entries.Take(i+1).ToList();
                

                for(int b = 0; b< Boards.Count; b++)
                {
                    if (CheckBoard(Boards[b], calledNumbers))
                    {
                        boardFound = true;
                        boardNum = b;
                    }
                }
                if(boardFound)
                {
                    Console.WriteLine(String.Format("board {0} wins with score of {1}",boardNum,CalcBoardScore(Boards[boardNum],calledNumbers)));
                    break;
                }
            }
        }

        private void FindLastWinningBoard()
        {
            var winningBoards = new List<int>();

            for(int i = 0; i< entries.Count; i++)
            {
                var calledNumbers = entries.Take(i+1).ToList();
                for(int b = 0; b< Boards.Count; b++)
                {
                    if(winningBoards.Contains(b))
                        continue;

                    if (CheckBoard(Boards[b], calledNumbers))
                    {
                        winningBoards.Add(b);
                    }
                }
                if(winningBoards.Count == Boards.Count)
                {
                    Console.WriteLine(String.Format("board {0} wins last with score of {1}",winningBoards.Last(),CalcBoardScore(Boards[winningBoards.Last()],calledNumbers)));
                    break;
                }
            }
        }

        private bool CheckBoard(List<List<int>> board, List<int>calledNumbers)
        {
            if (board.Any(r => r.All(e => calledNumbers.Contains(e))))
                return true;

            for(int i = 0; i< board.Count; i++)
            {
                if(board.All(r=> calledNumbers.Contains(r[i])))
                    return true;
            }

            return false;
        }

        private int CalcBoardScore(List<List<int>> board, List<int>calledNumbers)
        {
            var sum = 0;
            for(int i=0;i<board.Count; i++)
            {
                for(int j = 0; j<board[0].Count; j++)
                {
                    if(!calledNumbers.Contains(board[i][j]))
                        sum += board[i][j];
                }
            }

            return sum*calledNumbers.Last();
        }
    }    
}