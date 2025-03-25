using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private BlockController _blockController;
    [SerializeField] private GameUIController _gameUIController;
    [SerializeField] private Button confirmButton;
   
    public enum PlayerType { None, PlayerA, PlayerB }
    private PlayerType[,] _board;
    private enum TurnType { PlayerA, PlayerB }
    private TurnType currentTurn;
    private enum GameResult { None, Win, Lose, Draw }
    private List<Move> moves = new List<Move>();
    private (int, int) currentMoveIndex;
    private List<(int, int)> forbiddenCollection = new List<(int, int)>();
    private DBManager mongoDBManager;
    // 캔버스 참조
    private Canvas _canvas;
    
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // GameManager가 씬 전환 시 파괴되지 않도록 설정
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 구독
    }
    
    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        _board = new PlayerType[15, 15];
        _blockController.InitBlocks();
        _gameUIController.SetGameUIMode(GameUIController.GameUIMode.Init);
        SetTurn(TurnType.PlayerA);
        moves.Clear();
    }

    private void EndGame(GameResult gameResult)
    {
        _gameUIController.SetGameUIMode(GameUIController.GameUIMode.GameOver);
        _blockController.OnBlockClickedDelegate = null;

        switch (gameResult)
        {
            case GameResult.Win:
                Debug.Log("PlayerA win");
                SaveMatch("PlayerA");
                break;
            case GameResult.Lose:
                Debug.Log("AI win");
                SaveMatch("AI");
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
        Block.MarkerType markerType = playerType == PlayerType.PlayerA ? Block.MarkerType.Black : Block.MarkerType.White;
        _blockController.PlaceMarker(markerType, row, col);
        moves.Add(new Move { row = row, col = col, color = playerType == PlayerType.PlayerA ? "흑돌" : "백돌" });
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
                var checker = new ForbiddenRuleChecker(_board, currentMoveIndex);
                forbiddenCollection = checker.GetForbiddenSpots();
                SetForbiddenMarks(forbiddenCollection);
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
        if (currentTurn == TurnType.PlayerA && _board[row, col] == PlayerType.None && !forbiddenCollection.Contains((row, col)))
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
            currentMoveIndex = (row, col);
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
            currentMoveIndex = (row, col);
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

    private List<(int, int)> GetPossibleMoves(PlayerType[,] board)
    {
        List<(int, int)> possibleMoves = new List<(int, int)>();
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

    private void SetForbiddenMarks(List<(int, int)> forbiddenList)
    {
        foreach (var (row, col) in forbiddenList)
        {
            _blockController.PlaceMarker(Block.MarkerType.Forbidden, row, col);
        }
    }

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
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       
    }

   }