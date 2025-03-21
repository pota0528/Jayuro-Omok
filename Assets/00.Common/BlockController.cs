using UnityEngine;
using lee_namespace;

public class BlockController : MonoBehaviour
    {
        [SerializeField] private Block[] blocks;

        public delegate void OnBlockClicked(int row, int col);

        public OnBlockClicked OnBlockClickedDelegate;

        private int lastPreviewRow = -1;
        private int lastPreviewCol = -1;

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
            var index = row * 15 + col;
            blocks[index].SetMarker(markerType); // 현재 마커 타입으로 마커 설정
            // PlayerA 턴에서만 미리보기 제거 -> PlayerB(AI) 턴에도 지워지는 버그 발생..
            if (markerType == Block.MarkerType.Black && lastPreviewRow == row && lastPreviewCol == col)
            {
                blocks[index].SetPreviewMarker(false);
                lastPreviewRow = -1;
                lastPreviewCol = -1;
            }

            // 마커 유지 되는과정 보기
            if (markerType == Block.MarkerType.Black && blocks[index].markerSpriteRenderer.sprite != blocks[index].BlackSprite)
            {
                blocks[index].markerSpriteRenderer.sprite = blocks[index].BlackSprite;
            }
            else if (markerType == Block.MarkerType.White && blocks[index].markerSpriteRenderer.sprite != blocks[index].WhiteSprite)
            {
                blocks[index].markerSpriteRenderer.sprite = blocks[index].WhiteSprite;
            }
            
        }

        // 미리보기 표시/제거 메서드
        public void SetPreviewMarker(int row, int col, bool show)
        {
            if (lastPreviewRow != -1 && (lastPreviewRow != row || lastPreviewCol != col))
            {
                var lastIndex = lastPreviewRow * 15 + lastPreviewCol;
                blocks[lastIndex].SetPreviewMarker(false);
            }
            
            var index = row * 15 + col;
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
    }