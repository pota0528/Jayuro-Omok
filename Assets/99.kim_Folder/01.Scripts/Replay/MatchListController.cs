using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchListController : MonoBehaviour
{
    [SerializeField] private ScrollViewController scrollView; // 매치 목록용 스크롤뷰

    public event Action<MatchData> MatchSelected; // 매치 선택 이벤트

    private List<MatchData> matches = new List<MatchData>(); // 매치 데이터 리스트

    private void Start()
    {
        LoadMatches(); // 매치 데이터 로드
        DisplayMatches(); // 매치 목록 표시
    }

    private void LoadMatches()
    {
        // 예시 데이터s
        matches.Add(new MatchData { matchName = "Match 1", moves = new List<Move> { new Move { row = 1, col = 1, color = "black" } } });
        matches.Add(new MatchData { matchName = "Match 2", moves = new List<Move> { new Move { row = 2, col = 2, color = "white" } } });
    }

    private void DisplayMatches()
    {
        List<Item> items = new List<Item>();
        for (int i = 0; i < matches.Count; i++)
        {
            int index = i;
            items.Add(new Item
            {
                subtitle = matches[i].matchName,
                onClick = () => MatchSelected?.Invoke(matches[index]) // 클릭 시 이벤트 발생
            });
        }

        scrollView.LoadData(items); // 스크롤뷰에 데이터 설정
    }
}

public class MatchData
{
    public string matchName;
    public List<Move> moves;
}

public class Move
{
    public int row;
    public int col;
    public string color;
}