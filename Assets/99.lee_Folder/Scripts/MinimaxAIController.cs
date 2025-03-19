// using System;
// using UnityEngine;
// using lee_namespace;
//
// public static class MinimaxAIController
// {
//     public static (int row, int col)? GetBestMove(GameManager.PlayerType[,] board)
//     {
//         float bestScore = -1000;
//         (int row, int col)? bestMove = null;
//         
//         for (var row = 0; row < board.GetLength(0); row++)
//         {
//             for (var col = 0; col < board.GetLength(1); col++)
//             {
//                 if (board[row, col] == GameManager.PlayerType.None)
//                 {
//                     board[row, col] = GameManager.PlayerType.PlayerB;
//                     var score = DoMinimax(board, 0, false);
//                     board[row, col] = GameManager.PlayerType.None;
//                     
//                     if (score > bestScore)
//                     {
//                         bestScore = score;
//                         bestMove = (row, col);
//                     }
//                 }
//             }
//         }
//         
//         return bestMove;
//     }
//
//     private static float DoMinimax(GameManager.PlayerType[,] board, int depth, bool isMaximizing)
//     {
//         if (CheckGameWin(GameManager.PlayerType.PlayerA, board))
//             return -10 + depth;
//         if (CheckGameWin(GameManager.PlayerType.PlayerB, board))
//             return 10 - depth;
//         if (IsAllBlocksPlaced(board))
//             return 0;
//
//         if (isMaximizing)
//         {
//             var bestScore = float.MinValue;
//             for (var row = 0; row < board.GetLength(0); row++)
//             {
//                 for (var col = 0; col < board.GetLength(1); col++)
//                 {
//                     if (board[row, col] == GameManager.PlayerType.None)
//                     {
//                         board[row, col] = GameManager.PlayerType.PlayerB;
//                         var score = DoMinimax(board, depth + 1, false);
//                         board[row, col] = GameManager.PlayerType.None;
//                         bestScore = Math.Max(bestScore, score);
//                     }
//                 }
//             }
//             return bestScore;
//         }
//         else
//         {
//             var bestScore = float.MaxValue;
//             for (var row = 0; row < board.GetLength(0); row++)
//             {
//                 for (var col = 0; col < board.GetLength(1); col++)
//                 {
//                     if (board[row, col] == GameManager.PlayerType.None)
//                     {
//                         board[row, col] = GameManager.PlayerType.PlayerA;
//                         var score = DoMinimax(board, depth + 1, true);
//                         board[row, col] = GameManager.PlayerType.None;
//                         bestScore = Math.Min(bestScore, score);
//                     }
//                 }
//             }
//             return bestScore;
//         }
//     }
//     
//     /// <summary>
//     /// 모든 마커가 보드에 배치 되었는지 확인하는 함수
//     /// </summary>
//     /// <returns>True: 모두 배치</returns>
//     public static bool IsAllBlocksPlaced(GameManager.PlayerType[,] board)
//     {
//         for (var row = 0; row < board.GetLength(0); row++)
//         {
//             for (var col = 0; col < board.GetLength(1); col++)
//             {
//                 if (board[row, col] == GameManager.PlayerType.None)
//                     return false;
//             }
//         }
//         return true;
//     }
//     
//     /// <summary>
//     /// 게임의 승패를 판단하는 함수
//     /// </summary>
//     /// <param name="playerType"></param>
//     /// <param name="board"></param>
//     /// <returns></returns>
//     private static bool CheckGameWin(GameManager.PlayerType playerType, GameManager.PlayerType[,] board)
//     {
//         // 가로로 마커가 일치하는지 확인
//         for (var row = 0; row < board.GetLength(0); row++)
//         {
//             for (var col = 0; col < board.GetLength(1) - 5 + 1; col++)
//             {
//                 if (board[row, col] == playerType && board[row, col + 1] == playerType &&
//                     board[row, col + 2] == playerType && board[row, col + 3] == playerType &&
//                     board[row, col + 4] == playerType)
//                 {
//                     return true;
//                 }
//             }
//         }
//         
//         // 세로로 마커가 일치하는지 확인
//         for (var col = 0; col < board.GetLength(0); col++)
//         {
//             for (var row = 0; row < board.GetLength(1) - 5 + 1; row++)
//             {
//                 if (board[row, col] == playerType && board[row + 1, col] == playerType &&
//                     board[row + 2, col] == playerType && board[row + 3, col] == playerType &&
//                     board[row + 4, col] == playerType)
//                 {
//                     return true;
//                 }
//             }
//         }
//         
//         // \ 대각선 마커 일치하는지 확인
//         for (var row = 0; row < board.GetLength(0) - 5 + 1; row++)
//         {
//             for (var col = 0; col < board.GetLength(1) - 5 + 1; col++)
//             {
//                 if (board[row, col] == playerType && board[row + 1, col + 1] == playerType &&
//                     board[row + 2, col + 2] == playerType && board[row + 3, col + 3] == playerType &&
//                     board[row + 4, col + 4] == playerType)
//                 {
//                     return true;
//                 }
//             }
//         }
//
//         // / 대각선 마커 일치하는지 확인
//         for (var row = 0; row < board.GetLength(0) - 5 + 1; row++)
//         {
//             for (var col = 0; col < board.GetLength(1) - 5 + 1; col++)
//             {
//                 if (board[row, col + 4] == playerType && board[row + 1, col + 3] == playerType &&
//                     board[row + 2, col + 2] == playerType && board[row + 3, col + 1] == playerType &&
//                     board[row + 4, col] == playerType)
//                 {
//                     return true;
//                 }
//             }
//         }
//         return false;
//     }
// }