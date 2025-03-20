using System.Collections.Generic;
using UnityEngine;


public class BlockController : MonoBehaviour
{
    [SerializeField] private GameObject omokCellPrefab; // OmokCell 프리팹
    [SerializeField] private int gridSize = 15; // 15x15 격자
    [SerializeField] private float cellSize = 40f; // 한 칸의 크기

    private OmokBlock[,] blocks;
    private int[,] board; // 게임의 상태 (0: 빈칸, 1: 검은돌, 2: 흰돌)
    private bool isBlackTurn = true; // 검은돌 차례인지 확인
    private bool isGameOver = false; // 게임종료 확인
    private List<Move> moves = new List<Move>(); // 매치 기록용 리스트

    void Start()
    {
        isGameOver = false; // 게임 시작 시 종료 상태를 초기화 해준다.
        InitBlocks();
    }

    public void InitBlocks()
    {
        blocks = new OmokBlock[gridSize, gridSize];
        board = new int[gridSize, gridSize]; // 게임 상태 초기화
        moves.Clear(); // 새 게임 시작 시 moves 초기화
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                // 버튼 생성
                GameObject cell = Instantiate(omokCellPrefab, transform);
                RectTransform rect = cell.GetComponent<RectTransform>();
                // 위치 계산: 왼쪽 위부터 시작
                float x = col * cellSize;
                float y = -row * cellSize; // Y는 위에서 아래로 감소
                rect.anchoredPosition = new Vector2(x, y);
                rect.sizeDelta = new Vector2(cellSize, cellSize);

                OmokBlock omokBlock = cell.GetComponent<OmokBlock>();
                omokBlock.Init(row, col, OnBlockClicked);
                blocks[row, col] = omokBlock;
            }
        }
    }

    private void OnBlockClicked(int row, int col)
    {
        if (isGameOver || board[row, col] != 0) return; // 게임이 종료됐거나 행, 열이 0이 아니라면 돌을 못 놓게 설정.
        
        // 돌 놓기
        PlaceStone(row, col);
        CheckWin(row, col); // 승리 조건 확인
    }

    // 기본 PlaceStone: 턴을 변경하며 돌을 놓음 (게임 진행용)
    private void PlaceStone(int row, int col)
    {
        OmokBlock.MarkerType markerType = isBlackTurn ? OmokBlock.MarkerType.Black : OmokBlock.MarkerType.White;
        blocks[row, col].SetMarker(markerType);
        board[row, col] = isBlackTurn ? 1 : 2; // 게임 상태 업데이트
        moves.Add(new Move { row = row, col = col, color = isBlackTurn ? "흑돌" : "백돌" }); // 돌을 놓을때 기록됨.
        isBlackTurn = !isBlackTurn; // 턴 넘기기
    }

    // 오버로드한 PlaceStone: 턴 변경 없이 특정 플레이어의 돌을 놓음 (기보용 [ReplayController])
    public void PlaceStone(int row, int col, int player)
    {
        OmokBlock.MarkerType markerType = player == 1 ? OmokBlock.MarkerType.Black : OmokBlock.MarkerType.White;
        blocks[row, col].SetMarker(markerType);
        board[row, col] = player;
    }

    // 오목판 초기화
    public void ResetBoard()
    {
        if (blocks == null)
        {
            InitBlocks(); // blocks가 null이면 초기화 시켜준다.
        }
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                blocks[row, col].SetMarker(OmokBlock.MarkerType.None);
                board[row, col] = 0;
            }
        }

        isBlackTurn = true;
        isGameOver = false;
    }

    // 승리 조건 확인
    private void CheckWin(int row, int col)
    {
        int player = board[row, col];

        // 4가지 방향: 가로, 세로, 대각선 (왼 -> 오), 대각선 (오 -> 왼)
        int[,] directions = { { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 } };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int dx = directions[i, 0], dy = directions[i, 1];
            int count = 1;

            // 한쪽 방향 검사
            for (int j = 1; j < 5; j++)
            {
                int newRow = row + dx * j, newCol = col + dy * j;
                if (newRow < 0 || newRow >= gridSize || newCol < 0 || newCol >= gridSize || board[newRow, newCol] != player) break;
                count++;
            }

            // 반대쪽 방향 검사
            for (int j = 1; j < 5; j++)
            {
                int newRow = row - dx * j, newCol = col - dy * j;
                if (newRow < 0 || newRow >= gridSize || newCol < 0 || newCol >= gridSize || board[newRow, newCol] != player) break;
                count++;
            }

            // 승리 판정
            if (count >= 5)
            {
                Debug.Log($"{(player == 1 ? "흑돌" : "백돌")} 승리!");
                isGameOver = true; // 게임 종료 상태로 변경해서, 돌을 더 못놓게 설정.
                
                // 게임 종료할 때 매치 저장
                string nickname = "Player1"; // 실제 유저 닉네임으로 대체
                MatchSaver saver = FindObjectOfType<MatchSaver>();
                if (saver != null)
                {
                    saver.SaveMatch(nickname, moves);
                }
                else
                {
                    Debug.LogError("MatchSave 컴포넌트가 없습니다.");
                }
                return;
            }
        }
    }
}