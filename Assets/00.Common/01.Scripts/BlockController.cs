using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;    // Block 프리팹
    [SerializeField] private int gridSize = 15;         // 15x15 격자
    [SerializeField] private float cellSpacing = 1f;    // 셀 간 간격
    private Block[,] blocks;                            // 2차원 배열로 블록 저장
    public delegate void OnBlockClicked(int row, int col); // 클릭 이벤트 델리게이트
    public OnBlockClicked OnBlockClickedDelegate;

    private int lastPreviewRow = -1; // 마지막 미리보기 행
    private int lastPreviewCol = -1; // 마지막 미리보기 열

    // 블록 초기화
    public void InitBlocks()
    {
        // 배열 초기화
        blocks = new Block[gridSize, gridSize];
        transform.position = Vector3.zero; // 부모 위치를 원점으로 설정

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                // 셀 생성
                GameObject cell = Instantiate(blockPrefab, transform);
                float x = (col - gridSize / 2f) * cellSpacing;  // 중앙 정렬 가로 위치
                float y = -(row - gridSize / 2f) * cellSpacing; // 중앙 정렬 세로 위치
                cell.transform.localPosition = new Vector3(x, y, 0);

                // Block 컴포넌트 가져오기 및 초기화
                Block block = cell.GetComponent<Block>();
                if (block == null)
                {
                    Debug.LogError($"셀 ({row}, {col})에 Block 컴포넌트가 없습니다!");
                    continue;
                }
                block.InitMarker(row * gridSize + col, blockIndex => 
                {
                    // 1D 인덱스를 2D 좌표로 변환하여 델리게이트 호출
                    int clickedRow = blockIndex / gridSize;
                    int clickedCol = blockIndex % gridSize;
                    OnBlockClickedDelegate?.Invoke(clickedRow, clickedCol);
                });
                blocks[row, col] = block;
            }
        }
        Debug.Log("BlockController: 오목판 초기화 완료");
    }

    // 마커 배치
    public void PlaceMarker(Block.MarkerType markerType, int row, int col)
    {
        // 2D 배열로 직접 접근
        if (row >= 0 && row < gridSize && col >= 0 && col < gridSize)
        {
            blocks[row, col].SetMarker(markerType);

            // 미리보기 제거
            if (lastPreviewRow == row && lastPreviewCol == col)
            {
                blocks[row, col].SetPreviewMarker(false);
                lastPreviewRow = -1;
                lastPreviewCol = -1;
            }
        }
        else
        {
            Debug.LogError($"잘못된 위치 ({row}, {col})에 마커를 배치하려 했습니다!");
        }
    }

    // 미리보기 설정
    public void SetPreviewMarker(int row, int col, bool show)
    {
        if (row < 0 || row >= gridSize || col < 0 || col >= gridSize)
        {
            Debug.LogError($"잘못된 미리보기 위치: ({row}, {col})");
            return;
        }

        // 이전 미리보기 제거
        if (lastPreviewRow != -1 && (lastPreviewRow != row || lastPreviewCol != col))
        {
            blocks[lastPreviewRow, lastPreviewCol].SetPreviewMarker(false);
        }

        // 새 미리보기 설정
        blocks[row, col].SetPreviewMarker(show);
        if (show)
        {
            lastPreviewRow = row;
            lastPreviewCol = col;
        }
        else
        {
            lastPreviewRow = -1;
            lastPreviewCol = -1;
        }
    }

    // 보드 초기화
    public void ResetBoard()
    {
        // blocks가 null이면 초기화
        if (blocks == null)
        {
            InitBlocks();
        }

        // 모든 셀을 초기 상태로 리셋
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                if (blocks[row, col] != null)
                {
                    blocks[row, col].SetMarker(Block.MarkerType.None);
                    blocks[row, col].SetPreviewMarker(false);
                }
            }
        }
        lastPreviewRow = -1;
        lastPreviewCol = -1;
    }
}