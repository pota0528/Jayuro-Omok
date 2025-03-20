// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
//
//
//
// namespace Joek_namespace
// {
//     public class ScrollViewController : MonoBehaviour
//     {
//         [SerializeField] private float cellHeight; // 자동 배치되지만, 화면 계산에는 필요
//         [SerializeField] private GameObject cellPrefab;
//         [SerializeField] private Transform content; // Content 오브젝트
//         [SerializeField] private ScrollRect scrollRect;
//
//         private List<Item> _items;
//         private LinkedList<Cell> _visibleCells = new LinkedList<Cell>(); // 현재 보이는 셀
//         private Queue<Cell> _cellPool = new Queue<Cell>(); // 오브젝트 풀
//
//         private float _lastYValue = 1f;
//
//         private void Start()
//         {
//             LoadData();
//         }
//
//         private void LoadData()
//         {
//             _items = new List<Item>
//             {
//                 new Item { imageFileName = "image1", title = "Title 1", subtitle = "Subtitle 1" },
//                 new Item { imageFileName = "image2", title = "Title 2", subtitle = "Subtitle 2" },
//                 new Item { imageFileName = "image3", title = "Title 3", subtitle = "Subtitle 3" },
//                 new Item { imageFileName = "image4", title = "Title 4", subtitle = "Subtitle 4" },
//                 new Item { imageFileName = "image5", title = "Title 5", subtitle = "Subtitle 5" },
//                 new Item { imageFileName = "image6", title = "Title 6", subtitle = "Subtitle 6" },
//                 new Item { imageFileName = "image7", title = "Title 7", subtitle = "Subtitle 7" },
//                 new Item { imageFileName = "image8", title = "Title 8", subtitle = "Subtitle 8" },
//                 new Item { imageFileName = "image9", title = "Title 9", subtitle = "Subtitle 9" }
//             };
//
//             ReloadData();
//         }
//
//         private void ReloadData()
//         {
//             // 기존의 모든 셀 제거 (오브젝트 풀로 반환)
//             foreach (var cell in _visibleCells)
//             {
//                 ReturnCell(cell);
//             }
//
//             _visibleCells.Clear();
//
//             // 보이는 셀만 추가
//             var (startIndex, endIndex) = GetVisibleIndexRange();
//             for (int i = startIndex; i <= endIndex; i++)
//             {
//                 AddCell(i);
//             }
//         }
//
//         private void AddCell(int index)
//         {
//             // 오브젝트 풀에서 가져오기
//             Cell cell;
//             if (_cellPool.Count > 0)
//             {
//                 cell = _cellPool.Dequeue();
//             }
//             else
//             {
//                 var obj = Instantiate(cellPrefab, content);
//                 cell = obj.GetComponent<Cell>();
//             }
//
//             cell.SetItem(_items[index], index);
//             cell.gameObject.SetActive(true);
//
//             _visibleCells.AddLast(cell);
//         }
//
//         private void ReturnCell(Cell cell)
//         {
//             cell.gameObject.SetActive(false);
//             _cellPool.Enqueue(cell);
//         }
//
//         private (int startIndex, int endIndex) GetVisibleIndexRange()
//         {
//             var contentPosY = scrollRect.content.anchoredPosition.y; // 스크롤 위치
//             var viewportHeight = scrollRect.viewport.rect.height; // 뷰포트 높이
//
//             int startIndex = Mathf.FloorToInt(contentPosY / cellHeight);
//             int visibleCount = Mathf.CeilToInt(viewportHeight / cellHeight);
//
//             startIndex = Mathf.Max(0, startIndex - 1);
//             visibleCount += 2;
//
//             return (startIndex, startIndex + visibleCount - 1);
//         }
//
//         public void OnValueChanged(Vector2 value)
//         {
//             if (_lastYValue < value.y)
//             {
//                 // 위로 스크롤: 새로운 상단 셀 추가, 하단 셀 제거
//                 var firstCell = _visibleCells.First.Value;
//                 var newFirstIndex = firstCell.Index - 1;
//
//                 if (IsVisibleIndex(newFirstIndex))
//                 {
//                     AddCell(newFirstIndex);
//                 }
//
//                 var lastCell = _visibleCells.Last.Value;
//                 if (!IsVisibleIndex(lastCell.Index))
//                 {
//                     ReturnCell(lastCell);
//                     _visibleCells.RemoveLast();
//                 }
//             }
//             else
//             {
//                 // 아래로 스크롤: 새로운 하단 셀 추가, 상단 셀 제거
//                 var lastCell = _visibleCells.Last.Value;
//                 var newLastIndex = lastCell.Index + 1;
//
//                 if (IsVisibleIndex(newLastIndex))
//                 {
//                     AddCell(newLastIndex);
//                 }
//
//                 var firstCell = _visibleCells.First.Value;
//                 if (!IsVisibleIndex(firstCell.Index))
//                 {
//                     ReturnCell(firstCell);
//                     _visibleCells.RemoveFirst();
//                 }
//             }
//
//             _lastYValue = value.y;
//         }
//
//         private bool IsVisibleIndex(int index)
//         {
//             var (startIndex, endIndex) = GetVisibleIndexRange();
//             return index >= startIndex && index <= endIndex;
//         }
//     }
// }
