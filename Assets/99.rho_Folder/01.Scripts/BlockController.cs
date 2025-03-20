using UnityEngine;

namespace rho_namespace
{
    public class BlockController : MonoBehaviour
    {
        [SerializeField] private Block[] blocks;
    
        public delegate void OnBlockClicked(int row, int col);
        public OnBlockClicked OnBlockClickedDelegate;

        public void InitBlocks()
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].InitMarker(i, blockIndex =>
                {
                    var clickedRow = blockIndex / 15;
                    var clickedCol = blockIndex % 15;
                    OnBlockClickedDelegate?.Invoke(clickedRow, clickedCol);
                });
            }
        }
    
        public void PlaceMarker(Block.MarkerType markerType, int row, int col, int moveIndex)
        {
            // row, col을 index로 변환
            var markerIndex = row * 15 + col;
        
            // Block에게 마커 표시
            blocks[markerIndex].SetMarker(markerType);

            if (markerType != Block.MarkerType.Forbidden)
            {
                blocks[markerIndex].SetMarkMoveIndex(moveIndex);
            }
        }
    }
}
