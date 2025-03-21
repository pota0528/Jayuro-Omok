using UnityEngine;

public class ReplayBlockController : MonoBehaviour
{
    [SerializeField] private Transform boardPosition;
    [SerializeField] private GameObject blockPrefab;     // Block 프리팹 (오목 셀용)
    [SerializeField] private int gridSize = 15;          // 오목판 크기 (15x15)
    [SerializeField] private float cellSpacing = 1f;     // 셀 간 간격 (위치 계산용)
    private Block[,] blocks;                             // 셀 배열

    public void InitBlocks()
    {
        // 배열 초기화
        blocks = new Block[gridSize, gridSize];

        // 부모 오브젝트 위치를 원점으로 설정
        transform.position = Vector3.zero;

        // 모든 셀 생성 및 배치
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                // Block 프리팹 인스턴스 생성
                GameObject cell = Instantiate(blockPrefab, boardPosition);

                // 위치 계산 및 설정 (Transform 사용)
                float x = (col - gridSize / 2f) * cellSpacing;  // 가로 위치 (중앙 정렬)
                float y = -(row - gridSize / 2f) * cellSpacing; // 세로 위치 (위에서 아래로)
                float z = 0f;                                    // 2D이므로 Z축은 0
                cell.transform.localPosition = new Vector3(x, y, z);

                // Block 컴포넌트 가져오기
                Block omokBlock = cell.GetComponent<Block>();
                if (omokBlock == null)
                {
                    Debug.LogError($"셀 ({row}, {col})에 Block 컴포넌트가 없습니다!");
                    continue;
                }

                // Block 초기화 (기보 화면이므로 클릭 이벤트는 필요 없음)
                omokBlock.InitMarker(row * gridSize + col, null);

                // 배열에 저장
                blocks[row, col] = omokBlock;
            }
        }

        Debug.Log("ReplayBlockController: 오목판 초기화 완료");
    }

    public void PlaceStone(int row, int col, int player)
    {
        // 돌 배치
        Block.MarkerType markerType = player == 1 ? Block.MarkerType.Black : Block.MarkerType.White;
        if (blocks[row, col] != null)
        {
            blocks[row, col].SetMarker(markerType);
        }
        else
        {
            Debug.LogError($"돌을 놓을 위치 ({row}, {col})에 Block이 없습니다!");
        }
    }

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
                }
            }
        }
    }
}