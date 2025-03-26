using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField] private Block[] blocks;
    [SerializeField] private int gridSize = 15;

    public delegate void OnBlockClicked(int row, int col);

    public OnBlockClicked OnBlockClickedDelegate;

    private int lastPreviewRow = -1;
    private int lastPreviewCol = -1;

    public Block[] InitBlocks()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].InitMarker(i, blockIndex =>
            {
                var clickedRow = blockIndex / gridSize;
                var clickedCol = blockIndex % gridSize;
                OnBlockClickedDelegate?.Invoke(clickedRow, clickedCol);
            });
        }

        return blocks;
    }

    public void PlaceMarker(Block.MarkerType markerType, int row, int col)
    {
        var index = row * gridSize + col;
        blocks[index].SetMarker(markerType);
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

    public void ClearAllPreviews()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].StopAllCoroutines();
            if (blocks[i].markerSpriteRenderer != null)
            {
                blocks[i].markerSpriteRenderer.enabled = false;
                blocks[i].markerSpriteRenderer.sprite = null;    
            }

            lastPreviewRow = -1;
            lastPreviewCol = -1;
        }
    }

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