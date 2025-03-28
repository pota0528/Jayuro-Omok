using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RankingPanelController : PanelController
{
    [SerializeField] private GameObject rankingCellPrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private GameObject ScrollView;

    [SerializeField] private float cellHeight;
    [SerializeField] private float spacingY = 10f;

    private ScrollRect _scrollViewScrollRect;
    private RectTransform _scrollViewRectTransform;

    private List<(int index, RankingCellPanel rankingCell)> _visibleCells;
    private float _previousScrollRectYValue = 1f;
    public int _maxRankingCount = 50;
    private int _bufferRows = 2;

    private List<PlayerData> _topPlayers;

    protected override void Awake()
    {
        base.Awake();
        _scrollViewScrollRect = ScrollView.GetComponent<ScrollRect>();
        _scrollViewRectTransform = ScrollView.GetComponent<RectTransform>();
    }

    private void Start()
    {
        SetTitleText("장원급제");
        LoadRanking();
        ReloadData();
    }

    private void LoadRanking()
    {
        var allPlayers = DBManager.Instance.GetAllPlayers();
        allPlayers.Sort((a, b) => CalculateScore(b).CompareTo(CalculateScore(a)));
        _topPlayers = allPlayers.Take(_maxRankingCount).ToList();

        ShowMyRanking();
    }

    private int CalculateScore(PlayerData player)
    {
        return (19 - player.level) * 1000
             + player.levelPoint * 100
             + player.win * 10;
    }

    private (int start, int count) GetVisibleIndexRange()
    {
        float contentPosY = _scrollViewScrollRect.content.anchoredPosition.y;
        float viewportHeight = _scrollViewScrollRect.viewport.rect.height;

        int start = Mathf.FloorToInt(contentPosY / (cellHeight + spacingY));
        int visibleCount = Mathf.CeilToInt(viewportHeight / (cellHeight + spacingY));

        start = Mathf.Max(0, start - _bufferRows);
        int count = Mathf.Min(_maxRankingCount, start + visibleCount + (_bufferRows * 2));

        return (start, count);
    }

    private bool IsVisibleIndex(int index)
    {
        var (start, end) = GetVisibleIndexRange();
        return start <= index && index <= end;
    }

    private RankingCellPanel CreateRankingCellPanel(int index)
    {
        PlayerData playerData;

        if (_topPlayers != null && index < _topPlayers.Count)
        {
            playerData = _topPlayers[index];
        }
        else
        {
            playerData = new PlayerData
            {
                nickname = "순위 없음",
                level = 0,
                win = 0,
                lose = 0,
                imageIndex = -1
            };
        }

        var rankingCellPanelObject = ObjectPool.Instance.GetObject();
        var rankingCellPanel = rankingCellPanelObject.GetComponent<RankingCellPanel>();

        rankingCellPanel.SetRankingCellData(playerData, index);
        rankingCellPanel.transform.SetParent(contentTransform, false);

        float yPosition = -(cellHeight + spacingY) * index;
        rankingCellPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yPosition);

        return rankingCellPanel;
    }

    private void AdjustContentSize()
    {
        RectTransform contentRect = contentTransform as RectTransform;
        float totalHeight = _maxRankingCount * (cellHeight + spacingY);
        contentRect.sizeDelta = new Vector2(0, totalHeight);
    }

    private void ReloadData()
    {
        AdjustContentSize();
        _visibleCells = new List<(int index, RankingCellPanel rankingCell)>();

        var (start, count) = GetVisibleIndexRange();
        for (int i = start; i < count; i++)
        {
            if (i < _maxRankingCount && !_visibleCells.Any(cell => cell.index == i))
            {
                var rankingCellPanel = CreateRankingCellPanel(i);
                _visibleCells.Add((i, rankingCellPanel));
            }
        }
    }

    //MyRanking
    private void ShowMyRanking()
    {
        var myPlayerData = UserSessionManager.Instance.GetPlayerData();
        int myRankIndex = _topPlayers.FindIndex(p => p.id == myPlayerData.id);

        if (myRankIndex >= 0)
        {
            var myPanel = FindObjectOfType<MyRankingPanelController>();
            if (myPanel != null)
            {
                myPanel.SetMyRankingData(myPlayerData, myRankIndex);
            }
        }
    }

    
    
    
    
    
    
    public void OnValueChanged(Vector2 value)
    {
        if (_visibleCells.Count == 0) return;

        if (_previousScrollRectYValue < value.y)
        {
            if (_visibleCells.Count > 0)
            {
                var firstRow = _visibleCells.First();
                var newFirstIndex = firstRow.index - 1;

                if (IsVisibleIndex(newFirstIndex) && newFirstIndex >= 0)
                {
                    var rankingCellPanel = CreateRankingCellPanel(newFirstIndex);
                    _visibleCells.Insert(0, (newFirstIndex, rankingCellPanel));
                }
            }

            if (_visibleCells.Count > 0)
            {
                var lastRow = _visibleCells.Last();
                if (!IsVisibleIndex(lastRow.index))
                {
                    ObjectPool.Instance.ReturnObject(lastRow.rankingCell.gameObject);
                    _visibleCells.RemoveAt(_visibleCells.Count - 1);
                }
            }
        }
        else
        {
            if (_visibleCells.Count > 0)
            {
                var lastRow = _visibleCells.Last();
                var newLastIndex = lastRow.index + 1;

                if (IsVisibleIndex(newLastIndex) && newLastIndex < _maxRankingCount)
                {
                    var rankingCellPanel = CreateRankingCellPanel(newLastIndex);
                    _visibleCells.Add((newLastIndex, rankingCellPanel));
                }
            }

            if (_visibleCells.Count > 0)
            {
                var firstRow = _visibleCells.First();
                if (!IsVisibleIndex(firstRow.index))
                {
                    ObjectPool.Instance.ReturnObject(firstRow.rankingCell.gameObject);
                    _visibleCells.RemoveAt(0);
                }
            }
        }

        _previousScrollRectYValue = value.y;
    }

    public void OnClickCloseButton()
    {
        Hide();
    }
}
