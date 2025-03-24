using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject signUpPanel;
    [SerializeField] private GameObject userPanel;
    [SerializeField] private GameObject profilePanel;
    [SerializeField] private BlockController _blockController;
    [SerializeField] private GameUIController _gameUIController;
    [SerializeField] private Button confirmButton;

    public enum PlayerType { None, PlayerA, PlayerB }
    private enum TurnType { PlayerA, PlayerB }
    private enum GameResult { None, Win, Lose, Draw }

    private PlayerType[,] _board; // 게임 상태 저장
    private TurnType currentTurn;
    private Canvas _canvas;
    private DBManager mongoDBManager;
    private int currentImageIndex = 0;
    private List<Move> moves = new List<Move>(); // 매치 기록용 리스트 추가
    
    private void Start()
    {
        StartGame();
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _canvas = GameObject.FindObjectOfType<Canvas>();
        OpenLoginPanel();
        mongoDBManager = FindObjectOfType<DBManager>();
    }

    private void StartGame()
    {
        _board = new PlayerType[15, 15];
        _blockController.InitBlocks();
        _gameUIController.SetGameUIMode(GameUIController.GameUIMode.Init);
        SetTurn(TurnType.PlayerA);
        moves.Clear(); // 새 게임 시작 시 moves 초기화
    }

    private void EndGame(GameResult gameResult)
    {
        _gameUIController.SetGameUIMode(GameUIController.GameUIMode.GameOver);
        _blockController.OnBlockClickedDelegate = null;

        switch (gameResult)
        {
            case GameResult.Win:
                Debug.Log("PlayerA win");
                SaveMatch("PlayerA"); // 매치 저장 (nickname은 예시)
                break;
            case GameResult.Lose:
                Debug.Log("AI win");
                SaveMatch("AI"); // 매치 저장 (nickname은 예시)
                break;
            case GameResult.Draw:
                Debug.Log("Draw");
                break;
        }
    }

    private bool SetNewBoardValue(PlayerType playerType, int row, int col)
    {
        if (_board[row, col] != PlayerType.None) return false;
        _board[row, col] = playerType;
        _blockController.PlaceMarker(playerType == PlayerType.PlayerA ? Block.MarkerType.Black : Block.MarkerType.White, row, col);

        // moves에 기록 추가
        string color = playerType == PlayerType.PlayerA ? "흑돌" : "백돌";
        moves.Add(new Move { row = row, col = col, color = color });
        return true;
    }

    private void SetTurn(TurnType turnType)
    {
        currentTurn = turnType;
        switch (turnType)
        {
            case TurnType.PlayerA:
                _gameUIController.SetGameUIMode(GameUIController.GameUIMode.TurnA);
                _blockController.OnBlockClickedDelegate = OnBlockClicked;
                break;
            case TurnType.PlayerB:
                _gameUIController.SetGameUIMode(GameUIController.GameUIMode.TurnB);
                _blockController.OnBlockClickedDelegate = null;
                StartCoroutine(AIMove());
                break;
        }
    }

    private void OnBlockClicked(int row, int col)
    {
        if (currentTurn == TurnType.PlayerA && _board[row, col] == PlayerType.None)
        {
            _gameUIController.UpdateSelectedPosition(row, col);
            _blockController.SetPreviewMarker(row, col, true);
        }
    }

    public void OnClickedPlaceConfirmButton()
    {
        var (row, col) = _gameUIController.GetSelectedPosition();
        if (currentTurn == TurnType.PlayerA && row != -1 && col != -1 && SetNewBoardValue(PlayerType.PlayerA, row, col))
        {
            var gameResult = CheckGameResult();
            if (gameResult == GameResult.None)
                SetTurn(TurnType.PlayerB);
            else
                EndGame(gameResult);
        }
    }

    private IEnumerator AIMove()
    {
        MCTS mcts = new MCTS(_board);
        var (row, col) = mcts.GetBestMove(500);
        yield return new WaitForSeconds(1f);

        if (SetNewBoardValue(PlayerType.PlayerB, row, col))
        {
            var gameResult = CheckGameResult();
            if (gameResult == GameResult.None)
                SetTurn(TurnType.PlayerA);
            else
                EndGame(gameResult);
        }
    }

    private GameResult CheckGameResult()
    {
        if (CheckGameWin(PlayerType.PlayerA)) return GameResult.Win;
        if (CheckGameWin(PlayerType.PlayerB)) return GameResult.Lose;
        if (GetPossibleMoves(_board).Count == 0) return GameResult.Draw;
        return GameResult.None;
    }

    private bool CheckGameWin(PlayerType playerType)
    {
        int size = 15;
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size - 4; col++)
            {
                if (_board[row, col] == playerType && _board[row, col + 1] == playerType &&
                    _board[row, col + 2] == playerType && _board[row, col + 3] == playerType &&
                    _board[row, col + 4] == playerType)
                    return true;
            }
        }

        for (int col = 0; col < size; col++)
        {
            for (int row = 0; row < size - 4; row++)
            {
                if (_board[row, col] == playerType && _board[row + 1, col] == playerType &&
                    _board[row + 2, col] == playerType && _board[row + 3, col] == playerType &&
                    _board[row + 4, col] == playerType)
                    return true;
            }
        }

        for (int row = 0; row < size - 4; row++)
        {
            for (int col = 0; col < size - 4; col++)
            {
                if (_board[row, col] == playerType && _board[row + 1, col + 1] == playerType &&
                    _board[row + 2, col + 2] == playerType && _board[row + 3, col + 3] == playerType &&
                    _board[row + 4, col + 4] == playerType)
                    return true;
            }
        }

        for (int row = 0; row < size - 4; row++)
        {
            for (int col = 4; col < size; col++)
            {
                if (_board[row, col] == playerType && _board[row + 1, col - 1] == playerType &&
                    _board[row + 2, col - 2] == playerType && _board[row + 3, col - 3] == playerType &&
                    _board[row + 4, col - 4] == playerType)
                    return true;
            }
        }

        return false;
    }

    private List<(int row, int col)> GetPossibleMoves(PlayerType[,] board)
    {
        List<(int row, int col)> possibleMoves = new List<(int row, int col)>();
        for (int r = 0; r < 15; r++)
        {
            for (int c = 0; c < 15; c++)
            {
                if (board[r, c] == PlayerType.None)
                    possibleMoves.Add((r, c));
            }
        }
        return possibleMoves;
    }

    // 매치 저장 메서드 추가
    private void SaveMatch(string nickname)
    {
        MatchSaver saver = FindObjectOfType<MatchSaver>();
        if (saver != null)
        {
            saver.SaveMatch(nickname, moves);
        }
        else
        {
            Debug.LogError("MatchSaver 컴포넌트가 없습니다.");
        }
    }

    // UI 관련 메서드
    public void OpenLoginPanel()
    {
        if (_canvas != null)
            Instantiate(loginPanel, _canvas.transform);
    }

    public void OpenSignUpPanel()
    {
        if (_canvas != null)
            Instantiate(signUpPanel, _canvas.transform);
    }

    public void OpenUserPanel()
    {
        if (_canvas != null)
            Instantiate(userPanel, _canvas.transform);
    }

    public void OpenProfilePanel()
    {
        if (_canvas != null)
            Instantiate(profilePanel, _canvas.transform);
    }
}