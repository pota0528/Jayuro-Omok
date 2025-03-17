using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Block : MonoBehaviour
{
    [SerializeField] private Sprite blackSprite;
    [SerializeField] private Sprite whiteSprite;
    [SerializeField] private Sprite forbiddenSprite;
    [SerializeField] private SpriteRenderer markerSpriteRenderer;
    [SerializeField] private TextMeshPro markerMoveIndexText;

    public enum MarkerType { None, Black, White, Forbidden }
    public MarkerType BlockType { get; private set; } = MarkerType.None;
    
    public delegate void OnBlockClicked(int index);
    private OnBlockClicked _onBlockClicked;

    public int _blockIndex { get; private set; } = -1;
    private SpriteRenderer _spriteRenderer;
    private bool isPutDown = false;
    public int MoveIndex { get; private set; } = -1;

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
                markerSpriteRenderer.sprite = blackSprite;
                break;
            case MarkerType.White:
                markerSpriteRenderer.sprite = whiteSprite;
                break;
            case MarkerType.Forbidden:
                markerSpriteRenderer.sprite = forbiddenSprite;
                break;
            case MarkerType.None:
                markerSpriteRenderer.sprite = null;
                break;
        }
    }

    public void SetMarkMoveIndex(int _moveIndex)
    {
        MoveIndex = _moveIndex;
        markerMoveIndexText.text = _moveIndex.ToString();
    }
    public void OnMouseUpAsButton() //매개변수나 다른 걸로 현재 OmokSprite(Black, White값이 필요)
    {
        _onBlockClicked?.Invoke(_blockIndex);
    }
}