using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

  public class RankingPanelController : PanelController
    {
        [SerializeField] private GameObject rankingCellPrefab;  // 생성할 셀 프리팹
        [SerializeField] private Transform contentTransform;    // ScrollView의 Content (셀들의 부모)
        [SerializeField] private GameObject ScrollView;         // 스크롤 뷰 오브젝트

        [SerializeField] private float cellHeight;              // 셀 높이
        [SerializeField] private float spacingY = 10f;          // 셀 간 간격

        private ScrollRect _scrollViewScrollRect;               // ScrollRect 컴포넌트
        private RectTransform _scrollViewRectTransform;         // ScrollView의 RectTransform

        private List<(int index, RankingCellPanel rankingCell)> _visibleCells; // 현재 화면에 표시 중인 셀 리스트
        private float _previousScrollRectYValue = 1f;           // 이전 프레임에서의 스크롤 위치
        public int _maxRankingCount = 50;                       // 전체 랭킹 데이터 개수
        private int _bufferRows = 2;                            // 위아래 여유 셀 개수 (화면 밖에서도 추가 생성)

        protected override void Awake()
        {
            base.Awake();
            _scrollViewScrollRect = ScrollView.GetComponent<ScrollRect>();
            _scrollViewRectTransform = ScrollView.GetComponent<RectTransform>();
        }

        private void Start()
        {
            SetTitleText("장원급제"); // 패널 타이틀 설정
            ReloadData(); // 데이터 로드 및 초기 화면 설정
        }

        /// <summary>
        /// 현재 화면에 표시될 셀의 인덱스 범위를 계산
        /// (버퍼 추가하여 여유 셀 포함)
        /// </summary>
        private (int start, int count) GetVisibleIndexRange()
        {
            float contentPosY = _scrollViewScrollRect.content.anchoredPosition.y; // 현재 스크롤 위치
            float viewportHeight = _scrollViewScrollRect.viewport.rect.height;    // 뷰포트 높이

            int start = Mathf.FloorToInt(contentPosY / (cellHeight + spacingY));  // 시작 인덱스 계산
            int visibleCount = Mathf.CeilToInt(viewportHeight / (cellHeight + spacingY)); // 화면에 표시 가능한 개수

            start = Mathf.Max(0, start - _bufferRows); // 위쪽 여유 셀 추가
            int count = Mathf.Min(_maxRankingCount, start + visibleCount + (_bufferRows * 2)); // 전체 개수 제한

            return (start, count);
        }

        /// <summary>
        /// 특정 인덱스가 현재 화면에 표시되어야 하는지 확인
        /// </summary>
        private bool IsVisibleIndex(int index)
        {
            var (start, end) = GetVisibleIndexRange();
            return start <= index && index <= end;
        }

        /// <summary>
        /// 새로운 랭킹 셀을 생성하여 Content에 추가
        /// (오브젝트 풀에서 가져와서 재사용)
        /// </summary>
        private RankingCellPanel CreateRankingCellPanel(int index)
        {
            var rankingCellPanelObject = ObjectPool.Instance.GetObject();
            var rankingCellPanel = rankingCellPanelObject.GetComponent<RankingCellPanel>();

            rankingCellPanel.SetRankingCell(index); // 셀에 랭킹 정보 적용
            rankingCellPanel.transform.SetParent(contentTransform, false); // Content 하위에 배치

            // 위치 계산 후 적용
            float yPosition = -(cellHeight + spacingY) * index;
            rankingCellPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yPosition);

            return rankingCellPanel;
        }

        /// <summary>
        /// Content 크기를 전체 데이터 개수에 맞춰 조정
        /// </summary>
        private void AdjustContentSize()
        {
            RectTransform contentRect = contentTransform as RectTransform;
            float totalHeight = _maxRankingCount * (cellHeight + spacingY); // 총 리스트 높이 계산
            contentRect.sizeDelta = new Vector2(0, totalHeight);
        }

        /// <summary>
        /// 데이터 로드 및 화면 초기화 (초기 셀 생성)
        /// </summary>
        private void ReloadData()
        {
            AdjustContentSize(); // Content 크기 조정
            _visibleCells = new List<(int index, RankingCellPanel rankingCell)>(); // 현재 보이는 셀 리스트 초기화

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

        /// <summary>
        /// 스크롤 시 기존 셀을 재활용하여 새로운 데이터를 표시
        /// </summary>
        public void OnValueChanged(Vector2 value)
        {
            if (_visibleCells.Count == 0) return; // 🚨 빈 리스트에서 호출 방지

            if (_previousScrollRectYValue < value.y)
            {
                // 🔻 위로 스크롤 (맨 위의 셀을 제거하고 아래로 이동)
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

                // 🔻 보이지 않는 셀 제거
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
                // 🔺 아래로 스크롤 (맨 아래의 셀을 제거하고 위로 이동)
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

                // 🔺 보이지 않는 셀 제거
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