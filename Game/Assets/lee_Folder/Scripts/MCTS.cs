using System.Collections.Generic;
using UnityEngine;
using lee_namespace;

namespace lee_namespace
{
    public class MCTS
    {
        private MCTSNode root;
        private GameManager.PlayerType[,] initialBoard;
        private const int BOARD_SIZE = 15;

        public MCTS(GameManager.PlayerType[,] board)
        {
            initialBoard = (GameManager.PlayerType[,])board.Clone(); // 원본 Board 보호
            root = new MCTSNode(null, -1, -1, initialBoard, GameManager.PlayerType.PlayerB); // 첫 백돌
        }

        public (int row, int col) GetBestMove(int simulations)
        {
            var firstDefenseBlock = CheckFourSituation(initialBoard, GameManager.PlayerType.PlayerA);

            if (firstDefenseBlock.HasValue)
            {
                Debug.Log($" 우선 막아야할 위치 :  ({firstDefenseBlock.Value.row}, {firstDefenseBlock.Value.col})");
                return firstDefenseBlock.Value;
            }

            for (int i = 0; i < simulations; i++)
            {
                MCTSNode selectedNode = Select(root);
                MCTSNode expandedNode = Expand(selectedNode);
                double result = Simulate(expandedNode);
                Backpropagate(expandedNode, result);
            }

            MCTSNode bestChild = null;
            int maxVisits = -1;
            foreach (var child in root.children)
            {
                if (child.visits > maxVisits)
                {
                    maxVisits = child.visits;
                    bestChild = child;
                }
            }

            return (bestChild.row, bestChild.col);
        }

        /// <summary>
        /// node에서 시작해 자식노드가 없는 노드 찾을 때까지 UCB1 값이 가장높은 자식노드로 이동
        /// </summary>
        /// <param name="node">탐색할 노드</param>
        /// <returns>탐색할 노드</returns>
        private MCTSNode Select(MCTSNode node)
        {
            while (node.children.Count > 0) // 자식있는 경우 
            {
                node = node.children[0]; // 노드에 자식[0]을 넣어줌
                double maxUCB = node.GetUCB1();
                foreach (var child in node.children)
                {
                    double ucb = child.GetUCB1();
                    if (ucb > maxUCB)
                    {
                        maxUCB = ucb;
                        node = child;
                    }
                }
            }

            return node;
        }

        /// <summary>
        /// Select에서 선택된 node에 새로운 자식 노드를 추가하여 트리확장
        /// </summary>
        /// <param name="node">Select에서 선택된 node</param>
        /// <returns>새로 추가된 노드</returns>
        private MCTSNode Expand(MCTSNode node)
        {
            if (IsGameOver(node.boardState)) return node;

            List<(int row, int col, int score)> possibleMoves =
                GetHeuristicMoves(node.boardState,
                    node.currentPlayer,
                    GetMovesMade(node.boardState));
            if (possibleMoves.Count == 0) return node;

            var (row, col, _) = possibleMoves[0]; // 최고 점수의 수 선택
            var newBoard = (GameManager.PlayerType[,])node.boardState.Clone();
            newBoard[row, col] = node.currentPlayer;
            var nextPlayer = node.currentPlayer == GameManager.PlayerType.PlayerA
                ? GameManager.PlayerType.PlayerB
                : GameManager.PlayerType.PlayerA;
            MCTSNode child = new MCTSNode(node, row, col, newBoard, nextPlayer);
            node.children.Add(child);
            return child;
        }

        private double Simulate(MCTSNode node)
        {
            var board = (GameManager.PlayerType[,])node.boardState.Clone();
            var currentPlayer = node.currentPlayer;
            int movesMade = GetMovesMade(board);

            while (!IsGameOver(board))
            {
                var possibleMoves = GetHeuristicMoves(board, currentPlayer, movesMade);
                if (possibleMoves.Count == 0) return 0.5;

                var (row, col, _) = possibleMoves[0]; // 최고 점수 수 선택
                board[row, col] = currentPlayer;
                currentPlayer = currentPlayer == GameManager.PlayerType.PlayerA
                    ? GameManager.PlayerType.PlayerB
                    : GameManager.PlayerType.PlayerA;
                movesMade++;
            }

            if (HasFiveInARow(board, GameManager.PlayerType.PlayerB)) return 1;
            if (HasFiveInARow(board, GameManager.PlayerType.PlayerA)) return 0;
            return 0.5;
        }

