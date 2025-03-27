using System.Collections.Generic;
using UnityEngine;

public class MCTS
{
    private static MCTS _instance;
    public static MCTS Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MCTS(new GameManager.PlayerType[15, 15]);
            }
            return _instance;
        }
    }
    
    private MCTSNode root;
    private GameManager.PlayerType[,] initialBoard;
    private const int BOARD_SIZE = 15;
    
    private MCTS(GameManager.PlayerType[,] board)
    {
        initialBoard = (GameManager.PlayerType[,])board.Clone();
        root = new MCTSNode(null, -1, -1, initialBoard, GameManager.PlayerType.PlayerB); // 첫 백돌
    }
    
    // 보드 업데이트 해줘야함 new를 해줘서 새로운 인스터스 생성해줬기에 프로퍼티로 설정한 값 들어가지 않음. 
    public void UpdateBoard(GameManager.PlayerType[,] board) 
    {
        initialBoard = (GameManager.PlayerType[,])board.Clone();
        root = new MCTSNode(null, -1, -1, initialBoard, GameManager.PlayerType.PlayerB);
    }

    #region 난이도 조절상수
    private int consecutiveFiveBlocks;
    public int ConsecutiveFiveBlocks 
    { 
        get => consecutiveFiveBlocks; 
        set => consecutiveFiveBlocks = value; 
    }

    private int fourBlocks;
    public int FourBlocks 
    { 
        get => fourBlocks; 
        set => fourBlocks = value; 
    }

    private int threeBlocks;
    public int ThreeBlocks 
    { 
        get => threeBlocks; 
        set => threeBlocks = value; 
    }

    private int defenseFourBlocks;
    public int DefenseFourBlocks 
    { 
        get => defenseFourBlocks; 
        set => defenseFourBlocks = value; 
    }

    private int defenseThreeBlocks;
    public int DefenseThreeBlocks 
    { 
        get => defenseThreeBlocks; 
        set => defenseThreeBlocks = value; 
    }

    private int placeAroundBlackBlock;
    public int PlaceAroundBlackBlock 
    { 
        get => placeAroundBlackBlock; 
        set => placeAroundBlackBlock = value; 
    }
    #endregion

    #region 난이도 설정 메서드
    public void SetBeginnerMode()
    {
        ConsecutiveFiveBlocks = 1000;  // 5연속 (승리)
        FourBlocks = 500;             // 4연속 (공격)
        ThreeBlocks = 300;            // 3연속 (공격)
        DefenseFourBlocks = 400;      // 상대 4연속 (방어)
        DefenseThreeBlocks = 300;      // 상대 3연속 (방어)
        PlaceAroundBlackBlock = 200;  // 흑돌 주변에 두기
    }

    public void SetIntermediateMode()
    {
        ConsecutiveFiveBlocks = 1000;
        FourBlocks = 600;
        ThreeBlocks = 500;
        DefenseFourBlocks = 300;
        DefenseThreeBlocks = 700;
        PlaceAroundBlackBlock = 500;
    }

    public void SetProMode()
    {
        ConsecutiveFiveBlocks = 2000;
        FourBlocks = 700;
        ThreeBlocks = 650;
        DefenseFourBlocks = 850;
        DefenseThreeBlocks = 1000;
        PlaceAroundBlackBlock = 500;
    }
    #endregion
    
    public (int row, int col) GetBestMove(int simulations)
    {
        var firstDefenseBlock = CheckFourSituation(initialBoard, GameManager.PlayerType.PlayerA);

        if (firstDefenseBlock.HasValue)
        {
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
    int maxMoves = 50; // 최대 시뮬레이션 수 제한

    while (!IsGameOver(board) && movesMade < maxMoves)
    {
        var possibleMoves = GetHeuristicMoves(board, currentPlayer, movesMade);
        if (possibleMoves.Count == 0) return 0.5;

        var (row, col, _) = possibleMoves[0]; // 최고 점수 수 선택
        board[row, col] = currentPlayer;
        currentPlayer = currentPlayer == GameManager.PlayerType.PlayerA
            ? GameManager.PlayerType.PlayerB
            : GameManager.PlayerType.PlayerA;
        movesMade++;

        // 빠른 종료 조건 추가
        if (HasFiveInARow(board, GameManager.PlayerType.PlayerB)) return 1;
        if (HasFiveInARow(board, GameManager.PlayerType.PlayerA)) return 0;
    }

    return 0.5; // 무승부로 간주
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
    
    private int HeuristicScore(GameManager.PlayerType[,] board, int row, int col, 
        GameManager.PlayerType currentPlayer, int movesMade)
    {
        int score = 0;
        var tempBoard = (GameManager.PlayerType[,])board.Clone();
        tempBoard[row, col] = currentPlayer;

        // AI가 5개 연속 만들기
        if (HasFiveInARow(tempBoard, GameManager.PlayerType.PlayerB))
            score += ConsecutiveFiveBlocks;

        // AI 자신의 연속 돌 만들기
        int aiCount = CountConsecutive(tempBoard, row, col, GameManager.PlayerType.PlayerB);
        if (aiCount == 4) score += FourBlocks;
        else if (aiCount == 3) score += ThreeBlocks;

        // 플레이어의 흑돌 3개 4개 막기
        int opponentCount = CountConsecutive(tempBoard, row, col, GameManager.PlayerType.PlayerA);
        if (opponentCount == 4) score += DefenseFourBlocks; // 4개 연속 막기
        else if (opponentCount == 3) score += DefenseThreeBlocks; // 3개 연속 막기

        // 흑돌 주변에 두기
        if (IsNearOpponent(tempBoard, row, col, GameManager.PlayerType.PlayerA))
            score += PlaceAroundBlackBlock;
        
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