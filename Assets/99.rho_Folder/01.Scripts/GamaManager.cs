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
                var checker = new ForbiddenRuleChecker(_board, currentMoveindex);
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