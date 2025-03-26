using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField] private Block[] blocks;
    [SerializeField] private int gridSize = 15;

    public delegate void OnBlockClicked(int row, int col);
    public OnBlockClicked OnBlockClickedDelegate;

    private int lastPreviewRow = -1;
    private int lastPreviewCol = -1;
    
    private int lastBlackRow = -1;  // 흑돌 최근 위치
    private int lastBlackCol = -1;
    
    private int lastWhiteRow = -1;  // 백돌 최근 위치
    private int lastWhiteCol = -1;

    public void InitBlocks()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].InitMarker(i, blockIndex =>
            {
                var clickedRow = blockIndex / gridSize;
                var clickedCol = blockIndex % gridSize;
                OnBlockClickedDelegate?.Invoke(clickedRow, clickedCol);
            });
            blocks[i].SetMarker(Block.MarkerType.None);
            blocks[i].SetPreviewMarker(false);
            blocks[i].SetRecentMove(false);
        }
        lastPreviewRow = -1;
        lastPreviewCol = -1;
        lastBlackRow = -1;
        lastBlackCol = -1;
    }

    public void PlaceMarker(Block.MarkerType markerType, int row, int col)
    {
        var index = row * gridSize + col;
        blocks[index].SetMarker(markerType);
        
        if (markerType == Block.MarkerType.Black)
        {
            if (lastBlackRow != -1 && (lastBlackRow != row || lastBlackCol != col))
            {
                var lastIndex = lastBlackRow * gridSize + lastBlackCol;
                blocks[lastIndex].SetRecentMove(false);
            }
            blocks[index].SetRecentMove(true);
            lastBlackRow = row;
            lastBlackCol = col;
        }

        if (lastPreviewRow == row && lastPreviewCol == col)
        {
            blocks[index].SetPreviewMarker(false);
            lastPreviewRow = -1;
            lastPreviewCol = -1;
        }
    }

    public void SetPreviewMarker(int row, int col, bool show)
    {
        if (lastPreviewRow != -1 && (lastPreviewRow != row || lastPreviewCol != col))
        {
            var lastIndex = lastPreviewRow * gridSize + lastPreviewCol;
            blocks[lastIndex].SetPreviewMarker(false);
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

    public void DisableAllBlockInteractions()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].StopAllCoroutines();
            blocks[i].previewSpriteRenderer.enabled = false;
            blocks[i].previewSpriteRenderer.sprite = null;
            blocks[i].SetRecentMove(false);
        }
        OnBlockClickedDelegate = null;
        lastPreviewRow = -1;
        lastPreviewCol = -1;
        lastBlackRow = -1;
        lastBlackCol = -1;
    }
    
    public void UpdateRecentMoveDisplay(GameManager.TurnType turnType)
    {
        if (lastBlackRow != -1)
        {
            blocks[lastBlackRow * gridSize + lastBlackCol].SetRecentMove(false);
        }
        if (turnType == GameManager.TurnType.PlayerA && lastBlackRow != -1)
        {
            blocks[lastBlackRow * gridSize + lastBlackCol].SetRecentMove(true);
        }
    }
}