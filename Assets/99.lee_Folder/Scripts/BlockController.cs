using UnityEngine;
using lee_namespace;

namespace lee_namespace
{
    public class BlockController : MonoBehaviour
    {
        [SerializeField] private Block[] blocks;
        public delegate void OnBlockClicked(int row, int col);
        public OnBlockClicked OnBlockClickedDelegate;

        private int selectedRow = -1;
        private int selectedCol = -1;

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
    
        public void PlaceMarker(Block.MarkerType markerType, int row, int col)
        {
            var markerIndex = row * 15 + col;
            blocks[markerIndex].SetMarker(markerType);
            if (selectedRow == row && selectedCol == col)
            {
                blocks[markerIndex].SetPreviewMarker(false); // 돌이 놓이면 priview 제거하기
                selectedRow = -1;
                selectedCol = -1;
            }
        }
        
        // 미리보기 표시/제거 메서드
        public void SetPreviewMarker(int row, int col, bool show)
        {
            var markerIndex = row * 15 + col;
            if (show)
            {
                if (selectedRow != -1 && (selectedRow != row || selectedCol != col))
                {
                    blocks[selectedRow * 15 + selectedCol].SetPreviewMarker(false); // 이전 미리보기 제거
                }
                blocks[markerIndex].SetPreviewMarker(true);
                selectedRow = row;
                selectedCol = col;
            }
            else
            {
                blocks[markerIndex].SetPreviewMarker(false);
            }
        }
    }
}
