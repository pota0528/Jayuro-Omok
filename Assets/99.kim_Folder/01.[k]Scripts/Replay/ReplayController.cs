using UnityEngine;
using System.Collections.Generic;

public class ReplayController : MonoBehaviour
{
    [SerializeField] private ReplayBlockController blockController; // 리플레이용 오목판
    [SerializeField] private ScrollViewController logScrollView; // 로그 표시용 스크롤 뷰
    [SerializeField] private GameObject replayPanel; // 리플레이 패널
    private MatchData currentMatch; // 현재 매치 데이터
    private int currentStep = 0; // 현재 진행 단계
    private List<Item> logItems = new List<Item>(); // 로그 아이템 리스트
    
    public void LoadMatch(MatchData match)
    {
        currentMatch = match;
        currentStep = 0;
        blockController.InitBlocks(); // blocks 배열 초기화
        blockController.ResetBoard(); // 오목판 초기화
        logItems.Clear(); // 로그 초기화
        logScrollView.LoadData(logItems); // 빈 로그 표시
        replayPanel.SetActive(true); // 패널 활성화
    }
    
    public void FirstMove()
    {
        currentStep = 0; // 처음 단계로 설정
        blockController.ResetBoard(); // 보드 초기화
        logItems.Clear(); // 로그 초기화
        logScrollView.LoadData(logItems); // 빈 로그 표시
    }
    
    public void LastMove()
    {
        blockController.ResetBoard(); // 보드 초기화
        logItems.Clear(); // 로그 초기화
        for (int i = 0; i < currentMatch.moves.Count; i++) // 모든 수 재현
        {
            Move move = currentMatch.moves[i];
            blockController.PlaceStone(move.row, move.col, move.color == "흑돌" ? 1 : 2);
            string logText = $" {i + 1}: 행 {move.row}, 열 {move.col}, {move.color}";
            logItems.Add(new Item { subtitle = logText });
        }
        currentStep = currentMatch.moves.Count; // 마지막 단계로 설정
        logScrollView.LoadData(logItems); // 로그 업데이트
    }
    
    public void NextMove()
    {
        if (currentMatch == null || currentStep >= currentMatch.moves.Count) return;
        Move move = currentMatch.moves[currentStep];
        blockController.PlaceStone(move.row, move.col, move.color == "흑돌" ? 1 : 2);
        string logText = $" {currentStep + 1}: 행 {move.row}, 열 {move.col}, {move.color}";
        logItems.Add(new Item { subtitle = logText });
        logScrollView.LoadData(logItems); // 로그 업데이트
        currentStep++;
    }

    public void PreviousMove()
    {
        if (currentStep <= 0) return;
        currentStep--;
        blockController.ResetBoard(); // 보드 리셋
        logItems.RemoveAt(logItems.Count - 1); // 마지막 로그 제거
        for (int i = 0; i < currentStep; i++)
        {
            Move move = currentMatch.moves[i];
            blockController.PlaceStone(move.row, move.col, move.color == "흑돌" ? 1 : 2);
        }
        logScrollView.LoadData(logItems); // 로그 업데이트
    }
}