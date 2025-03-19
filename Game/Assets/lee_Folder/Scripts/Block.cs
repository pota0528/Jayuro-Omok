using System;
using UnityEngine;
using UnityEngine.EventSystems;
using lee_namespace;

namespace lee_namespace
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class Block :
        MonoBehaviour
    {
        [SerializeField] private Sprite BlackSprite;

        [SerializeField] private Sprite WhiteSprite;

        [SerializeField] private SpriteRenderer markerSpriteRenderer;

        public enum MarkerType
        {
            None,
            Black,
            White
        }

        public delegate void OnBlockClicked(int index);

        private OnBlockClicked _onBlockClicked;

        private int _blockIndex;
        private SpriteRenderer _spriteRenderer;
        private bool isPutDown = false;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void InitMarker(int blockIndex, OnBlockClicked onBlockClicked)
        {
            _blockIndex = blockIndex;
            SetMarker(MarkerType.None);
            this._onBlockClicked = onBlockClicked;
            //BlockController의 row와 col 초기화
            //OnBlockClickedDelegate?.Invoke(clickedRow, clickedCol);를 미리 넣기
            //BlockController.OnBlockClickedDelegate가 Block.onBlockClicked에 이식
            //그러므로 BlockController.onBlockClicked에 값을 넣고, Block.onBlockClicked을 시행이 될 가능성이 높음.
        }

        public void SetMarker(MarkerType markerType)
        {
            switch (markerType)
            {
                case MarkerType.Black:
                    markerSpriteRenderer.sprite = BlackSprite;
                    break;
                case MarkerType.White:
                    markerSpriteRenderer.sprite = WhiteSprite;
                    break;
                case MarkerType.None:
                    markerSpriteRenderer.sprite = null;
                    break;
            }
        }

        public void OnMouseUpAsButton() //매개변수나 다른 걸로 현재 OmokSprite(Black, White값이 필요)
        {
            _onBlockClicked?.Invoke(_blockIndex);
        }
    }
}