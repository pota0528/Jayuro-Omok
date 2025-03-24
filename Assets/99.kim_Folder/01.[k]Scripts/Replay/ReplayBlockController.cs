using System.Collections.Generic;
using UnityEngine;

public class ReplayBlockController : MonoBehaviour
{
    [SerializeField] private GameObject omokCellPrefab; // 오목 셀 프리팹
    [SerializeField] private Transform[] omokPositionList;
    private const int OMOK_LINE = 15;
    private Block[,] blocks; // 셀 배열

    public void InitBlocks()
    {
        //ResetBoard();
        blocks = new Block[OMOK_LINE, OMOK_LINE];

        for (int row = 0; row < OMOK_LINE; row++)
        {
            for (int col = 0; col < OMOK_LINE; col++)
            {
                int blockLine = row * OMOK_LINE + col; //blocks는 2차원 배열인데,
                                                       //Transform값은 1차원 배열이라 1차원배열로 우선 index값 받아와야함
                GameObject cell = Instantiate(omokCellPrefab, omokPositionList[blockLine]);
                // 위치 계산 및 설정 (Transform 사용)
                cell.transform.localScale = Vector3.one;

                // Block 컴포넌트 가져오기
                Block omokBlock = cell.GetComponent<Block>();
                if (omokBlock == null)
                {
                    Debug.LogError($"셀 ({row}, {col})에 Block 컴포넌트가 없습니다!");
                    continue;
                }

                // Block 초기화 (기보 화면이므로 클릭 이벤트는 필요 없음)
                omokBlock.InitMarker(row * OMOK_LINE + col, null);

                // 배열에 저장
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
        }
        
        for (int row = 0; row < OMOK_LINE; row++)
        {
            for (int col = 0; col < OMOK_LINE; col++)
            {
                Debug.Log("이중 들어옴");
                Debug.Log(blocks.Length);
                Debug.Log(blocks[row, col]);
                blocks[row, col].SetMarker(Block.MarkerType.None);
            }
        }
    }
}