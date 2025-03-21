using UnityEngine;

public class Block : MonoBehaviour
{
    public Sprite BlackSprite;         // 흑돌 스프라이트
    public Sprite WhiteSprite;         // 백돌 스프라이트
    public SpriteRenderer markerSpriteRenderer; // 마커 표시용
    public SpriteRenderer previewSpriteRenderer; // 미리보기 표시용
    public Sprite preSprite;           // 미리보기 스프라이트

    public enum MarkerType { None, Black, White } // 마커 타입 정의
    public delegate void OnBlockClicked(int index); // 클릭 이벤트 델리게이트
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

    // 블록 초기화: 인덱스와 클릭 이벤트 설정
    public void InitMarker(int blockIndex, OnBlockClicked onBlockClicked)
    {
        _blockIndex = blockIndex;
        SetMarker(MarkerType.None); // 초기 상태는 빈칸
        _onBlockClicked = onBlockClicked;
    }

    // 마커 설정
    public void SetMarker(MarkerType markerType)
    {
        Debug.Log("...");
        switch (markerType)
        {
            case MarkerType.Black:
                markerSpriteRenderer.sprite = BlackSprite;
                previewSpriteRenderer.sprite = null; // 마커가 설정되면 미리보기 제거
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

    // 미리보기 설정
    public void SetPreviewMarker(bool show)
    {
        previewSpriteRenderer.sprite = show ? preSprite : null; // 미리보기 표시/제거
        previewSpriteRenderer.color = new Color(1, 1, 1, show ? 0.5f : 1f); // 반투명 효과
    }

    // 마우스 클릭 이벤트
    public void OnMouseUpAsButton()
    {
        _onBlockClicked?.Invoke(_blockIndex); // 클릭 시 인덱스 전달
    }
}