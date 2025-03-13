using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private BlockController _blockController;
    [SerializeField] private GameUIController _gameUIController;
    private Canvas _canvas;
    
    public enum PlayerType { None, PlayerA, PlayerB }
    private PlayerType[,] _board;
    
    private enum TurnType { PlayerA, PlayerB }

    private enum GameResult
    {
        None,   // 게임 진행 중
        Win,    // 플레이어 승
        Lose,   // 플레이어 패
        Draw    // 비김
    }

    void Start()
    {
        StartGame();
    }
    
    private void StartGame()
    {
        // _board 초기화
        _board = new PlayerType[15, 15];
        
        // 블록 초기화
        _blockController.InitBlocks();
        
        // Game UI 초기화
        _gameUIController.SetGameUIMode(GameUIController.GameUIMode.Init);
        
        // 턴 시작
        SetTurn(TurnType.PlayerA);
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
        if (_board[row, col] != PlayerType.None) return false;
        
        if (playerType == PlayerType.PlayerA)
        {
            _board[row, col] = playerType;
            _blockController.PlaceMarker(Block.MarkerType.Black, row, col);
            return true;
        }
        else if (playerType == PlayerType.PlayerB)
        {
            _board[row, col] = playerType;
            _blockController.PlaceMarker(Block.MarkerType.White, row, col);
            return true;
        }
        return false;
    }

    private void SetTurn(TurnType turnType)
    {
        switch (turnType)
        {
            case TurnType.PlayerA:
                _gameUIController.SetGameUIMode(GameUIController.GameUIMode.TurnA);
                _blockController.OnBlockClickedDelegate = (row, col) =>
                {
                    if (SetNewBoardValue(PlayerType.PlayerA, row, col))
                    {
                        var gameResult = CheckGameResult();
                        if (gameResult == GameResult.None)
                            SetTurn(TurnType.PlayerB);
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
                _gameUIController.SetGameUIMode(GameUIController.GameUIMode.TurnB);
                _blockController.OnBlockClickedDelegate = (row, col) =>
                {
                    if (SetNewBoardValue(PlayerType.PlayerB, row, col))
                    {
                        var gameResult = CheckGameResult();
                        if (gameResult == GameResult.None)
                            SetTurn(TurnType.PlayerA);
                        else
                            EndGame(gameResult);
                    }
                    else
                    {
                        // TODO: 이미 있는 곳을 터치했을 때 처리
                    }
                };

                break;
        }

    }

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