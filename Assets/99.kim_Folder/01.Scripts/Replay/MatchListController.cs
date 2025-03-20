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
        MatchLoader loader = FindObjectOfType<MatchLoader>();
        if (loader != null)
        {
            matches = loader.LoadMatches();
        }
        else
        {
            Debug.LogError("MatchLoader 컴포넌트가 없습니다.");
        }
    }

    private void DisplayMatches()
    {
        List<Item> items = new List<Item>();
        for (int i = 0; i < matches.Count; i++)
        {
            int index = i;
            items.Add(new Item
            {
                subtitle = matches[i].title + "_" + matches[i].date, // 닉네임 + 날짜 표시
                onClick = () => MatchSelected?.Invoke(matches[index]) // 클릭 시 이벤트 발생
            });
        }

        scrollView.LoadData(items); // 스크롤뷰에 데이터 설정
    }
}