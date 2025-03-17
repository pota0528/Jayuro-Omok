using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace rho_namespace
{
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
        private Block[] blocks;
        private Block currentBlackBlock;
        private const int LINE_COUNT = 15;
        private List<(int, int)> forbiddenPositions;
        
        void Start()
        {
            StartGame();
        }
        
        private void StartGame()
        {
            // _board 초기화
            _board = new PlayerType[LINE_COUNT, LINE_COUNT];
            
            // 블록 초기화
            blocks = _blockController.InitBlocks();
            
            // Game UI 초기화
            _gameUIController.SetGameUIMode(GameUIController.GameUIMode.Init);
            _board[LINE_COUNT / 2, LINE_COUNT / 2] = PlayerType.PlayerA;
            currentMoveindex = (LINE_COUNT / 2, LINE_COUNT / 2);
            _blockController.PlaceMarker(Block.MarkerType.Black, LINE_COUNT / 2, LINE_COUNT / 2, moveIndex);
            SetTurn(TurnType.PlayerB);
            ++moveIndex;
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
                    forbiddenPositions = CheckForbiddenBoardLongJoists();
                    _blockController.OnBlockClickedDelegate = (row, col) =>
                    {
                        if (SetNewBoardValue(PlayerType.PlayerA, row, col))
                        {
                            currentMoveindex = (row, col);
                            ++moveIndex;
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
                        if (SetNewBoardValue(PlayerType.PlayerB, row, col))
                        {
                            ++moveIndex;
                            var gameResult = CheckGameResult();
                            if (gameResult == GameResult.None)
                            {
                                SetTurn(TurnType.PlayerA);
                            }
                            else
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

        private List<(int, int)> CheckForbiddenBoardLongJoists()
        {
            List<(int, int)> forbiddenList = new List<(int, int)>();
            int row = currentMoveindex.Item1;
            int col = currentMoveindex.Item2;

            // 각 방향별로 6목 이상 여부 체크
            if (CountStones(row, col, 1, 0) >= 5)
            {
                forbiddenPositions.Add((row, col)); // 가로(→)
            }           
            
            if (CountStones(row, col, 0, 1) >= 5)
            {
                forbiddenPositions.Add((row, col)); // 세로(↓)
            }
            
            if (CountStones(row, col, 1, 1) >= 5)
            {
                forbiddenPositions.Add((row, col)); // 대각선(↘)
            }
            
            if (CountStones(row, col, 1, -1) >= 5)
            {
                forbiddenPositions.Add((row, col)); // 대각선(↙)
            }

            // 금수 위치 마커 표시
            foreach (var (fRow, fCol) in forbiddenPositions)
            {
                _board[fRow, fCol] = PlayerType.PlayerX;
                _blockController.PlaceMarker(Block.MarkerType.Forbidden, fRow, fCol, moveIndex);
            }

            return forbiddenList;
        }
        
        private int CountStones(int row, int col, int dRow, int dCol)
        {
            int count = 1;  // 현재 돌 포함
            int voidCount = 0;  // 빈칸 개수
            int r, c;

            // 한쪽 방향으로 체크
            r = row + dRow;
            c = col + dCol;
            while (r >= 0 && r < LINE_COUNT && c >= 0 && c < LINE_COUNT)
            {
                if (_board[r, c] == PlayerType.PlayerA) // 흑돌이면 카운트 증가
                {
                    count++;
                }
                else if (_board[r, c] == PlayerType.PlayerB) // 백돌이 나오면 count 초기화, 하지만 계속 검사해야 함!
                {
                    count = 0;
                    continue;  // 다음 칸 검사
                }
                else if (_board[r, c] == PlayerType.None) // 빈칸이면 voidCount 증가
                {
                    voidCount++;
                    if (voidCount >= 2) // 빈칸이 2개 이상이면 장목 체크 중단
                    {
                        continue;
                    }
                }
                r += dRow;
                c += dCol;
            }

            // 반대쪽 방향으로 체크
            voidCount = 0;  // 다시 빈칸 개수 초기화
            r = row - dRow;
            c = col - dCol;
            while (r >= 0 && r < LINE_COUNT && c >= 0 && c < LINE_COUNT)
            {
                if (_board[r, c] == PlayerType.PlayerA) // 흑돌이면 카운트 증가
                {
                    count++;
                }
                else if (_board[r, c] == PlayerType.PlayerB) // 백돌이 나오면 count 초기화, 하지만 계속 검사해야 함!
                {
                    count = 0;
                    continue;  // 다음 칸 검사
                }
                else if (_board[r, c] == PlayerType.None) // 빈칸이면 voidCount 증가
                {
                    voidCount++;
                    if (voidCount >= 2) // 빈칸이 2개 이상이면 장목 체크 중단
                    {
                        break;
                    }
                }
                r -= dRow;
                c -= dCol;
            }

            return count;
        }

        /// <summary>
        /// 게임 결과 확인 함수
        /// </summary>
        /// <returns>플레이어 기준 게임 결과</returns>
        private GameResult CheckGameResult()
        {
            if (CheckGameWin(PlayerType.PlayerA)) { return GameResult.Win; }
            if (CheckGameWin(PlayerType.PlayerB)) { return GameResult.Lose; }
            
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
}
