using System.Collections.Generic;
using kim_namespace;
using UnityEngine;

public class ReplayController : MonoBehaviour
{
    [SerializeField] private BlockController blockController; // 오목판 관리 클래스
    [SerializeField] private ScrollViewController logScrollView; // 로그 스크롤뷰
    [SerializeField] private GameObject replayPanel; // 재생 패널 UI

    private MatchData currentMatch;
    private int currentStep = 0;

    public void LoadMatch(MatchData match)
    {
        currentMatch = match;
        currentStep = 0;
        blockController.ResetBoard(); // 오목판 초기화
        LoadLog(match.moves); // 로그 로드
        replayPanel.SetActive(true); // 재생 패널 활성화
    }

    private void LoadLog(List<Move> moves)
    {
        List<Item> logItems = new List<Item>();
        for (int i = 0; i < moves.Count; i++)
        {
            Move move = moves[i];
            string logText = $"Step {i + 1}: Row {move.row}, Col {move.col}, {move.color}";
            logItems.Add(new Item { subtitle = logText });
        }

        logScrollView.LoadData(logItems); // 로그 스크롤뷰에 데이터 설정
    }

    public void NextMove() // 다음 수 표시
    {
        if (currentStep < currentMatch.moves.Count)
        {
            Move move = currentMatch.moves[currentStep];
            blockController.PlaceStone(move.row, move.col, move.color == "black" ? 1 : 2);
            currentStep++;
        }
    }

    public void PreviousMove() // 이전 수로 돌아가기
    {
        if (currentStep > 0)
        {
            currentStep--;
            blockController.ResetBoard();
            for (int i = 0; i < currentStep; i++)
            {
                Move move = currentMatch.moves[i];
                blockController.PlaceStone(move.row, move.col, move.color == "black" ? 1 : 2);
            }
        }
    }

    public void FirstMove() // 첫 수로 이동
    {
        currentStep = 0;
        blockController.ResetBoard();
    }

    public void LastMove() // 마지막 수로 이동
    {
        blockController.ResetBoard();
        foreach (var move in currentMatch.moves)
        {
            blockController.PlaceStone(move.row, move.col, move.color == "black" ? 1 : 2);
        }

        currentStep = currentMatch.moves.Count;
    }
}