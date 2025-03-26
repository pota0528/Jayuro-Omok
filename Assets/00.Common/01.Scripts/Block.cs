using System.Collections;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    public Sprite BlackSprite;         // 흑돌 스프라이트
    public Sprite WhiteSprite;         // 백돌 스프라이트
    public Sprite ForbiddenSprite;     // 금수 스프라이트
    public Sprite PreSprite;           // 미리보기 스프라이트
    public SpriteRenderer markerSpriteRenderer;  // 마커 표시용
    public SpriteRenderer previewSpriteRenderer; // 미리보기 표시용
    
    

    public enum MarkerType { None, Black, White, Forbidden }
    public MarkerType BlockType { get; private set; } = MarkerType.None;

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
        _onBlockClicked = onBlockClicked;
    }

    public void SetMarker(MarkerType markerType)
    {
        BlockType = markerType;
        switch (markerType)
        {
            case MarkerType.Black:
                markerSpriteRenderer.sprite = BlackSprite;
                previewSpriteRenderer.sprite = null;
                break;
            case MarkerType.White:
                markerSpriteRenderer.sprite = WhiteSprite;
                previewSpriteRenderer.sprite = null;
                break;
            case MarkerType.Forbidden:
                markerSpriteRenderer.sprite = ForbiddenSprite;
                previewSpriteRenderer.sprite = null;
                break;
            case MarkerType.None:
                markerSpriteRenderer.sprite = null;
                break;
        }
    }

    public void SetPreviewMarker(bool show)
    {
        if (show)
        {
            AudioManager.Instance.OnPutSelector();
            previewSpriteRenderer.sprite = PreSprite;
            previewSpriteRenderer.material = new Material(Shader.Find("Custom/FillAmountShader"));
            previewSpriteRenderer.material.SetFloat("_FillAmount", 0f); // 초기화
            StartCoroutine(FillPreviewCoroutine());
        }
        else
        {
            previewSpriteRenderer.sprite = null;
        }
    }

    private IEnumerator FillPreviewCoroutine()
    {
        float fillAmount = 0f;
        while (fillAmount < 1f)
        {
            fillAmount += Time.deltaTime * 1.5f; // 채우는 속도 조절
            previewSpriteRenderer.material.SetFloat("_FillAmount", fillAmount);
            yield return null;
        }
        
    }

    public void OnMouseUpAsButton()
    {
        _onBlockClicked?.Invoke(_blockIndex);
    }
}