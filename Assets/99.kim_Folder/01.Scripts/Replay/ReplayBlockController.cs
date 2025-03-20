using UnityEngine;

public class ReplayBlockController : MonoBehaviour
{
    [SerializeField] private GameObject omokCellPrefab; // 오목 셀 프리팹
    [SerializeField] private int gridSize = 15; // 오목판 크기
    [SerializeField] private float cellSize = 64f; // 셀 크기
    private OmokBlock[,] blocks; // 셀 배열

    // 오목판 초기화
    public void InitBlocks()
    {
        blocks = new OmokBlock[gridSize, gridSize];
        RectTransform parentRect = GetComponent<RectTransform>();
        parentRect.sizeDelta = new Vector2(gridSize * cellSize, gridSize * cellSize);

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                GameObject cell = Instantiate(omokCellPrefab, transform);
                RectTransform rect = cell.GetComponent<RectTransform>();
                float x = (col - gridSize / 2f) * cellSize; // 중앙 정렬
                float y = -(row - gridSize / 2f) * cellSize;
                rect.anchoredPosition = new Vector2(x, y);
                rect.sizeDelta = new Vector2(cellSize, cellSize);
                OmokBlock omokBlock = cell.GetComponent<OmokBlock>();
                omokBlock.Init(row, col, null); // 클릭 이벤트 없음
                blocks[row, col] = omokBlock;
            }
        }
    }

    // 돌 표시 함수
    public void PlaceStone(int row, int col, int player)
    {
        OmokBlock.MarkerType markerType = player == 1 ? OmokBlock.MarkerType.Black : OmokBlock.MarkerType.White;
        blocks[row, col].SetMarker(markerType);
    }

    // 오목판 리셋
    public void ResetBoard()
    {
        if (blocks == null)
        {
            InitBlocks(); // blocks가 null이면 초기화
        }
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                blocks[row, col].SetMarker(OmokBlock.MarkerType.None);
            }
        }
    }
}