using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopConfirmPopup : MonoBehaviour
{
    [SerializeField] private RectTransform confirmScroll;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Action _onConfirm;

    public void Show(string message, Action onConfirm)
    {
        messageText.text = message;
        _onConfirm = onConfirm;
        ShowConfirmScroll();
        gameObject.SetActive(true);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            _onConfirm?.Invoke();
            Close();
        });

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(Close);
       
    }

    public void ShowConfirmScroll()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(confirmScroll.DOSizeDelta(new Vector2(-200f, confirmScroll.sizeDelta.y), 0.5f));
    }
    

    private void Close()
    {
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
