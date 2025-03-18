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
            blocks = _blockController.InitBlocks();
            
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
                    forbiddenCollecition = SetForbidden(FindEmptySpotsInRow());
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

        private List<(int, int)> FindEmptySpotsInRow() // int row = currentMoveindex.Item1, int col = currentMoveindex.Item2;
        {
            List<(int, int)> emptyList = new List<(int, int)>();
            int row = currentMoveindex.Item1;
            int col = currentMoveindex.Item2;
            
            int currentCol = col + 1; // 오른쪽 탐색 ) 최근에 놓은 돌을 기준으로 가로 한 칸 이내로 빈칸이 있는지 확인 + 0보다 크고, 15보다 작아야하는 조건 충족
            
            while (0 <= currentCol && currentCol < col + 5 && currentCol <= 15) 
            {
                if (_board[row, currentCol] == PlayerType.None)
                {
                    emptyList.Add((row, currentCol));
                }
                else
                {
                    continue; // 돌을 만나면 탐색 종료
                }
                
                ++currentCol;
            }
            
            currentCol = col - 1; // 왼쪽 탐색 ) 최근에 놓은 돌을 기준으로 가로 한 칸 이내로 빈칸이 있는지 확인 + 0보다 크고, 15보다 작아야하는 조건 충족
            
            while (0 <= currentCol && currentCol > col - 5 && currentCol <= 15) 
            {
                if (_board[row, currentCol] == PlayerType.None)
                {
                    emptyList.Add((row, currentCol));
                }
                else
                {
                    continue; // 돌을 만나면 탐색 종료
                }
                
                --currentCol;
            }
            
            return emptyList;
        }

        private List<(int, int)> SetForbidden(List<(int, int)> emptyList)
        {
            List<(int, int)> forbiddenList = new List<(int, int)>();
            for (int i = 0; i < emptyList.Count; i++)
            {
                // 오른쪽 검사
                int row = emptyList[i].Item1; //공백의 그 다음 자리부터 계산을 해야하니 + 1이 되어야한다.
                int col = emptyList[i].Item2 + 1;
                
                int blockIndex = 0;
                
                for (int j = col; j <= 15 && j < col + 4; j++) // + 조건 j가 0보다 크거나 같고, 15보다 작거나 같아야한다.
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
                
                blockIndex = 0;
                
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
                    forbiddenList.Add((emptyList[i].Item1, emptyList[i].Item2));
                }
            }
            
            return forbiddenList;
        }
        
        private void SetForbiddenMark(List<(int, int)> forbiddenList)
        {
            for (int i = 0; i < forbiddenList.Count; i++)
            {
                _blockController.PlaceMarker(Block.MarkerType.Forbidden, forbiddenList[i].Item1, forbiddenList[i].Item2, moveIndex);
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
}
