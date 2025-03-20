using System;
using UnityEngine;
using UnityEngine.EventSystems;
using lee_namespace;

namespace lee_namespace
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class Block : MonoBehaviour
    {
        public Sprite BlackSprite;
        [SerializeField] private Sprite WhiteSprite;
        [SerializeField] private SpriteRenderer markerSpriteRenderer;
        public Sprite priviewSpriteRenderer;
        public Sprite preSprite;

        public enum MarkerType {None, Black, White}
        public delegate void OnBlockClicked(int index);
        private OnBlockClicked _onBlockClicked;
        private int _blockIndex;
        
        private SpriteRenderer _spriteRenderer;
        

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void InitMarker(int blockIndex, OnBlockClicked onBlockClicked)
        {
            _blockIndex = blockIndex;
            SetMarker(MarkerType.None);
            this._onBlockClicked = onBlockClicked;
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
        
        /// <summary>
        /// priview 표시 및 제거
        /// </summary>
        /// <param name="show">true of false</param>
        public void SetPreviewMarker(bool show)
        {
            priviewSpriteRenderer = show? preSprite : null;
        }

        public void OnMouseUpAsButton() 
        {
            _onBlockClicked?.Invoke(_blockIndex);
            markerSpriteRenderer.sprite = preSprite;
        }
    }
}