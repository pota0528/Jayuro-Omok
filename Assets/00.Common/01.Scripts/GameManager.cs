using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using DG.Tweening;
using static GameManager;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BlockController _blockController;
    [SerializeField] private GameUIController _gameUIController;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Timer _timer;


    // UI 패널 프리팹 (인스펙터에서 설정)

    public enum PlayerType
    {
        None,
        PlayerA,
        PlayerB,
        PlayerX
    }

    private PlayerType[,] _board;

    public enum TurnType
    {
        PlayerA,
        PlayerB
    }

    private TurnType currentTurn;

    public enum GameResult //자현 추가, private > public 으로 변경
    {
        None,
        Win,
        Lose,
        Draw
    }

    private List<Move> moves = new List<Move>();
    private (int, int) currentMoveIndex;
    private List<(int, int)> forbiddenCollection = new List<(int, int)>();

    private DBManager mongoDBManager;
    private MCTS _mcts;
    // 캔버스 참조
    private Canvas _canvas;
    private PlayerData playerData;
    private Block[] _blocks;

    private void Start()
    {
        playerData = UserSessionManager.Instance.GetPlayerData();
        _gameUIController.DisplayUserInfo(playerData.nickname, playerData.level.ToString(), playerData.imageIndex);
        _gameUIController.DisplayAIInfo();
        _timer.InitTimer();
        StartGame();
    }

    private void StartGame()
    {
        BeforeSetting();
        MCTS.Instance.UpdateBoard(_board); // 초기 보드 설정

        if (playerData.level >= 12 && playerData.level <= 18) // 12 ~ 18 까지임
        {
            Debug.Log("하수 진입");
            MCTS.Instance.SetBeginnerMode();
        }
        else if (playerData.level >= 5 && playerData.level < 12) // 5 ~ 11 까지임
        {
            Debug.Log("중수 진입");
            MCTS.Instance.SetIntermediateMode();
        }
        else if (playerData.level < 5) // 1 ~ 4 까지임
        {
            Debug.Log("고수 진입");
            MCTS.Instance.SetProMode();
        }
    }

    private void BeforeSetting()
    {
        _board = new PlayerType[15, 15];
        _blockController.InitBlocks();
        _gameUIController.SetGameUIMode(GameUIController.GameUIMode.Init);
        SetTurn(TurnType.PlayerA);
        moves.Clear();
    }

    private bool SetNewBoardValue(PlayerType playerType, int row, int col)
    {
        if (_board[row, col] != PlayerType.None) return false;
        _board[row, col] = playerType;
        Block.MarkerType markerType =
            playerType == PlayerType.PlayerA ? Block.MarkerType.Black : Block.MarkerType.White;
        _blockController.PlaceMarker(markerType, row, col);
        moves.Add(new Move { row = row, col = col, color = playerType == PlayerType.PlayerA ? "흑돌" : "백돌" });
        return true;
    }

    private async void SetTurn(TurnType turnType)
    {
        currentTurn = turnType;
        _timer.StartTimer();

        if (currentTurn == TurnType.PlayerA)
        {
            _timer.OnTimeout = () =>
            {
                _timer.PauseTimer();
                _blockController.DisableAllBlockInteractions();
                _blockController.OnBlockClickedDelegate = null;
                UIManager.Instance.OpenWinLosePanel(GameResult.Lose);
            };
        }
        else if (currentTurn == TurnType.PlayerB)
        {
            _timer.OnTimeout = () =>
            {
                _timer.PauseTimer();
                _blockController.DisableAllBlockInteractions();
                _blockController.OnBlockClickedDelegate = null;
                UIManager.Instance.OpenWinLosePanel(GameResult.Win);
            };
        }

        switch (turnType)
        {
            case TurnType.PlayerA:
                _timer.ChangeTurnResetTimer();
                _gameUIController.SetGameUIMode(GameUIController.GameUIMode.TurnA);
                _blockController.OnBlockClickedDelegate = OnBlockClicked;
                SetForbiddenMarks(forbiddenCollection, false); //기존 금수들 마크 해제
                var checker = new ForbiddenRuleChecker(_board);
                forbiddenCollection = checker.CheckForbiddenRelease(forbiddenCollection); // 기존 금수들 값 고정 및 해제
                SetForbiddenMarks(forbiddenCollection, true); //기존 금수들 마크 설정
                List<(int, int)> _forbiddenCollection = checker.GetForbiddenSpots(currentMoveIndex); //배열은 참조형이므로 배열을 이어쓰면 안된다.
                SetForbiddenMarks(_forbiddenCollection, true);
                forbiddenCollection.AddRange(_forbiddenCollection);
                forbiddenCollection = forbiddenCollection.Distinct().ToList();
                _blockController.UpdateRecentMoveDisplay(TurnType.PlayerA);
                break;
            case TurnType.PlayerB:
                _timer.ChangeTurnResetTimer();
                _gameUIController.SetGameUIMode(GameUIController.GameUIMode.TurnB);
                _blockController.OnBlockClickedDelegate = null;
                await AIMoveAsync(); // 비동기 AI 연산 호출
                AudioManager.Instance.OnPutStone();  // 백돌 놓는 효과음 추가
                break;
        }
    }

    private async Task AIMoveAsync()
    {
        // AI 연산을 별도 스레드에서 실행
        MCTS.Instance.UpdateBoard(_board);
        var (row, col) = await Task.Run(() => MCTS.Instance.GetBestMove(50));
        await Task.Delay(500); // 1초 대기 (메인 스레드에서 실행)

        if (SetNewBoardValue(PlayerType.PlayerB, row, col))
        {
            var _tempCurrentMoveIndex = (row, col);
            forbiddenCollection.Remove(_tempCurrentMoveIndex);
            var gameResult = CheckGameResult();
            if (gameResult == GameResult.None)
                SetTurn(TurnType.PlayerA);
            else
            {
                _timer.PauseTimer();
                _blockController.DisableAllBlockInteractions();
                _blockController.OnBlockClickedDelegate = null;
                SaveMatch(playerData.nickname);
                UIManager.Instance.OpenWinLosePanel(gameResult);
            }
        }
    }
    private void OnBlockClicked(int row, int col)
    {
        if (currentTurn == TurnType.PlayerA && _board[row, col] == PlayerType.None &&
            !forbiddenCollection.Contains((row, col)))
        {
            _gameUIController.UpdateSelectedPosition(row, col);
            _blockController.SetPreviewMarker(row, col, true);
        }
    }

    public void OnClickedPlaceConfirmButton()
    {
        AudioManager.Instance.OnPutStone();
        var (row, col) = _gameUIController.GetSelectedPosition();
        if (currentTurn == TurnType.PlayerA && row != -1 && col != -1 && SetNewBoardValue(PlayerType.PlayerA, row, col))
        {
            currentMoveIndex = (row, col);
            var gameResult = CheckGameResult();
            if (gameResult == GameResult.None)
                SetTurn(TurnType.PlayerB); // AI 턴으로 전환
            else
            {
                _timer.PauseTimer();
                _blockController.DisableAllBlockInteractions();
                _blockController.OnBlockClickedDelegate = null;
                SaveMatch(playerData.nickname);
                UIManager.Instance.OpenWinLosePanel(gameResult);
            }
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

    private void SetForbiddenMarks(List<(int, int)> forbiddenList, bool isForbidden)
    {
        if (isForbidden)
        {
            for (int i = 0; i < forbiddenList.Count; i++)
            {
                _board[forbiddenList[i].Item1, forbiddenList[i].Item2] = PlayerType.PlayerX;
                _blockController.PlaceMarker(Block.MarkerType.Forbidden, forbiddenList[i].Item1, forbiddenList[i].Item2);
            }
        }
        else
        {
            for (int i = 0; i < forbiddenList.Count; i++)
            {
                _board[forbiddenList[i].Item1, forbiddenList[i].Item2] = PlayerType.PlayerX;
                _blockController.PlaceMarker(Block.MarkerType.None, forbiddenList[i].Item1, forbiddenList[i].Item2);
            }
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

    public void OnClickSettingButton()
    {
        UIManager.Instance.OpenSettingPopup();
    }

    public void OnClickGiveupButton()
    {
        UIManager.Instance.OpenGiveupPanel();
    }

}