using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using lee_namespace;
using UnityEngine.UI;

namespace lee_namespace
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private BlockController _blockController;
        [SerializeField] private GameUIController _gameUIController;
        //[SerializeField] private Button confirmButton;

        public enum PlayerType {None, PlayerA, PlayerB}
        private enum TurnType {PlayerA, PlayerB}
        private enum GameResult {None, Win, Lose, Draw}

        private PlayerType[,] _board;
        private TurnType currentTurn; // 턴 상태 업데이트 위함..
        void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            //TODO: 총 게임 수 증가시키기 (승률 계산 위함)

            _board = new PlayerType[15, 15];
            _blockController.InitBlocks();
            _gameUIController.SetGameUIMode(GameUIController.GameUIMode.Init);
            SetTurn(TurnType.PlayerA);
        }

        private void EndGame(GameResult gameResult)
        {
            _gameUIController.SetGameUIMode(GameUIController.GameUIMode.GameOver);
            _blockController.OnBlockClickedDelegate = null;

            switch (gameResult)
            {
                case GameResult.Win:
                    Debug.Log("PlayerA win");
                    //TODO:  여기서 승점 증가 
                    break;
                case GameResult.Lose:
                    Debug.Log("AI win");
                    //TODO:  여기서 점수 감소
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
            return true;
        }

        /// <summary>
        /// 턴 교체
        /// </summary>
        /// <param name="turnType"></param>
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
                _gameUIController.UpdateSelectedPosition(row, col); // 미리보기 위치 전달
                _blockController.SetPreviewMarker(row, col, true); // 미리보기 표시
            }
        }

        public void OnClickedConfirmButton()
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

        /// <summary>
        /// AI의 움직임
        /// </summary>
        /// <returns></returns>
        private IEnumerator AIMove()
        {
            MCTS mcts = new MCTS(_board);
            var (row, col) = mcts.GetBestMove(1000); // 계속 조정하면서 테스트 하기 (2000 일때 나쁘지않음 (중하수정도))
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

        /// <summary>
        /// 게임결과 알려주는 함수
        /// </summary>
        /// <returns></returns>
        private GameResult CheckGameResult()
        {
            if (CheckGameWin(PlayerType.PlayerA)) return GameResult.Win;
            if (CheckGameWin(PlayerType.PlayerB)) return GameResult.Lose;
            if (GetPossibleMoves(_board).Count == 0) return GameResult.Draw;
            return GameResult.None;
        }

        /// <summary>
        /// 게임이 이겼는지 판단하는 함수
        /// </summary>
        /// <param name="playerType">플레이어 타입</param>
        /// <returns></returns>
        private bool CheckGameWin(PlayerType playerType)
        {
            for (var row = 0; row < _board.GetLength(0); row++)
            {
                for (var col = 0; col < _board.GetLength(1) - 5 + 1; col++)
                {
                    if (_board[row, col] == playerType && _board[row, col + 1] == playerType &&
                        _board[row, col + 2] == playerType && _board[row, col + 3] == playerType &&
                        _board[row, col + 4] == playerType)
                        return true;
                }
            }

            for (var col = 0; col < _board.GetLength(0); col++)
            {
                for (var row = 0; row < _board.GetLength(1) - 5 + 1; row++)
                {
                    if (_board[row, col] == playerType && _board[row + 1, col] == playerType &&
                        _board[row + 2, col] == playerType && _board[row + 3, col] == playerType &&
                        _board[row + 4, col] == playerType)
                        return true;
                }
            }

            for (var row = 0; row < _board.GetLength(0) - 5 + 1; row++)
            {
                for (var col = 0; col < _board.GetLength(1) - 5 + 1; col++)
                {
                    if (_board[row, col] == playerType && _board[row + 1, col + 1] == playerType &&
                        _board[row + 2, col + 2] == playerType && _board[row + 3, col + 3] == playerType &&
                        _board[row + 4, col + 4] == playerType)
                        return true;
                }
            }

            for (var row = 0; row < _board.GetLength(0) - 5 + 1; row++)
            {
                for (var col = 0; col < _board.GetLength(1) - 5 + 1; col++)
                {
                    if (_board[row, col + 4] == playerType && _board[row + 1, col + 3] == playerType &&
                        _board[row + 2, col + 2] == playerType && _board[row + 3, col + 1] == playerType &&
                        _board[row + 4, col] == playerType)
                        return true;
                }
            }

            return false;
        }

        private List<(int row, int col)> GetPossibleMoves(PlayerType[,] board)
        {
            List<(int row, int col)> moves = new List<(int row, int col)>();
            for (int r = 0; r < 15; r++)
            {
                for (int c = 0; c < 15; c++)
                {
                    if (board[r, c] == PlayerType.None)
                        moves.Add((r, c));
                }
            }

            return moves;
        }

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
        }
    }
}