        private void Backpropagate(MCTSNode node, double result)
        {
            while (node != null)
            {
                node.visits++;
                node.score += (node.currentPlayer == GameManager.PlayerType.PlayerB) ? result : 1 - result;
                node = node.parent;
            }
        }

        /// <summary>
        /// 보드에서 현재 돌을 놓을 수 있는 위치를 모두 계산.
        /// </summary>
        /// <param name="board">보드</param>
        /// <returns></returns>
        private List<(int row, int col)> GetPossibleMoves(GameManager.PlayerType[,] board)
        {
            List<(int row, int col)> moves = new List<(int row, int col)>();
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    if (board[r, c] == GameManager.PlayerType.None)
                        moves.Add((r, c));
                }
            }

            return moves;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="board">보드</param>
        /// <param name="currentPlayer">현재 수를 두는 플레이어</param>
        /// <param name="currentBoardAllCount">현재 보드에 놓인 돌의 총 개수</param>
        /// <returns></returns>
        private List<(int row, int col, int score)> GetHeuristicMoves(GameManager.PlayerType[,] board,
            GameManager.PlayerType currentPlayer, int currentBoardAllCount)
        {
            var allMoves = GetPossibleMoves(board); // 놓을 수있는 칸 모두 담기.
            List<(int row, int col)> moves;

            // 초반 5수 - 초반에 흑돌의 위치와는 무관한 위치에 백돌이 놓여지는 문제점 발견. 
            if (currentBoardAllCount < 5)
            {
                moves = new List<(int row, int col)>();
                foreach (var (row, col) in allMoves)
                {
                    if (IsNearOpponent(board, row, col, GameManager.PlayerType.PlayerA))
                        moves.Add((row, col));
                }

                if (moves.Count == 0) moves = allMoves;
            }
            else
            {
                moves = allMoves;
            }

            var scoredMoves = new List<(int row, int col, int score)>();
            foreach (var (row, col) in moves)
            {
                int score = HeuristicScore(board, row, col, currentPlayer, currentBoardAllCount);
                scoredMoves.Add((row, col, score));
            }

            scoredMoves.Sort((a, b) => b.score.CompareTo(a.score));
            return scoredMoves;
        }

        //
        private int HeuristicScore(GameManager.PlayerType[,] board, int row, int col,
            GameManager.PlayerType currentPlayer,
            int movesMade)
        {
            int score = 0;
            var tempBoard = (GameManager.PlayerType[,])board.Clone();
            tempBoard[row, col] = currentPlayer;

            // AI가 5개 연속 만들기
            if (HasFiveInARow(tempBoard, GameManager.PlayerType.PlayerB))
                score += 2000;

            // AI 자신의 연속 돌 만들기
            int aiCount = CountConsecutive(tempBoard, row, col, GameManager.PlayerType.PlayerB);
            if (aiCount == 4) score += 800;
            else if (aiCount == 3) score += 900;

            // 플레이어의 흑돌 3개 4개 막기
            int opponentCount = CountConsecutive(tempBoard, row, col, GameManager.PlayerType.PlayerA);
            if (opponentCount == 4) score += 500; // 4개 연속 막기
            else if (opponentCount == 3) score += 1000; // 3개 연속 막기

            // 흑돌 주변에 두기
            if (IsNearOpponent(tempBoard, row, col, GameManager.PlayerType.PlayerA))
                score += 500;

            return score;
        }

