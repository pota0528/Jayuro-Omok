using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField] private Block[] blocks; // 블록 배열
    [SerializeField] private int gridSize = 15; // 15x15 격자

    public delegate void OnBlockClicked(int row, int col); // 클릭 이벤트 델리게이트
    public OnBlockClicked OnBlockClickedDelegate;

    private int lastPreviewRow = -1; // 마지막 미리보기 행
    private int lastPreviewCol = -1; // 마지막 미리보기 열

    // 블록 초기화
    public void InitBlocks()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].InitMarker(i, blockIndex =>
            {
                var clickedRow = blockIndex / gridSize;
                var clickedCol = blockIndex % gridSize;
                OnBlockClickedDelegate?.Invoke(clickedRow, clickedCol); // 행, 열 전달
            });
        }
    }

    // 마커 배치
    public void PlaceMarker(Block.MarkerType markerType, int row, int col)
    {
        var index = row * gridSize + col;
        blocks[index].SetMarker(markerType);
        // 미리보기 제거
        if (lastPreviewRow == row && lastPreviewCol == col)
        {
            blocks[index].SetPreviewMarker(false);
            lastPreviewRow = -1;
            lastPreviewCol = -1;
        }
    }

    // 미리보기 설정
    public void SetPreviewMarker(int row, int col, bool show)
    {
        if (lastPreviewRow != -1 && (lastPreviewRow != row || lastPreviewCol != col))
        {
            var lastIndex = lastPreviewRow * gridSize + lastPreviewCol;
            blocks[lastIndex].SetPreviewMarker(false); // 이전 미리보기 제거
        }

        var index = row * gridSize + col;
        blocks[index].SetPreviewMarker(show);
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
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].SetMarker(Block.MarkerType.None);
            blocks[i].SetPreviewMarker(false);
        }
        lastPreviewRow = -1;
        lastPreviewCol = -1;
    }
}