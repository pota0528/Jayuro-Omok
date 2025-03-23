using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private BlockController _blockController;
    [SerializeField] private GameUIController _gameUIController;
    private Canvas _canvas;

    public enum PlayerType { None, PlayerA, PlayerB, PlayerX }
    private PlayerType[,] _board;

    private enum TurnType { None, PlayerA, PlayerB }
    private TurnType currentTurnType = TurnType.None;
    private enum GameResult { None, Win, Lose, Draw }
    private int moveIndex = 0;
    private (int, int) currentMoveindex = (-1, -1);
    private Block currentBlackBlock;
    private const int LINE_COUNT = 15;
    private List<(int, int)> forbiddenCollecition = new List<(int, int)>();
    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        // _board 초기화
        _board = new PlayerType[LINE_COUNT, LINE_COUNT];

        // 블록 초기화
        _blockController.InitBlocks();

        // Game UI 초기화
        _gameUIController.SetGameUIMode(GameUIController.GameUIMode.Init);
        _board[7, 7] = PlayerType.PlayerA;
        currentMoveindex = (7, 7);
        _blockController.PlaceMarker(Block.MarkerType.Black, LINE_COUNT / 2, LINE_COUNT / 2, moveIndex);
        SetTurn(TurnType.PlayerB);
    }

    private void EndGame(GameResult gameResult)
    {
        // 게임오버 표시
        _gameUIController.SetGameUIMode(GameUIController.GameUIMode.GameOver);
        _blockController.OnBlockClickedDelegate = null;

        switch (gameResult)
        {
            case GameResult.Win:
                Debug.Log("Player A win");
                break;
            case GameResult.Lose:
                Debug.Log("Player B win");
                break;
            case GameResult.Draw:
                Debug.Log("Draw");
                break;
        }
    }

    private bool SetNewBoardValue(PlayerType playerType, int row, int col)
    {
        if (currentTurnType == TurnType.PlayerA) //A턴일때는 TurnType.PlayerA, TurnType.PlayerB일 경우 return
        {
            if (_board[row, col] != PlayerType.None)
            {
                return false;
            }
        }
        if (currentTurnType == TurnType.PlayerB) //A턴일때는 != PlayerType.None이면 return false;
        {
            if (_board[row, col] == PlayerType.PlayerA || _board[row, col] == PlayerType.PlayerB)
            {
                return false;
            }
        }

        //if (_board[row, col] != PlayerType.None) return false;

        if (playerType == PlayerType.PlayerA)
        {
            _board[row, col] = playerType;
            _blockController.PlaceMarker(Block.MarkerType.Black, row, col, moveIndex);
            return true;
        }
        else if (playerType == PlayerType.PlayerB)
        {
            _board[row, col] = playerType;
            _blockController.PlaceMarker(Block.MarkerType.White, row, col, moveIndex);
            return true;
        }
        return false;
    }

    private void SetTurn(TurnType turnType)
    {
        switch (turnType)
        {
            case TurnType.PlayerA:
                currentTurnType = TurnType.PlayerA;
                _gameUIController.SetGameUIMode(GameUIController.GameUIMode.TurnA);
                //최근에 놓인 흑돌을 기준으로, 가로 검사
                var checker = new ForbiddenRuleChecker(_board, LINE_COUNT, currentMoveindex);
                forbiddenCollecition = checker.GetForbiddenSpots();
                SetForbiddenMark(forbiddenCollecition);
                _blockController.OnBlockClickedDelegate = (row, col) =>
                {
                    ++moveIndex;
                    currentMoveindex = (row, col);
                    if (SetNewBoardValue(PlayerType.PlayerA, row, col))
                    {
                        var gameResult = CheckGameResult();
                        if (gameResult == GameResult.None)
                        {
                            SetTurn(TurnType.PlayerB);
                        }
                        else
                            EndGame(gameResult);
                    }
                    else
                    {
                        // TODO: 이미 있는 곳을 터치했을 때 처리
                    }
                };
                break;
            case TurnType.PlayerB:
                currentTurnType = TurnType.PlayerB;
                _gameUIController.SetGameUIMode(GameUIController.GameUIMode.TurnB);
                _blockController.OnBlockClickedDelegate = (row, col) =>
                {
                    ++moveIndex;
                    if (SetNewBoardValue(PlayerType.PlayerB, row, col))
                    {
                        Debug.Log("하하!");
                        var gameResult = CheckGameResult();
                        if (gameResult == GameResult.None)
                        {
                            SetTurn(TurnType.PlayerA); //그러므로 SetTurn 하면서 뭔가 오류가 나는거일수도?
                        }
                        else //게임이 끝나면 금수 자리를 터치안해도 안 멈춘다.
                        {
                            EndGame(gameResult);
                        }
                    }
                    else
                    {
                        // TODO: 이미 있는 곳을 터치했을 때 처리
                    }
                };

                break;
        }
    }

    #region 장목 검사할 때 쓰이는 함수
    /// <summary>
    /// 최근에 놓은 흑돌을 기준으로 양방향으로 4칸을 돌면서 빈 공백(금수의 가능성이 있는 좌표)을 담는 함수이다.
    /// 만약 순회할 때 백돌이 보이면 반대 방향으로 돌거나, 순회를 멈춘다.
    /// </summary>
    /// <returns></returns>
    private List<(int, int)> FindEmptySpotsInRow()
    {
        List<(int, int)> emptyList = new List<(int, int)>();
        int row = currentMoveindex.Item1;
        int col = currentMoveindex.Item2;

        int currentCol = col + 1; // 오른쪽 탐색

        while (0 <= currentCol && currentCol < col + 5 && currentCol <= 14)
        {
            if (_board[row, currentCol] == PlayerType.None)
            {
                emptyList.Add((row, currentCol));
            }
            else if (_board[row, currentCol] == PlayerType.PlayerB)
            {
                break;
            }
            ++currentCol;
        }

        currentCol = col - 1; // 왼쪽 탐색

        while (0 <= currentCol && currentCol > col - 5 && currentCol <= 14)
        {
            if (_board[row, currentCol] == PlayerType.None)
            {
                emptyList.Add((row, currentCol));
            }
            else if (_board[row, currentCol] == PlayerType.PlayerB)
            {
                break;
            }
            --currentCol;
        }

        //아래쪽 탐색
        int currentRow = row + 1;

        while (0 <= currentRow && currentRow < row + 5 && currentRow <= 14)
        {
            if (_board[currentRow, col] == PlayerType.None)
            {
                emptyList.Add((currentRow, col));
            }
            else if (_board[currentRow, col] == PlayerType.PlayerB)
            {
                break;
            }

            ++currentRow;
        }

        // 위쪽 탐색
        currentRow = row - 1;

        while (0 <= currentRow && currentRow > row - 5 && currentRow <= 14)
        {
            if (_board[currentRow, col] == PlayerType.None)
            {
                emptyList.Add((currentRow, col));
            }
            else if (_board[currentRow, col] == PlayerType.PlayerB)
            {
                break;
            }
            --currentRow;
        }

        // 오른쪽 아래 탐색

        currentRow = row + 1;
        currentCol = col + 1;

        while (currentRow <= 14 && currentCol <= 14 && currentRow < row + 5 && currentCol < col + 5)
        {
            if (_board[currentRow, currentCol] == PlayerType.None)
            {
                emptyList.Add((currentRow, currentCol));
            }
            else if (_board[currentRow, currentCol] == PlayerType.PlayerB)
            {
                break;
            }

            ++currentRow;
            ++currentCol;
        }

        // 왼쪽 위 탐색

        currentRow = row - 1;
        currentCol = col - 1;

        while (currentRow >= 0 && currentCol >= 0 && currentRow > row - 5 && currentCol > col - 5)
        {
            if (_board[currentRow, currentCol] == PlayerType.None)
            {
                emptyList.Add((currentRow, currentCol));
            }
            else if (_board[currentRow, currentCol] == PlayerType.PlayerB)
            {
                break;
            }
            --currentRow;
            --currentCol;
        }


        //↙↗ 탐색 구현

        // 왼쪽 아래 탐색
        currentRow = row + 1;
        currentCol = col - 1;

        while (currentRow <= 14 && currentCol >= 0 && currentRow < row + 5 && currentCol > col - 5)
        {
            if (_board[currentRow, currentCol] == PlayerType.None)
            {
                emptyList.Add((currentRow, currentCol));
            }
            else if (_board[currentRow, currentCol] == PlayerType.PlayerB)
            {
                break;
            }

            ++currentRow;
            --currentCol;
        }

        // 오른쪽 위 탐색
        currentRow = row - 1;
        currentCol = col + 1;

        while (currentRow >= 0 && currentCol <= 14 && currentRow > row - 5 && currentCol < col + 5)
        {
            if (_board[currentRow, currentCol] == PlayerType.None)
            {
                emptyList.Add((currentRow, currentCol));
            }
            else if (_board[currentRow, currentCol] == PlayerType.PlayerB)
            {
                break;
            }

            --currentRow;
            ++currentCol;
        }
        return emptyList;
    }

    /// <summary>
    /// 금수의 가능성이 있는 공백을 양방향으로 4칸 돌면서 흑돌이 5개 이상인지, 즉 금수가 되는 조건인지 검사하는 함수이다.
    /// 만약 순회할 때 백돌이 반대 방향으로 돌거나, 순회를 멈춘다.
    /// </summary>
    /// <returns></returns>
    private void SetOverlineForbidden(List<(int, int)> emptyList)
    {
        for (int i = 0; i < emptyList.Count; i++) //좌우 검사
        {
            // 오른쪽 검사
            int row = emptyList[i].Item1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
            int col = emptyList[i].Item2 + 1;

            int blockIndex = 0;

            for (int j = col; j <= 14 && j < col + 4; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                if (_board[row, j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            // 왼쪽 검사
            row = emptyList[i].Item1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
            col = emptyList[i].Item2 - 1;

            for (int j = col; 0 <= j && j > col - 4; --j) // + 조건 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                if (_board[row, j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            if (blockIndex >= 5)
            {
                forbiddenCollecition.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }

        for (int i = 0; i < emptyList.Count; i++) //상하 검사
        {
            // 위쪽 검사
            int row = emptyList[i].Item1 + 1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
            int col = emptyList[i].Item2;

            int blockIndex = 0;

            for (int j = row; j <= 14 && j < row + 4; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                if (_board[j, col] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            // 아래쪽 검사
            row = emptyList[i].Item1 - 1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
            col = emptyList[i].Item2;

            for (int j = row; 0 <= j && j > row - 4; --j) // + 조건 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                if (_board[j, col] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            if (blockIndex >= 5)
            {
                forbiddenCollecition.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }

        for (int i = 0; i < emptyList.Count; i++) // ↖↘
        {
            // ↘ 부터 검사! // TODO : 체크 확인

            int row = emptyList[i].Item1 + 1; //공백의 그 다음 자리부터 계산을 해야하니 - 1이 되어야한다.
            int col = emptyList[i].Item2 + 1;

            int blockIndex = 0; //1,2,3,4

            for (int j = 0; j < 4; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                if (row + j > 14 || col + j > 14)
                {
                    break;
                }

                if (_board[row + j, col + j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            // ↖ 검사! // TODO : 체크 확인

            row = emptyList[i].Item1 - 1; //공백의 그 다음 자리부터 계산을 해야하니 - 1이 되어야한다.
            col = emptyList[i].Item2 - 1;

            for (int j = 0; j < 4; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                if (row - j < 0 || col - j < 0)
                {
                    break;
                }

                if (_board[row - j, col - j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            if (blockIndex >= 5)
            {
                forbiddenCollecition.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }

        for (int i = 0; i < emptyList.Count; i++) // ↙↗
        {
            // ↙(왼쪽 아래) 검사
            int row = emptyList[i].Item1 + 1; // 공백의 그 다음 자리부터 계산해야 하니 +1
            int col = emptyList[i].Item2 - 1;

            int blockIndex = 0; // 1,2,3,4

            for (int j = 0; j < 4; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야 한다.
            {
                if (row + j > 14 || col - j < 0) // 왼쪽 아래 방향 범위 초과 검사
                {
                    break;
                }

                if (_board[row + j, col - j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            // ↗(오른쪽 위) 검사
            row = emptyList[i].Item1 - 1; // 공백의 그 다음 자리부터 계산해야 하니 -1
            col = emptyList[i].Item2 + 1;

            for (int j = 0; j < 4; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야 한다.
            {
                if (row - j < 0 || col + j > 14) // 오른쪽 위 방향 범위 초과 검사
                {
                    break;
                }

                if (_board[row - j, col + j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            if (blockIndex >= 5)
            {
                forbiddenCollecition.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }
    }
    #endregion

    private List<(int, int)> FindEmpty4X4()
    {
        List<(int, int)> emptyList = new List<(int, int)>();
        int row = currentMoveindex.Item1;
        int col = currentMoveindex.Item2;

        int currentCol = col + 1; // 오른쪽 탐색

        while (0 <= currentCol && currentCol < col + 5 && currentCol <= 14)
        {
            if (_board[row, currentCol] == PlayerType.None)
            {
                emptyList.Add((row, currentCol));
            }
            else if (_board[row, currentCol] == PlayerType.PlayerB)
            {
                break;
            }

            ++currentCol;
        }

        currentCol = col - 1; // 왼쪽 탐색

        while (0 <= currentCol && currentCol > col - 5 && currentCol <= 14)
        {
            if (_board[row, currentCol] == PlayerType.None)
            {
                emptyList.Add((row, currentCol));
            }
            else if (_board[row, currentCol] == PlayerType.PlayerB)
            {
                break;
            }
            --currentCol;
        }

        //아래쪽 탐색
        int currentRow = row + 1;

        while (0 <= currentRow && currentRow < row + 5 && currentRow <= 14)
        {
            if (_board[currentRow, col] == PlayerType.None)
            {
                emptyList.Add((currentRow, col));
            }
            else if (_board[currentRow, col] == PlayerType.PlayerB)
            {
                break;
            }

            ++currentRow;
        }

        // 위쪽 탐색
        currentRow = row - 1;

        while (0 <= currentRow && currentRow > row - 5 && currentRow <= 14)
        {
            if (_board[currentRow, col] == PlayerType.None)
            {
                emptyList.Add((currentRow, col));
            }
            else if (_board[currentRow, col] == PlayerType.PlayerB)
            {
                break;
            }
            --currentRow;
        }

        // 오른쪽 아래 탐색

        currentRow = row + 1;
        currentCol = col + 1;

        while (currentRow <= 14 && currentCol <= 14 && currentRow < row + 5 && currentCol < col + 5)
        {
            if (_board[currentRow, currentCol] == PlayerType.None)
            {
                emptyList.Add((currentRow, currentCol));
            }
            else if (_board[currentRow, currentCol] == PlayerType.PlayerB)
            {
                break;
            }

            ++currentRow;
            ++currentCol;
        }

        // 왼쪽 위 탐색

        currentRow = row - 1;
        currentCol = col - 1;

        while (currentRow >= 0 && currentCol >= 0 && currentRow > row - 5 && currentCol > col - 5)
        {
            if (_board[currentRow, currentCol] == PlayerType.None)
            {
                emptyList.Add((currentRow, currentCol));
            }
            else if (_board[currentRow, currentCol] == PlayerType.PlayerB)
            {
                break;
            }
            --currentRow;
            --currentCol;
        }


        //↙↗ 탐색 구현

        // 왼쪽 아래 탐색
        currentRow = row + 1;
        currentCol = col - 1;

        while (currentRow <= 14 && currentCol >= 0 && currentRow < row + 5 && currentCol > col - 5)
        {
            if (_board[currentRow, currentCol] == PlayerType.None)
            {
                emptyList.Add((currentRow, currentCol));
            }
            else if (_board[currentRow, currentCol] == PlayerType.PlayerB)
            {
                break;
            }

            ++currentRow;
            --currentCol;
        }

        // 오른쪽 위 탐색
        currentRow = row - 1;
        currentCol = col + 1;

        while (currentRow >= 0 && currentCol <= 14 && currentRow > row - 5 && currentCol < col + 5)
        {
            if (_board[currentRow, currentCol] == PlayerType.None)
            {
                emptyList.Add((currentRow, currentCol));
            }
            else if (_board[currentRow, currentCol] == PlayerType.PlayerB)
            {
                break;
            }

            --currentRow;
            ++currentCol;
        }
        return emptyList;
    }

    private void Set4X4Forbidden(List<(int, int)> emptyList)
    {
        //서로 다른 방향으로 4x4 금수일 때
        for (int i = 0; i < emptyList.Count; i++)
        {
            const int MAX_TURNING_COUNT = 5; //한 줄 당 최대 공백 3칸까지 제한
            const int MAX_VOID_COUNT = 3;

            int tempForbiddenCount = 0;

            // 오른쪽 검사
            int row = emptyList[i].Item1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
            int col = emptyList[i].Item2 + 1;

            int blockIndex = 1;
            int turningCount = 0; //공백 맞닿뜨리는 곳까지 합쳐서 turningCount를 계산해야한다. 현재 공백 좌표까지 합쳐서 turningCount가 포함됨
            int voidCount = 0;

            for (int j = col; j <= 14 && j < col + 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                if (_board[row, j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else if (_board[row, j] == PlayerType.PlayerB)
                {
                    ++turningCount;
                    break;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            // 왼쪽 검사
            row = emptyList[i].Item1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
            col = emptyList[i].Item2 - 1;
            voidCount = 0;

            for (int j = col; 0 <= j && j > col - 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; --j) // + 조건 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                if (_board[row, j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else if (_board[row, j] == PlayerType.PlayerB)
                {
                    ++turningCount;
                    break;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            if (blockIndex == 4)
            {
                ++tempForbiddenCount;
            }

            // 위쪽 검사
            row = emptyList[i].Item1 + 1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
            col = emptyList[i].Item2;

            blockIndex = 1;
            turningCount = 0;
            voidCount = 0;

            for (int j = row; j <= 14 && j < row + 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                if (_board[j, col] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else if (_board[j, col] == PlayerType.PlayerB)
                {
                    ++turningCount;
                    break;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            // 아래쪽 검사
            row = emptyList[i].Item1 - 1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
            col = emptyList[i].Item2;
            voidCount = 0;

            for (int j = row; 0 <= j && j > row - 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; --j) // + 조건 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                if (_board[j, col] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else if (_board[j, col] == PlayerType.PlayerB)
                {
                    ++turningCount;
                    break;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            if (blockIndex == 4)
            {
                ++tempForbiddenCount;
            }

            // ↘ 부터 검사! // TODO : 체크 확인
            row = emptyList[i].Item1 + 1; //공백의 그 다음 자리부터 계산을 해야하니 - 1이 되어야한다.
            col = emptyList[i].Item2 + 1;

            blockIndex = 1; //1,2,3,4
            turningCount = 0;
            voidCount = 0;

            for (int j = 0; j < 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                if (row + j > 14 || col + j > 14)
                {
                    break;
                }

                if (_board[row + j, col + j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else if (_board[row + j, col + j] == PlayerType.PlayerB)
                {
                    ++turningCount;
                    break;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            // ↖ 검사! // TODO : 체크 확인

            row = emptyList[i].Item1 - 1; //공백의 그 다음 자리부터 계산을 해야하니 - 1이 되어야한다.
            col = emptyList[i].Item2 - 1;
            voidCount = 0;

            for (int j = 0; j < 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                if (row - j < 0 || col - j < 0)
                {
                    break;
                }

                if (_board[row - j, col - j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else if (_board[row - j, col - j] == PlayerType.PlayerB)
                {
                    ++turningCount;
                    break;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            if (blockIndex == 4)
            {
                ++tempForbiddenCount;
            }

            // ↙(왼쪽 아래) 검사
            row = emptyList[i].Item1 + 1; // 공백의 그 다음 자리부터 계산해야 하니 +1
            col = emptyList[i].Item2 - 1;

            blockIndex = 1; // 1,2,3,4
            turningCount = 0;
            voidCount = 0;

            for (int j = 0; j < 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야 한다.
            {
                if (row + j > 14 || col - j < 0) // 왼쪽 아래 방향 범위 초과 검사
                {
                    break;
                }

                if (_board[row + j, col - j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else if (_board[row + j, col - j] == PlayerType.PlayerB)
                {
                    ++turningCount;
                    break;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            // ↗(오른쪽 위) 검사
            row = emptyList[i].Item1 - 1; // 공백의 그 다음 자리부터 계산해야 하니 -1
            col = emptyList[i].Item2 + 1;
            voidCount = 0;

            for (int j = 0; j < 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야 한다.
            {
                if (row - j < 0 || col + j > 14) // 오른쪽 위 방향 범위 초과 검사
                {
                    break;
                }

                if (_board[row - j, col + j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else if (_board[row - j, col + j] == PlayerType.PlayerB)
                {
                    ++turningCount;
                    break;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            if (blockIndex == 4)
            {
                ++tempForbiddenCount;
            }

            if (tempForbiddenCount >= 2)
            {
                forbiddenCollecition.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }

        for (int i = 0; i < emptyList.Count; i++)
        {
            const int MAX_TURN_COUNT = 5; //한쪽 방향의 공백
            int tempForbiddenCount = 0;
            // 오른쪽 검사
            int row = emptyList[i].Item1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
            int col = emptyList[i].Item2;

            int turnCount = 0;
            string tempPattern = "";

            for (int j = col; 0 <= j && j <= 14 && turnCount < MAX_TURN_COUNT; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                ++turnCount;

                if (_board[row, j] == PlayerType.PlayerA || j == emptyList[i].Item2)
                {
                    tempPattern += "A";
                }
                else if (_board[row, j] == PlayerType.PlayerB)
                {
                    return;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    tempPattern += ".";
                }
            }

            switch (tempPattern)
            {
                case "A.AAA":
                case "AA.AA":
                case "AAA.A":
                    ++tempForbiddenCount;
                    break;// 한 칸 띄운 4
            }

            row = emptyList[i].Item1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
            col = emptyList[i].Item2 - 1;

            turnCount = 0;
            tempPattern = "";

            for (int j = col; 0 <= j && j <= 14 && turnCount < MAX_TURN_COUNT; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                ++turnCount;

                if (_board[row, j] == PlayerType.PlayerA || j == emptyList[i].Item2)
                {
                    tempPattern += "A";
                }
                else if (_board[row, j] == PlayerType.PlayerB)
                {
                    return;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    tempPattern += ".";
                }
            }

            switch (tempPattern)
            {
                case "A.AAA":
                case "AA.AA":
                case "AAA.A":
                    ++tempForbiddenCount;
                    break;// 한 칸 띄운 4
            }

            row = emptyList[i].Item1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
            col = emptyList[i].Item2 - 2;

            turnCount = 0;
            tempPattern = "";

            for (int j = col; 0 <= j && j <= 14 && turnCount < MAX_TURN_COUNT; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                ++turnCount;

                if (_board[row, j] == PlayerType.PlayerA || j == emptyList[i].Item2)
                {
                    tempPattern += "A";
                }
                else if (_board[row, j] == PlayerType.PlayerB)
                {
                    return;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    tempPattern += ".";
                }
            }

            switch (tempPattern)
            {
                case "A.AAA":
                case "AA.AA":
                case "AAA.A":
                    ++tempForbiddenCount;
                    break;// 한 칸 띄운 4
            }

            row = emptyList[i].Item1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
            col = emptyList[i].Item2 - 3;

            turnCount = 0;
            tempPattern = "";

            for (int j = col; 0 <= j && j <= 14 && turnCount < MAX_TURN_COUNT; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                ++turnCount;

                if (_board[row, j] == PlayerType.PlayerA || j == emptyList[i].Item2)
                {
                    tempPattern += "A";
                }
                else if (_board[row, j] == PlayerType.PlayerB)
                {
                    return;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    tempPattern += ".";
                }
            }

            switch (tempPattern)
            {
                case "A.AAA":
                case "AA.AA":
                case "AAA.A":
                    ++tempForbiddenCount;
                    break;// 한 칸 띄운 4
            }

            row = emptyList[i].Item1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
            col = emptyList[i].Item2 - 4;

            turnCount = 0;
            tempPattern = "";

            for (int j = col; 0 <= j && j <= 14 && turnCount < MAX_TURN_COUNT; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
            {
                ++turnCount;

                if (_board[row, j] == PlayerType.PlayerA || j == emptyList[i].Item2)
                {
                    tempPattern += "A";
                }
                else if (_board[row, j] == PlayerType.PlayerB)
                {
                    return;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    tempPattern += ".";
                }
            }

            switch (tempPattern)
            {
                case "A.AAA":
                case "AA.AA":
                case "AAA.A":
                    ++tempForbiddenCount;
                    break;// 한 칸 띄운 4
            }

            if (tempForbiddenCount > 1)
            {
                forbiddenCollecition.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }
    }
    private void SetForbiddenMark(List<(int, int)> forbiddenList)
    {
        for (int i = 0; i < forbiddenList.Count; i++)
        {
            _board[forbiddenList[i].Item1, forbiddenList[i].Item2] = PlayerType.PlayerX;
            _blockController.PlaceMarker(Block.MarkerType.Forbidden, forbiddenList[i].Item1, forbiddenList[i].Item2, moveIndex);
        }
    }
    //금수 마크 표시하는 함수
    /// <summary>
    /// 게임 결과 확인 함수
    /// </summary>
    /// <returns>플레이어 기준 게임 결과</returns>
    private GameResult CheckGameResult()
    {
        if (CheckGameWin(PlayerType.PlayerA)) { return GameResult.Win; }
        if (CheckGameWin(PlayerType.PlayerB)) { return GameResult.Lose; }
        //if (MinimaxAIController.IsAllBlocksPlaced(_board)) { return GameResult.Draw; }

        return GameResult.None;
    }

    //게임의 승패를 판단하는 함수
    private bool CheckGameWin(PlayerType playerType)
    {
        // 가로로 마커가 일치하는지 확인
        for (var row = 0; row < _board.GetLength(0); row++)
        {
            for (var col = 0; col < _board.GetLength(1) - 5 + 1; col++)
            {
                if (_board[row, col] == playerType && _board[row, col + 1] == playerType &&
                    _board[row, col + 2] == playerType && _board[row, col + 3] == playerType &&
                    _board[row, col + 4] == playerType)
                {
                    Debug.Log("게임 끝!");
                    return true;
                }
            }
        }

        // 세로로 마커가 일치하는지 확인
        for (var col = 0; col < _board.GetLength(0); col++)
        {
            for (var row = 0; row < _board.GetLength(1) - 5 + 1; row++)
            {
                if (_board[row, col] == playerType && _board[row + 1, col] == playerType &&
                    _board[row + 2, col] == playerType && _board[row + 3, col] == playerType &&
                    _board[row + 4, col] == playerType)
                {
                    Debug.Log("게임 끝!");
                    return true;
                }
            }
        }

        // \ 대각선 마커 일치하는지 확인
        for (var row = 0; row < _board.GetLength(0) - 5 + 1; row++)
        {
            for (var col = 0; col < _board.GetLength(1) - 5 + 1; col++)
            {
                if (_board[row, col] == playerType && _board[row + 1, col + 1] == playerType &&
                    _board[row + 2, col + 2] == playerType && _board[row + 3, col + 3] == playerType &&
                    _board[row + 4, col + 4] == playerType)
                {
                    Debug.Log("게임 끝!");
                    return true;
                }
            }
        }

        // / 대각선 마커 일치하는지 확인
        for (var row = 0; row < _board.GetLength(0) - 5 + 1; row++)
        {
            for (var col = 0; col < _board.GetLength(1) - 5 + 1; col++)
            {
                if (_board[row, col + 4] == playerType && _board[row + 1, col + 3] == playerType &&
                    _board[row + 2, col + 2] == playerType && _board[row + 3, col + 1] == playerType &&
                    _board[row + 4, col] == playerType)
                {
                    Debug.Log("게임 끝!");
                    return true;
                }
            }
        }
        return false;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // if (scene.name == "Game")
        // {
        //     _blockController = GameObject.FindObjectOfType<BlockController>();
        //     _gameUIController = GameObject.FindObjectOfType<GameUIController>();
        //
        //     // 게임 시작
        //     StartGame();
        // }
        //
        // _canvas = GameObject.FindObjectOfType<Canvas>();
    }
}