        private int CountConsecutive(GameManager.PlayerType[,] board, int row, int col, GameManager.PlayerType player)
        {
            int maxCount = 0;

            for (int c = Mathf.Max(0, col - 4); c <= Mathf.Min(BOARD_SIZE - 5, col); c++)
            {
                int count = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (board[row, c + i] == player)
                        count++;
                }

                maxCount = Mathf.Max(maxCount, count);
            }

            for (int r = Mathf.Max(0, row - 4); r <= Mathf.Min(BOARD_SIZE - 5, row); r++)
            {
                int count = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (board[r + i, col] == player)
                        count++;
                }

                maxCount = Mathf.Max(maxCount, count);
            }

            for (int d = -4; d <= 0; d++)
            {
                if (row + d >= 0 && col + d >= 0 && row + d + 4 < BOARD_SIZE && col + d + 4 < BOARD_SIZE)
                {
                    int count = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (board[row + d + i, col + d + i] == player)
                            count++;
                    }

                    maxCount = Mathf.Max(maxCount, count);
                }
            }

            for (int d = -4; d <= 0; d++)
            {
                if (row + d >= 0 && col - d - 4 >= 0 && row + d + 4 < BOARD_SIZE && col - d < BOARD_SIZE)
                {
                    int count = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (board[row + d + i, col - d - i] == player)
                            count++;
                    }

                    maxCount = Mathf.Max(maxCount, count);
                }
            }

            return maxCount;
        }

        /// <summary>
        /// row,col 기준 주변 1칸이내에 적의 수가 있는지 확인
        /// </summary>
        /// <param name="board">보드</param>
        /// <param name="row">확인하고싶은 행</param>
        /// <param name="col">확인하고싶은 열</param>
        /// <param name="opponent">상대방 플레이어 돌</param>
        /// <returns>true : 지정된 위치는 상대방 돌 주변
        /// false : 지정된 위치가 상대방 돌 주변에 없음</returns>
        private bool IsNearOpponent(GameManager.PlayerType[,] board, int row, int col, GameManager.PlayerType opponent)
        {
            for (int rOffset = -1; rOffset <= 1; rOffset++)
            {
                for (int cOffset = -1; cOffset <= 1; cOffset++)
                {
                    int r = row + rOffset;
                    int c = col + cOffset;
                    if (r >= 0 && r < BOARD_SIZE && c >= 0 && c < BOARD_SIZE && board[r, c] == opponent)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 게임종료 판단 함수
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        private bool IsGameOver(GameManager.PlayerType[,] board)
        {
            return HasFiveInARow(board, GameManager.PlayerType.PlayerA) ||
                   HasFiveInARow(board, GameManager.PlayerType.PlayerB) ||
                   GetPossibleMoves(board).Count == 0;
        }

        private bool HasFiveInARow(GameManager.PlayerType[,] board, GameManager.PlayerType player)
        {
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c <= BOARD_SIZE - 5; c++)
                {
                    if (board[r, c] == player && board[r, c + 1] == player && board[r, c + 2] == player &&
                        board[r, c + 3] == player && board[r, c + 4] == player)
                        return true;
                }
            }

            for (int c = 0; c < BOARD_SIZE; c++)
            {
                for (int r = 0; r <= BOARD_SIZE - 5; r++)
                {
                    if (board[r, c] == player && board[r + 1, c] == player && board[r + 2, c] == player &&
                        board[r + 3, c] == player && board[r + 4, c] == player)
                        return true;
                }
            }

            for (int r = 0; r <= BOARD_SIZE - 5; r++)
            {
                for (int c = 0; c <= BOARD_SIZE - 5; c++)
                {
                    if (board[r, c] == player && board[r + 1, c + 1] == player && board[r + 2, c + 2] == player &&
                        board[r + 3, c + 3] == player && board[r + 4, c + 4] == player)
                        return true;
                }
            }

            for (int r = 0; r <= BOARD_SIZE - 5; r++)
            {
                for (int c = 4; c < BOARD_SIZE; c++)
                {
                    if (board[r, c] == player && board[r + 1, c - 1] == player && board[r + 2, c - 2] == player &&
                        board[r + 3, c - 3] == player && board[r + 4, c - 4] == player)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 보드에 놓인 돌 개수 계산
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        private int GetMovesMade(GameManager.PlayerType[,] board)
        {
            int count = 0;
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    if (board[r, c] != GameManager.PlayerType.None)
                        count++;
                }
            }

            return count;
        }

        // 한 쪽이 막혀있을 경우(백돌로) 
        private (int row, int col)? CheckFourSituation(GameManager.PlayerType[,] board, GameManager.PlayerType opponent)
        {
            // 수평 체크
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c <= BOARD_SIZE - 5; c++)
                {
                    int count = 0;
                    int emptyPos = -1;
                    for (int i = 0; i < 5; i++)
                    {
                        if (board[r, c + i] == opponent) count++;
                        else if (board[r, c + i] == GameManager.PlayerType.None) emptyPos = c + i;
                    }

                    if (count == 4 && emptyPos != -1)
                    {
                        if ((c == 0 || board[r, c - 1] != GameManager.PlayerType.None) ||
                            (c + 4 == BOARD_SIZE - 1 || board[r, c + 5] != GameManager.PlayerType.None))
                            return (r, emptyPos);
                    }
                }
            }

            // 수직 체크
            for (int c = 0; c < BOARD_SIZE; c++)
            {
                for (int r = 0; r <= BOARD_SIZE - 5; r++)
                {
                    int count = 0;
                    int emptyPos = -1;
                    for (int i = 0; i < 5; i++)
                    {
                        if (board[r + i, c] == opponent) count++;
                        else if (board[r + i, c] == GameManager.PlayerType.None) emptyPos = r + i;
                    }

                    if (count == 4 && emptyPos != -1)
                    {
                        if ((r == 0 || board[r - 1, c] != GameManager.PlayerType.None) ||
                            (r + 4 == BOARD_SIZE - 1 || board[r + 5, c] != GameManager.PlayerType.None))
                            return (emptyPos, c);
                    }
                }
            }

            // 대각선 (\) 체크
            for (int r = 0; r <= BOARD_SIZE - 5; r++)
            {
                for (int c = 0; c <= BOARD_SIZE - 5; c++)
                {
                    int count = 0;
                    int emptyR = -1, emptyC = -1;
                    for (int i = 0; i < 5; i++)
                    {
                        if (board[r + i, c + i] == opponent) count++;
                        else if (board[r + i, c + i] == GameManager.PlayerType.None)
                        {
                            emptyR = r + i;
                            emptyC = c + i;
                        }
                    }

                    if (count == 4 && emptyR != -1)
                    {
                        if ((r == 0 || c == 0 || board[r - 1, c - 1] != GameManager.PlayerType.None) ||
                            (r + 4 == BOARD_SIZE - 1 || c + 4 == BOARD_SIZE - 1 ||
                             board[r + 5, c + 5] != GameManager.PlayerType.None))
                            return (emptyR, emptyC);
                    }
                }
            }

            // 대각선 (/) 체크
            for (int r = 0; r <= BOARD_SIZE - 5; r++)
            {
                for (int c = 4; c < BOARD_SIZE; c++)
                {
                    int count = 0;
                    int emptyR = -1, emptyC = -1;
                    for (int i = 0; i < 5; i++)
                    {
                        if (board[r + i, c - i] == opponent) count++;
                        else if (board[r + i, c - i] == GameManager.PlayerType.None)
                        {
                            emptyR = r + i;
                            emptyC = c - i;
                        }
                    }

                    if (count == 4 && emptyR != -1)
                    {
                        if ((r == 0 || c == BOARD_SIZE - 1 || board[r - 1, c + 1] != GameManager.PlayerType.None) ||
                            (r + 4 == BOARD_SIZE - 1 || c - 4 == 0 ||
                             board[r + 5, c - 5] != GameManager.PlayerType.None))
                            return (emptyR, emptyC);
                    }
                }
            }

            return null;
        }
    }
}