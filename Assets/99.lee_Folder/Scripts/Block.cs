using System;
using UnityEngine;
using UnityEngine.EventSystems;
using lee_namespace;
using UnityEngine.Serialization;

namespace lee_namespace
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class Block : MonoBehaviour
    {
        public Sprite BlackSprite;
        public Sprite WhiteSprite;
        public SpriteRenderer markerSpriteRenderer;
        public SpriteRenderer previewSpriteRenderer;
        public Sprite preSprite;

        public enum MarkerType {None, Black, White}
        public delegate void OnBlockClicked(int index);
        private OnBlockClicked _onBlockClicked;
        private int _blockIndex;
        
        private void Awake()
        {
            markerSpriteRenderer = GetComponent<SpriteRenderer>();
            if (previewSpriteRenderer == null)
            {
                previewSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            }
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
                    previewSpriteRenderer.sprite = null; // 미리보기 제거
                    break;
                case MarkerType.White:
                    markerSpriteRenderer.sprite = WhiteSprite;
                    previewSpriteRenderer.sprite = null;
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
            previewSpriteRenderer.sprite = show ? preSprite : null; // 미리보기 표시/제거
            previewSpriteRenderer.color = new Color(1, 1, 1, show ? 1f : 1f); // 반투명 효과
        }

        public void OnMouseUpAsButton()
        {
            _onBlockClicked?.Invoke(_blockIndex); // 클릭 이벤트만 전달
            Debug.Log(_blockIndex);
        }
    }
}