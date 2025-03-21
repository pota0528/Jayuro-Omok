using UnityEngine;

public class ReplayBlockController : MonoBehaviour
{
    [SerializeField] private GameObject omokCellPrefab; // 오목 셀 프리팹
    [SerializeField] private int gridSize = 15; // 오목판 크기
    [SerializeField] private float cellSize = 64f; // 셀 크기
    private Block[,] blocks; // 셀 배열

    public void InitBlocks()
    {
        blocks = new Block[gridSize, gridSize];
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
                Block omokBlock = cell.GetComponent<Block>();
                //omokBlock.InitMarker(row, col, null); // 클릭 이벤트 없음
                blocks[row, col] = omokBlock;
            }
        }
    }

    public void PlaceStone(int row, int col, int player)
    {
        Block.MarkerType markerType = player == 1 ? Block.MarkerType.Black : Block.MarkerType.White;
        blocks[row, col].SetMarker(markerType);
    }

    public void ResetBoard()
    {
        if (blocks == null)
        {
            Debug.Log(blocks.Length +"초기화 전");
            InitBlocks(); // blocks가 null이면 초기화
            Debug.Log(blocks.Length + "초기화 후");
        }
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                Debug.Log("이중 들어옴");
                Debug.Log(blocks.Length);
                Debug.Log(blocks[row, col]);
                blocks[row, col].SetMarker(Block.MarkerType.None);
            }
        }
    }
}